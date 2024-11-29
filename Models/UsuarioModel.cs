using MasterIdiomas.Helper;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class UsuarioModel
    {
        [Key] // Indica que este campo é a chave primária
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DataCadastro { get; set; } = DateTime.Now; // Define valor padrão

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
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