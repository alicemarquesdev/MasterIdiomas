using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas a professores
    public interface IProfessorRepositorio
    {
        // Retorna um professor específico baseado no seu ID
        Task<ProfessorModel?> BuscarProfessorPorIdAsync(int id);

        // Retorna uma lista de todos os professores cadastrados
        Task<List<ProfessorModel>> BuscarTodosProfessoresAsync();

        // Retorna uma lista de todos os professores que não tenha chegado ao limite de cursos 
        Task<List<ProfessorModel>> BuscarProfessorSemCursoAsync();

        // Verifica se um professor já existe no sistema com base no e-mail e nome
        Task<bool> VerificarProfessorExistenteAsync(string email, string nome);

        // Adiciona um novo professor ao sistema
        Task AddProfessorAsync(ProfessorModel professor);

        // Atualiza as informações de um professor existente
        Task AtualizarProfessorAsync(ProfessorModel professor);

        // Remove um professor do sistema com base no seu ID
        Task<bool> RemoverProfessorAsync(int id);

        // Retorna o total de professores cadastrados no sistema
        int TotalProfessores();
    }
}
