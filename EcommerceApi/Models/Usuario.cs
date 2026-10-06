namespace EcommerceApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Aqui NUNCA se guarda la contrasena tal cual: se guarda el hash que genera PasswordHasher
        public string Password { get; set; } = string.Empty;

        // Un usuario puede tener muchos productos (relacion uno a muchos)
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
