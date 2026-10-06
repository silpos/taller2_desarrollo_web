namespace EcommerceApi.Models.Dtos
{
    public class ProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // El cliente envia la imagen en Base64; la API la sube a Cloudinary
        // y en la base de datos solo se guarda la URL (Producto.ImagenUrl).
        public string? ImagenBase64 { get; set; }
    }
}
