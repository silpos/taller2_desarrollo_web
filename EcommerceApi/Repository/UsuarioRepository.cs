using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceApi.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public UsuarioRepository(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> Registrar(UsuarioDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Nombre) || string.IsNullOrWhiteSpace(item.Email) || string.IsNullOrWhiteSpace(item.Password))
            {
                throw new ArgumentException("Nombre, correo y contraseña son obligatorios.");
            }

            var email = item.Email.Trim().ToLower();

            var usuarioExiste = await _context.Usuario.AnyAsync(x => x.Email == email);
            if (usuarioExiste)
            {
                throw new ArgumentException("El usuario ya existe.");
            }

            var usuario = new Usuario
            {
                Nombre = item.Nombre.Trim(),
                Email = email
            };

            // PasswordHasher convierte la contrasena en un hash: en la base de datos
            // nunca queda la contrasena real.
            var passwordHasher = new PasswordHasher<Usuario>();
            usuario.Password = passwordHasher.HashPassword(usuario, item.Password);

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return "Usuario registrado correctamente.";
        }

        public async Task<string> Login(LoginDto item)
        {
            var email = item.Email.Trim().ToLower();
            var usuario = await _context.Usuario.FirstOrDefaultAsync(x => x.Email == email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }

            // Compara la contrasena escrita con el hash guardado
            var passwordHasher = new PasswordHasher<Usuario>();
            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.Password, item.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }

            return GenerarToken(usuario);
        }

        // Crea el JWT: un texto firmado que lleva dentro el Id, el nombre y el correo del usuario
        private string GenerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
