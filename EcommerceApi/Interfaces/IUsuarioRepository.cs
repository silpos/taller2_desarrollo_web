using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<string> Registrar(UsuarioDto item);
        Task<string> Login(LoginDto item);
    }
}
