using System.Security.Claims;
using EcommerceApi.Interfaces;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    // Cada accion responde en dos rutas: la REST que pide el taller (/api/producto)
    // y la de estilo nombrado del ejemplo de clase (/Producto/GetProductos, etc.).
    // Consultar es publico; crear, editar y eliminar exigen haber iniciado sesion ([Authorize]).
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _repository;

        public ProductoController(IProductoRepository repository)
        {
            _repository = repository;
        }

        // GET /api/producto
        // GET /Producto/GetProductos
        [HttpGet("api/producto")]
        [HttpGet("Producto/GetProductos")]
        public async Task<ActionResult<List<Producto>>> Get()
        {
            return Ok(await _repository.GetProductos());
        }

        // POST /api/producto
        // POST /Producto/CreateProducto
        [Authorize]
        [HttpPost("api/producto")]
        [HttpPost("Producto/CreateProducto")]
        public async Task<ActionResult<Producto>> Post([FromBody] ProductoDto item)
        {
            try
            {
                // El Id del usuario sale del token (claim NameIdentifier): ese usuario queda como dueno
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var producto = await _repository.CreateProducto(item, userId);
                return Ok(producto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT /api/producto/{id}
        // PUT /Producto/UpdateProducto/{id}
        [Authorize]
        [HttpPut("api/producto/{id}")]
        [HttpPut("Producto/UpdateProducto/{id}")]
        public async Task<ActionResult<Producto>> Put([FromRoute] int id, [FromBody] ProductoDto item)
        {
            try
            {
                var producto = await _repository.UpdateProducto(id, item);
                return Ok(producto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE /api/producto/{id}
        // DELETE /Producto/DeleteProducto/{id}
        [Authorize]
        [HttpDelete("api/producto/{id}")]
        [HttpDelete("Producto/DeleteProducto/{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var respuesta = await _repository.DeleteProducto(id);
                return Ok(new { mensaje = respuesta });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
