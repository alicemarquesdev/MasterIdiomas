using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo utilizado para redefinir a senha do usuário.
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "Por favor, insira o e-mail.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public required string Email { get; set; }
    }
}