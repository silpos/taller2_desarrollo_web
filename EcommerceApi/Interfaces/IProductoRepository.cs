using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Interfaces
{
    public interface IProductoRepository
    {
        Task<List<Producto>> GetProductos();
        Task<Producto> CreateProducto(ProductoDto item, int userId);
        Task<Producto> UpdateProducto(int idProducto, ProductoDto item);
        Task<string> DeleteProducto(int idProducto);
    }
}
