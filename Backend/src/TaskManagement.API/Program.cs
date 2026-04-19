using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Mở tính năng Controllers (Chuẩn bị cho các API Login, Task...)
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

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
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
bool useInMemory = false;

// In Development, we can pre-check if we want to force InMemory if SQL Server is known to be problematic
if (builder.Environment.IsDevelopment())
{
    // Forcing true to resolve immediate 500 errors caused by local SQL Server permission issues.
    useInMemory = true; 
}

if (!string.IsNullOrWhiteSpace(defaultConnection) && !useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(defaultConnection,
           sqlOptions => sqlOptions.EnableRetryOnFailure(
               maxRetryCount: 3, // Reduced for faster dev fallback
               maxRetryDelay: TimeSpan.FromSeconds(5),
               errorNumbersToAdd: null)));
}
else
{
    // Fallback to InMemory
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

app.UseRouting();

// 4. KÍCH HOẠT CORS (Vị trí cực kỳ quan trọng, phải đứng trước Authorization và các Middleware query DB)
app.UseCors(myAllowSpecificOrigins);

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

app.UseAuthentication();
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
        // Attempt to apply existing migrations
        await context.Database.MigrateAsync();
        await TaskManagement.Infrastructure.Data.DataSeeder.SeedMockDataAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("======= DATABASE MIGRATION ERROR - ATTEMPTING FALLBACK =======");
        Console.WriteLine("Message: " + ex.Message);
        
        try
        {
            // Fallback: If migrations fail (e.g. database exists but history table is missing)
            // try EnsureCreated to at least get the tables ready for dev.
            await context.Database.EnsureCreatedAsync();
            await TaskManagement.Infrastructure.Data.DataSeeder.SeedMockDataAsync(context);
            Console.WriteLine("Fallback successful: Database created/verified via EnsureCreated.");
        }
        catch (Exception fallbackEx)
        {
            Console.WriteLine("CRITICAL: Fallback failed as well.");
            Console.WriteLine("Fallback Message: " + fallbackEx.Message);
            if (fallbackEx.InnerException != null) Console.WriteLine("Inner: " + fallbackEx.InnerException.Message);
        }
        Console.WriteLine("==============================================================");
    }
}

app.MapFallbackToFile("index.html");
app.Run();
