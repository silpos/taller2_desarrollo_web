using EcommerceApi.Configuration;
using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Repository;
using EcommerceApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers y OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Repositorios
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Base de datos MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(
        builder.Configuration.GetConnectionString("DefaultConnection")!
    )
);

// Cloudinary: credenciales desde la seccion "CloudinarySettings" y servicio por DI
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORS
app.UseCors("AngularPolicy");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
