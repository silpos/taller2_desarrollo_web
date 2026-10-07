using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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

        // Dueno del producto (el usuario que lo creo). Es opcional (int?) porque los
        // productos creados antes de tener usuarios no tienen dueno.
        public int? UsuarioId { get; set; }

        // [JsonIgnore]: al devolver productos en JSON no se incluye el usuario completo
        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
