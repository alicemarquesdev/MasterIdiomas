using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface IProfessorRepositorio
    {
        Task<List<ProfessorModel>> BuscarTodosProfessoresAsync();

        Task<ProfessorModel> BuscarProfessorExistenteAsync(string email, string nome, int professorIdIgnorar);

        Task<ProfessorModel> BuscarProfessorPorIdAsync(int id);

        Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId);

        Task AddProfessorAsync(ProfessorModel professor);

        Task AtualizarProfessorAsync(ProfessorModel professor);

        Task<bool> RemoverProfessorAsync(int id);

        int TotalProfessores();
    }
}