using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo que representa os dados de um aluno no sistema.
    public class AlunoModel
    {
        [Key]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "Digite o seu nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Zá-úÁ-Úà-ùÀ-Ùã-õÃ-ÕçÇ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public required GeneroEnum Genero { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "1950-01-01", "2015-12-31", ErrorMessage = "A data de nascimento deve ser a partir de 01/01/1950.")]
        public required DateTime DataNascimento { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public int QuantidadeCursos { get; set; } = 0;

        public virtual IList<AlunoCursoModel> AlunoCurso { get; set; } = new List<AlunoCursoModel>();
    }
}
