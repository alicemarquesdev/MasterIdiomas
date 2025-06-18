using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo que representa os dados de um professor no sistema.
    public class ProfessorModel
    {
        [Key]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "Por favor, informe o nome do professor.")]
        [StringLength(30, ErrorMessage = "O nome do professor não pode exceder 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Zá-úÁ-Úà-ùÀ-Ùã-õÃ-ÕçÇ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public required GeneroEnum Genero { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "1940-01-01", "2010-12-31", ErrorMessage = "A data de nascimento deve estar entre 01/01/1940 e 31/12/2010.")]
        public required DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email informado não é válido.")]
        public required string Email { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public int QuantidadeCursos { get; set; } = 0;

        public IList<CursoModel> Cursos { get; set; } = new List<CursoModel>();
    }
}