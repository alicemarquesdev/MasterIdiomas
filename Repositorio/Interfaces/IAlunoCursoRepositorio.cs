using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas à associação de alunos e cursos
    public interface IAlunoCursoRepositorio
    {
        // Verifica se existe uma associação entre o aluno e o curso na tabela AlunoCurso
        Task<AlunoCursoModel?> AlunoECursoExistentesAsync(int alunoId, int cursoId);

        // Busca a lista de cursos em que um aluno está inscrito, dado seu ID
        Task<List<CursoModel>> BuscarCursosDoAlunoAsync(int alunoId);

        // Busca a lista de cursos nos quais o aluno ainda não está inscrito
        Task<List<CursoModel>> BuscarCursosAlunoNaoEstaInscritoAsync(int alunoId);

        // Busca a lista de alunos inscritos em um determinado curso, dado seu ID
        Task<List<AlunoModel>> BuscarAlunosDoCursoAsync(int cursoId);

        // Busca alunos que não estão inscritos em um determinado curso
        Task<List<AlunoModel>> BuscarAlunosNaoInscritosNoCursoAsync(int cursoId);

        // Adiciona um aluno a um curso específico
        Task AddAlunoAoCursoAsync(AlunoModel aluno, CursoModel curso);

        // Remove um aluno de um curso específico, utilizando seus respectivos IDs
        Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId);
    }
}
