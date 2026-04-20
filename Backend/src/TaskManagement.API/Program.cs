// Nhớ thêm thư viện này để dùng DbContext và SQL Server
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using TaskManagement.Infrastructure.Data;

using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 1. Mở tính năng Controllers (Chuẩn bị cho các API Login, Task...)
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("FixedWindow", httpContext =>
    {
        var userKey = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var partitionKey = !string.IsNullOrWhiteSpace(userKey)
            ? $"user:{userKey}"
            : $"ip:{httpContext.Connection.RemoteIpAddress}";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
});

// Đăng ký Custom Services từ Extension Methods
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddWorkspaceServices();
builder.Services.AddAuditLogServices();

// 2. Khai báo Policy CORS
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          // Cho phép Vue.js gọi vào (các port dev server có thể khác nhau)
                          policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                      });
});

// 3. CẤU HÌNH CODE-FIRST (ENTITY FRAMEWORK CORE)
// Luôn dùng SQL Server để dữ liệu được lưu trữ vĩnh viễn (comment, notification, v.v.)
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryFallback = false;

if (!string.IsNullOrWhiteSpace(defaultConnection) && builder.Environment.IsDevelopment())
{
    useInMemoryFallback = !CanConnectToSqlServer(defaultConnection);
    if (useInMemoryFallback)
    {
        Console.WriteLine("SQL Server is unavailable. Falling back to InMemory database for Development.");
    }
}

if (!string.IsNullOrWhiteSpace(defaultConnection) && !useInMemoryFallback)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(defaultConnection,
           sqlOptions => sqlOptions.EnableRetryOnFailure(
               maxRetryCount: 5,
               maxRetryDelay: TimeSpan.FromSeconds(30),
               errorNumbersToAdd: null)));
}
else
{
    // Fallback to InMemory chỉ khi không có connection string
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevInMemoryDb"));
}


var app = builder.Build();

// ---------------- CẤU HÌNH PIPELINE ----------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<TaskManagement.API.Middlewares.PerformanceMiddleware>();
app.UseMiddleware<TaskManagement.API.Middlewares.IpWhitelistMiddleware>();

// app.UseHttpsRedirection(); // Tắt HTTPS redirect để Axios có thể gọi vào HTTP 5136 mà ko bị CORS lỗi

// Middleware for Google OAuth popup support
app.Use(async (context, next) =>
{
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
    context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
    await next();
});


// 4. KÍCH HOẠT CORS (Vị trí cực kỳ quan trọng, phải đứng trước Authorization)
app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.UseDefaultFiles(); // Phải gọi dòng này trước
app.UseStaticFiles();

// Serve uploaded files from /uploads
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// 5. Nối các endpoint vào Controllers
app.MapControllers();
app.MapHub<TaskManagement.API.Hubs.KanbanHub>("/kanban-hub");
app.MapHub<TaskManagement.API.Hubs.NotificationHub>("/notification-hub");

// TỰ ĐỘNG MIGRATE VÀ SEED DỮ LIỆU KHI STARTUP (PM: Vui lòng không xóa đoạn này)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    try 
    {
        // QUAN TRỌNG: Không xóa DB mỗi lần khởi động
        // await context.Database.EnsureDeletedAsync();
        // await context.Database.EnsureCreatedAsync();
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        await TaskManagement.Infrastructure.Data.DataSeeder.SeedMockDataAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi khi Migrate/Seed: " + ex.Message);
    }
}

app.MapFallbackToFile("index.html");
app.Run();

static bool CanConnectToSqlServer(string connectionString)
{
    try
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            ConnectTimeout = 3
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();
        return true;
    }
    catch
    {
        return false;
    }
}
