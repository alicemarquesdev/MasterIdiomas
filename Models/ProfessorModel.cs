using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class ProfessorModel
    {
        [Key]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "Por favor, informe o nome do professor.")]
        [StringLength(100, ErrorMessage = "O nome do professor não pode exceder 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, selecione o gênero.")]
        public GeneroEnum Genero { get; set; } = GeneroEnum.Outro;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "1940-01-01", "2010-12-31", ErrorMessage = "A data de nascimento deve estar entre 01/01/1940 e 31/12/2010.")]
        public DateTime DataNascimento { get; set; } = default;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public StatusEnum Status { get; set; } = StatusEnum.Ativo;

        public IList<CursoModel> Cursos { get; set; } = new List<CursoModel>();
    }
}