using MasterIdiomas.Helper;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class UsuarioModel
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Por favor, insira o nome.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DataCadastro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
        public string Senha { get; set; } = string.Empty;

        // Verifica se a senha informada é válida comparando com o hash armazenado
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

        // Gera uma nova senha aleatória, converte para hash e retorna a senha gerada
        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            Senha = novaSenha.GerarHash();
            return novaSenha;
        }
    }
}