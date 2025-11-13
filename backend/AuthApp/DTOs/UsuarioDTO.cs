namespace AuthApp.DTOs
{
    public class UsuarioDTO
    {
        public int ID { get; set; }
        public required string Login { get; set; }           
        public required string NomeCompleto { get; set; }    
        public required string Email { get; set; }           
        public int Acessos { get; set; }
    }

    public class UsuarioCreateDTO
    {
        public required string Login { get; set; }          
        public required string NomeCompleto { get; set; }    
        public required string Email { get; set; }           
        public required string Senha { get; set; }           
    }
}