using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class ProfessorModel
    {
        [Key]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public string? Genero { get; set; } = "Outro";

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "1940-01-01", "2010-12-31", ErrorMessage = "A data de nascimento deve estar entre 01/01/1940 e 31/12/2010.")]
        public DateTime DataNascimento { get; set; } = default;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado é inválido.")]
        public string Email { get; set; } = string.Empty;

        public DateTime? DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public IList<CursoModel> Cursos { get; set; }
    }
}