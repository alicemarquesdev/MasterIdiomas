using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo para alteração de senha do usuário.
    public class AlterarSenhaModel
    {
        public int Id { get; set; } // Identificador do usuário

        [Required(ErrorMessage = "Digite a senha atual do usuário.")]
        public required string SenhaAtual { get; set; } // Senha atual do usuário

        [Required(ErrorMessage = "Digite a nova senha do usuário.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
           ErrorMessage = "A senha deve conter de 8 a 20 caracteres, sendo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
        public required string NovaSenha { get; set; } // Nova senha do usuário

        [Required(ErrorMessage = "Confirme a nova senha do usuário.")]
        [Compare("NovaSenha", ErrorMessage = "Senha não confere com a nova senha.")]
        public required string ConfirmarNovaSenha { get; set; } // Confirmação da nova senha
    }
}
