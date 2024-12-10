using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface IProfessorRepositorio
    {
        Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId);

        Task<List<ProfessorModel>> BuscarTodosProfessoresAsync();

        Task<ProfessorModel> BuscarProfessorExistenteAsync(string email, string nome, int professorIdIgnorar);

        Task<ProfessorModel> BuscarProfessorPorIdAsync(int id);

        Task AddProfessorAsync(ProfessorModel professor);

        Task AtualizarProfessorAsync(ProfessorModel professor);

        Task AddProfessorAoCursoAsync(int professorId, int cursoId);

        Task RemoverProfessorDoCursoAsync(int professorId, int cursoId);

        Task<bool> RemoverProfessorAsync(int id);

        int TotalProfessores();
    }
}