using AuthApp.DTOs;
using AuthApp.Models;

namespace AuthApp.Services
{
    public interface IAuthService
    {
        Task<Usuario?> Autenticar(LoginDTO loginDTO);  
        Task<Usuario?> Registrar(UsuarioCreateDTO usuarioDTO);  
        Task<List<UsuarioDTO>> ListarUsuarios();
        Task IncrementarAcessos(int usuarioId);
    }
}