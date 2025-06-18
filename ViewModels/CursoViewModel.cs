using MasterIdiomas.Models;

namespace MasterIdiomas.ViewModels
{
    // A classe CursoViewModel é usada para representar os dados necessários
    // para exibir e manipular informações relacionadas aos cursos, professores, alunos, etc.
    public class CursoViewModel
    {
        // Representa um curso específico que está sendo visualizado ou editado
        public CursoModel? Curso { get; set; }

        // Representa o professor que leciona o curso
        public ProfessorModel? Professor { get; set; }

        // Lista de idiomas disponíveis para o curso (por exemplo: Português, Inglês, Espanhol)
        public List<string> Idiomas { get; set; } = new List<string>();

        // Lista de todos os cursos cadastrados no sistema
        public List<CursoModel> Cursos { get; set; } = new List<CursoModel>();

        // Lista de todos os professores cadastrados no sistema
        public List<ProfessorModel> Professores { get; set; } = new List<ProfessorModel>();

        // Lista de alunos que estão matriculados no curso
        public List<AlunoModel> AlunosDoCurso { get; set; } = new List<AlunoModel>();

        // Lista de alunos que ainda não estão matriculados no curso, mas que podem ser inscritos
        public List<AlunoModel> AlunosNaoInscritosNoCurso { get; set; } = new List<AlunoModel>();
    }
}
