using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas à associação entre professores e cursos
    public interface IProfessorCursoRepositorio
    {
        // Retorna uma lista de cursos atribuídos a um professor específico
        Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId);

        // Retorna uma lista de cursos em que um professor ainda não está inscrito
        Task<List<CursoModel>> BuscarCursosProfessorNaoInscritoAsync(int professorId);

        // Associa um professor a um curso específico
        Task AddProfessorAoCursoAsync(CursoModel curso, ProfessorModel professor);

        // Remove a associação de um professor de um curso específico
        Task RemoverProfessorDoCursoAsync(CursoModel curso);
    }
}
