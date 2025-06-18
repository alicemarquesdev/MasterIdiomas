using MasterIdiomas.Models;

namespace MasterIdiomas.ViewModels
{
    // ViewModel que contém os dados necessários para a gestão de alunos na aplicação
    public class AlunoViewModel
    {
        // Propriedade para armazenar as informações de um aluno específico, podendo ser utilizado em casos de edição ou visualização detalhada
        public AlunoModel? Aluno { get; set; }

        // Lista de alunos para exibição ou manipulação em massa, como exibir todos os alunos cadastrados no sistema
        public List<AlunoModel> Alunos { get; set; } = new List<AlunoModel>();

        // Lista de cursos aos quais um aluno está inscrito. Essa lista pode ser usada para exibir os cursos nos quais o aluno está participando
        public List<CursoModel> CursosDoAluno { get; set; } = new List<CursoModel>();

        // Lista de cursos que estão disponíveis para o aluno se inscrever, geralmente pode ser usada para sugerir cursos ao aluno
        public List<CursoModel> CursosDisponiveisParaOAluno { get; set; } = new List<CursoModel>();
    }
}
