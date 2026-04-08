// Nhá»› thĂªm thÆ° viá»‡n nĂ y Ä‘á»ƒ dĂ¹ng DbContext vĂ  SQL Server
using Microsoft.EntityFrameworkCore;


using TaskManagement.Infrastructure.Data;

using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Má»Ÿ tĂ­nh nÄƒng Controllers (Chuáº©n bá»‹ cho cĂ¡c API Login, Task...)
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

// ÄÄƒng kĂ½ Custom Services tá»« Extension Methods
builder.Services.AddAuthServices(builder.Configuration);

// 2. Khai bĂ¡o Policy CORS
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          // Cho phĂ©p Vue.js gá»i vĂ o (cĂ¡c port dev server cĂ³ thá»ƒ khĂ¡c nhau)
                          policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                      });
});

// 3. Cáº¤U HĂŒNH CODE-FIRST (ENTITY FRAMEWORK CORE)
// Náº¿u cĂ³ connection string sáº½ dĂ¹ng SQL Server; náº¿u khĂ´ng (vĂ­ dá»¥ mĂ´i trÆ°á»ng dev nhanh),
// fallback sang InMemory DB Ä‘á»ƒ báº¡n cĂ³ thá»ƒ thá»­ register/login dá»… dĂ ng.
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

// ---------------- Cáº¤U HĂŒNH PIPELINE ----------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // Táº¯t HTTPS redirect Ä‘á»ƒ Axios cĂ³ thá»ƒ gá»i vĂ o HTTP 5136 mĂ  ko bá»‹ CORS lá»—i

// 4. KĂCH HOáº T CORS (Vá»‹ trĂ­ cá»±c ká»³ quan trá»ng, pháº£i Ä‘á»©ng trÆ°á»›c Authorization)
app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles(); // Pháº£i gá»i dĂ²ng nĂ y trÆ°á»›c
app.UseStaticFiles();
// 5. Ná»‘i cĂ¡c endpoint vĂ o Controllers
app.MapControllers();
// Hubs are currently disabled or deleted


// Tá»° Äá»˜NG MIGRATE VĂ€ SEED Dá»® LIá»†U KHI STARTUP (PM: Vui lĂ²ng khĂ´ng xĂ³a Ä‘oáº¡n nĂ y)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    try 
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        // await context.Database.MigrateAsync();
        // await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lá»—i khi Migrate/Seed: " + ex.Message);
    }
}

app.MapFallbackToFile("index.html");
app.Run();