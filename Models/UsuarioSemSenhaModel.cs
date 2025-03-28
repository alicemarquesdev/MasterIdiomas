using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo de dados para alterar os dados do usuário sem alterar a senha
    public class UsuarioSemSenhaModel
    {
        public int Id { get; set; }

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
        [Range(typeof(DateTime), "1940-01-01", "2010-12-31", ErrorMessage = "A data de nascimento deve estar entre 01/01/1940 e 31/12/2010.")]
        public required DateTime DataNascimento { get; set; }
    }
}