using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface IAlunoRepositorio
    {
        Task<List<AlunoModel>> BuscarTodosAlunosAsync();

        Task<AlunoModel> BuscarAlunoPorIdAsync(int id);

        Task<AlunoModel> BuscarAlunoExistenteAsync(string nome);

        Task AddAlunoAsync(AlunoModel aluno);

        Task AtualizarAlunoAsync(AlunoModel aluno);

        Task<bool> RemoverAlunoAsync(int id);

        int TotalAlunos();
    }
}