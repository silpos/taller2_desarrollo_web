using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Precision(18, 2)]
        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string? ImagenUrl { get; set; }

    }
}
