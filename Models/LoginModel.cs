using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo utilizado para o login do usuário.
    public class LoginModel
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email informado não é válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public required string Senha { get; set; }
    }
}