using EcommerceApi.Configuration;
using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using EcommerceApi.Repository;
using EcommerceApi.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers y OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Repositorios
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

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

// JWT: configuracion desde la seccion "JwtSettings" de appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

// HmacSha256 necesita una clave de al menos 32 caracteres; si es mas corta, avisa al arrancar
if (jwtSettings == null || Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException("JwtSettings:Key debe tener al menos 32 caracteres.");
}

// Autenticacion: la API revisa que el token venga firmado con nuestra clave,
// que sea de nuestro Issuer/Audience y que no este vencido
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

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

// Primero se identifica al usuario (token) y despues se revisan los permisos
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
