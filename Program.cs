using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SESSION CONFIGURATION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.Name = ".ShopifyClone.Session";
    options.Cookie.HttpOnly = false; // ← THAY ĐỔI: false để debug
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // ← LAX
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .SetIsOriginAllowed(origin => true) // ← CHO PHÉP MỌI ORIGIN
            .AllowAnyHeader() // chấp nhận mọi loại header
            .AllowAnyMethod() // chấp nhận mọi phương thức HTTP (GET, POST, PUT, DELETE, v.v.)
            .AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();