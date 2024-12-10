using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class UsuarioSemSenhaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Por favor, insira o seu nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, insira o seu e-mail.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; } = string.Empty;
    }
}