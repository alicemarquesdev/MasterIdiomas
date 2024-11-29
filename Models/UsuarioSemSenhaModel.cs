using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class UsuarioSemSenhaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o seu nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite o seu email.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é valido!")]
        public string Email { get; set; } = string.Empty;
    }
}