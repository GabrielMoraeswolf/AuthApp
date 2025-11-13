using Microsoft.EntityFrameworkCore;
using AuthApp.Data;
using AuthApp.Models;
using AuthApp.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace AuthApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> Autenticar(LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.Login) || string.IsNullOrWhiteSpace(loginDTO.Senha))
                return null;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == loginDTO.Login);

            if (usuario == null || !VerifyPassword(loginDTO.Senha, usuario.Senha))
                return null;

            usuario.Acessos++;
            usuario.DataAtualizacao = DateTime.Now;
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario?> Registrar(UsuarioCreateDTO usuarioDTO)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Login == usuarioDTO.Login))
                throw new Exception("Login já existe");

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email))
                throw new Exception("Email já existe");

            var usuario = new Usuario
            {
                Login = usuarioDTO.Login,
                NomeCompleto = usuarioDTO.NomeCompleto,
                Email = usuarioDTO.Email,
                Senha = HashPassword(usuarioDTO.Senha),
                Acessos = 0
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<List<UsuarioDTO>> ListarUsuarios()
        {
            return await _context.Usuarios
                .OrderBy(u => u.ID)
                .Select(u => new UsuarioDTO
                {
                    ID = u.ID,
                    Login = u.Login,
                    NomeCompleto = u.NomeCompleto,
                    Email = u.Email,
                    Acessos = u.Acessos
                })
                .ToListAsync();
        }

        public async Task IncrementarAcessos(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario != null)
            {
                usuario.Acessos++;
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var newHash = HashPassword(password);
            return newHash == hashedPassword;
        }
    }
}