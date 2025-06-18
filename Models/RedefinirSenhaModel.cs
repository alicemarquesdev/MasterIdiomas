using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo utilizado para redefinir a senha do usuário.
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "Por favor, insira o e-mail.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email informado não é válido.")]
        public required string Email { get; set; }
    }
}