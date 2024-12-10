using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "Por favor, insira o e-mail.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; }
    }
}