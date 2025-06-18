using MasterIdiomas.Enums;
using MasterIdiomas.Helper;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo que representa os dados do usuário no sistema.
    public class UsuarioModel
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Por favor, insira o nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Zá-úÁ-Úà-ùÀ-Ùã-õÃ-ÕçÇ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email informado não é válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public required GeneroEnum Genero { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public required DateTime DataNascimento { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
        public required string Senha { get; set; }

        // Verifica se a senha informada é válida, gerando hash para a senha inserida e comparando com o hash da senha armazenada no banco de dados.
        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }


        // Converte a senha existente para hash
        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }

        // Define uma nova senha, convertendo-a para hash
        public void SetNovaSenha(string novaSenha)
        {
            Senha = novaSenha.GerarHash();
        }

        // Gera uma nova senha aleatória, usada para redefinir a senha do usuário.
        // A senha é enviada por email.
        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            return novaSenha;
        }
    }
}
