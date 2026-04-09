// Nhớ thêm thư viện này để dùng DbContext và SQL Server
using Microsoft.EntityFrameworkCore;


using TaskManagement.Infrastructure.Data;

using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Mở tính năng Controllers (Chuẩn bị cho các API Login, Task...)
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

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
// Nếu có connection string sẽ dùng SQL Server; nếu không (ví dụ môi trường dev nhanh),
// fallback sang InMemory DB để bạn có thể thử register/login dễ dàng.
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsDevelopment())
{
    // For local development, prefer an in-memory DB so you can test register/login
    // without having a local SQL Server instance configured.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevInMemoryDb"));
}
else if (!string.IsNullOrWhiteSpace(defaultConnection))
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
    // Fallback to InMemory if nothing else
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevInMemoryDb"));
}


var app = builder.Build();

// ---------------- CẤU HÌNH PIPELINE ----------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
app.UseAuthorization();
app.UseDefaultFiles(); // Phải gọi dòng này trước
app.UseStaticFiles();
// 5. Nối các endpoint vào Controllers
app.MapControllers();
app.MapHub<TaskManagement.API.Hubs.KanbanHub>("/kanban-hub");

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
        // await context.Database.MigrateAsync();
        // await DatabaseSeeder.SeedAsync(context); removed
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi khi Migrate/Seed: " + ex.Message);
    }
}

app.MapFallbackToFile("index.html");
app.Run();
