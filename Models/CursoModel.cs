using MasterIdiomas.Enums;
using System.ComponentModel.DataAnnotations;

namespace MasterIdiomas.Models
{
    // Modelo que representa os dados de um curso no sistema.
    public class CursoModel
    {
        [Key]
        public int CursoId { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o idioma do curso.")]
        public required string Idioma { get; set; }

        [Required(ErrorMessage = "Por favor, informe o turno do curso.")]
        public required TurnoEnum Turno { get; set; }

        [Required(ErrorMessage = "Por favor, informe o nível do curso.")]
        public required NivelEnum Nivel { get; set; }

        [Required(ErrorMessage = "Por favor, informe a data de início do curso.")]
        public required DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Por favor, informe a carga horária do curso.")]
        [Range(10, 50, ErrorMessage = "A carga horária deve estar entre 10 e 50 horas.")]
        public required int CargaHoraria { get; set; }

        [Required(ErrorMessage = "Por favor, informe o número máximo de alunos para o curso.")]
        [Range(10, 30, ErrorMessage = "O número máximo de alunos deve estar entre 10 e 30.")]
        public required int MaxAlunos { get; set; }

        public int QuantidadeAlunos { get; set; } = 0;

        public StatusCursoEnum Status { get; set; } = StatusCursoEnum.Ativo;

        public DateTime? DataAtualizacao { get; set; }

        public int? ProfessorId { get; set; }

        public virtual ProfessorModel? Professor { get; set; }

        public virtual IList<AlunoCursoModel> AlunoCurso { get; set; } = new List<AlunoCursoModel>();
    }
}
