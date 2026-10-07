namespace EcommerceApi.Models.Dtos
{
    // Datos que llegan en el body del registro
    public class UsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
