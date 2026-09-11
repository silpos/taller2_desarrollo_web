using EcommerceApi.DB;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetProductos()
        {
            return await _context.Producto.ToListAsync();
        }

        public async Task<Producto> CreateProducto(ProductoDto item)
        {
            Validar(item);

            Producto nuevoProducto = new()
            {
                Nombre = item.Nombre!.Trim(),
                Descripcion = item.Descripcion?.Trim(),
                Precio = item.Precio,
                Stock = item.Stock
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
