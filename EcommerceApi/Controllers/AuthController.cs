using EcommerceApi.Interfaces;
using EcommerceApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    // Rutas: POST /api/Auth/Register y POST /api/Auth/Login
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto item)
        {
            try
            {
                return Ok(await _usuarioRepository.Registrar(item));
            }
            catch (ArgumentException ex)
            {
                // 400: datos vacios o correo ya registrado
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto item)
        {
            try
            {
                // Si el login es correcto la respuesta es el token (texto)
                return Ok(await _usuarioRepository.Login(item));
            }
            catch (UnauthorizedAccessException ex)
            {
                // 401: correo o contrasena incorrectos
                return Unauthorized(ex.Message);
            }
        }
    }
}
