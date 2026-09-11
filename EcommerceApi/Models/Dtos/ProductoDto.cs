namespace EcommerceApi.Models.Dtos
{
    public class ProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
