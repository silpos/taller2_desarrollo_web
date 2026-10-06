using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductoRepository(AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<Producto>> GetProductos()
        {
            return await _context.Producto.ToListAsync();
        }

        public async Task<Producto> CreateProducto(ProductoDto item, int userId)
        {
            Validar(item);

            // La imagen es opcional: si viene en Base64 se sube a Cloudinary
            // y se guarda solo la URL que devuelve el servicio.
            string? imagenUrl = null;
            if (!string.IsNullOrWhiteSpace(item.ImagenBase64))
            {
                imagenUrl = await _cloudinaryService.UploadImageAsync(item.ImagenBase64);
            }

            Producto nuevoProducto = new()
            {
                Nombre = item.Nombre!.Trim(),
                Descripcion = item.Descripcion?.Trim(),
                Precio = item.Precio,
                Stock = item.Stock,
                ImagenUrl = imagenUrl,
                UsuarioId = userId // dueno del producto: el usuario del token
            };

            await _context.Producto.AddAsync(nuevoProducto);
            await _context.SaveChangesAsync();

            return nuevoProducto;
        }

        public async Task<Producto> UpdateProducto(int idProducto, ProductoDto item)
        {
            var productoExiste = await _context.Producto
                .FirstOrDefaultAsync(producto => producto.Id == idProducto);

            if (productoExiste == null)
            {
                throw new KeyNotFoundException("El producto no fue encontrado.");
            }

            Validar(item);

            productoExiste.Nombre = item.Nombre!.Trim();
            productoExiste.Descripcion = item.Descripcion?.Trim();
            productoExiste.Precio = item.Precio;
            productoExiste.Stock = item.Stock;

            // Si se envia una imagen nueva se reemplaza; si no, se conserva la actual.
            if (!string.IsNullOrWhiteSpace(item.ImagenBase64))
            {
                productoExiste.ImagenUrl = await _cloudinaryService.UploadImageAsync(item.ImagenBase64);
            }

            await _context.SaveChangesAsync();

            return productoExiste;
        }

        public async Task<string> DeleteProducto(int idProducto)
        {
            var productoExiste = await _context.Producto
                .FirstOrDefaultAsync(producto => producto.Id == idProducto);

            if (productoExiste == null)
            {
                throw new KeyNotFoundException("El producto no fue encontrado.");
            }

            _context.Producto.Remove(productoExiste);
            await _context.SaveChangesAsync();

            return "El producto fue eliminado con exito.";
        }

        // Reglas de validacion pedidas en el taller: el nombre no puede estar vacio,
        // el precio no puede ser negativo y el stock no puede ser negativo.
        private static void Validar(ProductoDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Nombre))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacio.");
            }

            if (item.Precio < 0)
            {
                throw new ArgumentException("El precio del producto no puede ser negativo.");
            }

            if (item.Stock < 0)
            {
                throw new ArgumentException("El stock del producto no puede ser negativo.");
            }
        }
    }
}
