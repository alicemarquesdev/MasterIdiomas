using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class RedefinirSenhaModel
    {
        [Required(ErrorMessage = "Digite o e-mail")]
        public string Email { get; set; }
    }
}