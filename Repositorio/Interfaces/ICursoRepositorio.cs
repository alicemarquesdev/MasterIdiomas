using MasterIdiomas.Enums;
using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas aos cursos
    public interface ICursoRepositorio
    {
        // Retorna uma lista de idiomas disponíveis nos cursos
        Task<List<string>> BuscarIdiomasAsync();

        // Busca um curso específico pelo seu ID
        Task<CursoModel?> BuscarCursoPorIdAsync(int id);

        // Busca todos os cursos disponíveis para um idioma específico
        Task<List<CursoModel>> BuscarCursosPorIdiomaAsync(string idioma);

        // Retorna uma lista com todos os cursos cadastrados no sistema
        Task<List<CursoModel>> BuscarTodosCursosAsync();

        // Realiza uma busca por cursos utilizando um termo de pesquisa
        Task<List<CursoModel>> BuscarCursosBarraDePesquisaAsync(string termo);

        // Verifica se já existe um curso com o mesmo idioma, turno e nível
        Task<bool> VerificarCursoExistenteAsync(string idioma, TurnoEnum turno, NivelEnum nivel, int cursoId);

        // Adiciona um novo curso ao sistema
        Task AddCursoAsync(CursoModel curso);

        // Atualiza os dados de um curso existente
        Task AtualizarCursoAsync(CursoModel curso);

        // Remove um curso do sistema com base no seu ID
        Task<bool> RemoverCursoAsync(int id);

        // Retorna o número de cursos em andamento no sistema
        int CursosEmAndamento();

        // Retorna o número total de cursos cadastrados no sistema
        int TotalCursos();

        // Retorna o número de idiomas disponíveis nos cursos cadastrados
        int TotalIdiomas();
    }
}
