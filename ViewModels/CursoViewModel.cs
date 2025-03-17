using MasterIdiomas.Models;

namespace MasterIdiomas.ViewModels
{
    public class CursoViewModel
    {
        public CursoModel? Curso { get; set; }
        public ProfessorModel? Professor { get; set; }

        public List<string> Idiomas { get; set; } = new List<string>();

        public List<CursoModel> Cursos { get; set; } = new List<CursoModel>();

        public List<ProfessorModel> Professores { get; set; } = new List<ProfessorModel>();

        public List<AlunoModel> AlunosDoCurso { get; set; } = new List<AlunoModel>();
        public List<AlunoModel> AlunosNaoInscritosNoCurso { get; set; } = new List<AlunoModel>();

    }
}
