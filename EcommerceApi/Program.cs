using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Repository;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
