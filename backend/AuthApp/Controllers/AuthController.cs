using Microsoft.AspNetCore.Mvc;
using AuthApp.Services;
using AuthApp.DTOs;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDTO.Login) || string.IsNullOrWhiteSpace(loginDTO.Senha))
                    return BadRequest(new { message = "Login e senha são obrigatórios" });

                var usuario = await _authService.Autenticar(loginDTO);

                if (usuario == null)
                    return Unauthorized(new { message = "Credenciais inválidas" });

                return Ok(new
                {
                    id = usuario.ID,
                    login = usuario.Login,
                    nomeCompleto = usuario.NomeCompleto,
                    email = usuario.Email,
                    acessos = usuario.Acessos
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioCreateDTO usuarioDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioDTO.Login) || 
                    string.IsNullOrWhiteSpace(usuarioDTO.Senha) ||
                    string.IsNullOrWhiteSpace(usuarioDTO.NomeCompleto) ||
                    string.IsNullOrWhiteSpace(usuarioDTO.Email))
                    return BadRequest(new { message = "Todos os campos são obrigatórios" });

                var usuario = await _authService.Registrar(usuarioDTO);

                return Ok(new { message = "Usuário registrado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            try
            {
                var usuarios = await _authService.ListarUsuarios();
                return Ok(usuarios);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}