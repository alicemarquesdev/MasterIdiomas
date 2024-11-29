using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface ICursoRepositorio
    {
        Task<CursoModel> BuscarCursoPorIdAsync(int id);

        Task<List<CursoModel>> BuscarIdiomasAsync();

        Task<List<CursoModel>> BuscarTodosCursosAsync();

        Task<CursoModel> BuscarCursoExistenteAsync(string idioma, string turno, string nivel, int cursoIdIgnorar);

        Task AddCursoAsync(CursoModel curso);

        Task AtualizarCursoAsync(CursoModel curso);

        Task<bool> RemoverCursoAsync(int id);

        int CursosEmAndamento();

        int TotalCursos();

        int TotalIdiomas();
    }
}