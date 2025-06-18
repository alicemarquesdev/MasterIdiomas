using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas aos alunos
    public interface IAlunoRepositorio
    {
        // Busca um aluno específico com base no seu ID
        Task<AlunoModel?> BuscarAlunoPorIdAsync(int id);

        // Busca todos os alunos cadastrados no sistema
        Task<List<AlunoModel>> BuscarTodosAlunosAsync();

        // Verifica se já existe um aluno com o mesmo nome e data de nascimento
        Task<AlunoModel?> VerificarAlunoExistenteAsync(string nome, DateTime dataDascimento);

        // Adiciona um novo aluno ao sistema
        Task AddAlunoAsync(AlunoModel aluno);

        // Atualiza os dados de um aluno existente no sistema
        Task AtualizarAlunoAsync(AlunoModel aluno);

        // Remove um aluno do sistema com base no seu ID
        Task<bool> RemoverAlunoAsync(int id);

        // Retorna o número total de alunos cadastrados no sistema
        int TotalAlunos();
    }
}
