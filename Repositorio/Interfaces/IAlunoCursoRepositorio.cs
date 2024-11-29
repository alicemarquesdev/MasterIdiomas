using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface IAlunoCursoRepositorio
    {
        Task<List<CursoModel>> BuscarCursosDoAlunoAsync(int alunoId);

        Task<List<CursoModel>> BuscarCursosAlunoNaoInscritoAsync(int alunoId);

        Task<List<AlunoModel>> BuscarAlunosDoCursoAsync(int cursoId);

        Task<List<AlunoModel>> BuscarAlunosPorIdiomaAsync(CursoModel curso);

        Task AddAlunoAoCursoAsync(int alunoId, int cursoId);

        Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId);

        Task<int> TotalAlunosCurso(int cursoId);
    }
}