using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    public class CursoModel
    {
        [Key]
        public int CursoId { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o idioma do curso.")]
        public string Idioma { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, informe o turno do curso.")]
        public string Turno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, informe o nível do curso.")]
        public string Nivel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, informe a data de início do curso.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Por favor, informe a carga horária do curso.")]
        public int CargaHoraria { get; set; }

        [Required(ErrorMessage = "Por favor, informe o número máximo de alunos para o curso.")]
        public int MaxAlunos { get; set; }

        public StatusCursoEnum Status { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public int ProfessorId { get; set; }

        public ProfessorModel Professor { get; set; }

        public virtual IList<AlunoCursoModel> AlunoCurso { get; set; } = new List<AlunoCursoModel>();
    }
}