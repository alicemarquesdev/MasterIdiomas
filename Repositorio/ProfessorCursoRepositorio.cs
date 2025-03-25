using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas aos cursose professores,
    // incluindo busca de cursos associados a professor, adição e remoção de professores
    // - BuscarCursosDoProfessorAsync(int professorId) - busca todos os cursos nos quais um professor específico está inscrito
    // - BuscarCursosProfessorNaoInscritoAsync(int professorId) - busca todos os cursos onde o professor ainda não está inscrito
    // - AddProfessorAoCursoAsync(CursoModel curso, ProfessorModel professor) - adiciona um professor a um curso específico
    // - RemoverProfessorDoCursoAsync(CursoModel curso, int professorId) - remove a relação de um professor com um curso
    public class ProfessorCursoRepositorio : IProfessorCursoRepositorio
    {
        private readonly BancoContext _context;
        private readonly ILogger<ProfessorCursoRepositorio> _logger;

        // Construtor que recebe o contexto do banco
        public ProfessorCursoRepositorio(BancoContext context, ILogger<ProfessorCursoRepositorio> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Busca todos os cursos em que um professor está inscrito
        public async Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId)
        {
            try
            {
                return await _context.Cursos
                    .Include(c => c.Professor) // Inclui os dados do professor relacionado ao curso
                    .Where(c => c.ProfessorId == professorId) // Filtra cursos onde o professor está inscrito
                    .OrderBy(a => a.Idioma) // Ordena por idioma do curso
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Lança uma exceção personalizada para evitar exposição de erros internos
                _logger.LogError(ex, "Erro ao buscar os cursos do professor.");
                throw new Exception("Erro ao buscar os cursos do professor.", ex);
            }
        }

        // Busca cursos onde não tem professores
        public async Task<List<CursoModel>> BuscarCursosProfessorNaoInscritoAsync(int professorId)
        {
            try
            {
                return await _context.Cursos
                    .Where(c => c.ProfessorId == null) // Filtra cursos sem professor
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cursos sem professor.");
                throw new Exception("Erro ao buscar cursos sem professor.", ex);
            }
        }

        // Adiciona um professor a um curso
        public async Task AddProfessorAoCursoAsync(CursoModel curso, ProfessorModel professor)
        {
            try
            {
                const int MAX_CURSOS_POR_PROFESSOR = 3;

                // Verificar se o professor atingiu o limite de cursos
                var cursosDoProfessor = await BuscarCursosDoProfessorAsync(professor.ProfessorId);
                if (cursosDoProfessor.Count() >= MAX_CURSOS_POR_PROFESSOR)
                {
                    throw new InvalidOperationException($"O professor '{professor.Nome}' já está associado ao número máximo de cursos permitidos ({MAX_CURSOS_POR_PROFESSOR}).");
                }

                curso.ProfessorId = professor.ProfessorId; // Define o professor do curso
                professor.QuantidadeCursos++;

                _context.Cursos.Update(curso); // Atualiza o curso no banco de dados
                var result = await _context.SaveChangesAsync(); // Salva as alterações

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados."); // Garante que a operação foi bem-sucedida
                }
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar professor ao curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar professor ao curso.");
                throw new Exception("Erro ao adicionar professor ao curso", ex);
            }
        }

        // Remove um professor de um curso
        public async Task RemoverProfessorDoCursoAsync(CursoModel curso)
        {
            try
            {
                curso.ProfessorId = null; // Remove a relação entre professor e curso
                var professorDb = curso.Professor;

                professorDb.QuantidadeCursos = professorDb.QuantidadeCursos > 0 ? professorDb.QuantidadeCursos - 1 : 0;

                _context.Cursos.Update(curso); // Atualiza o curso no banco de dados
                var result = await _context.SaveChangesAsync(); // Salva as alterações

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados."); // Garante que a operação foi bem-sucedida
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover o professor do curso.");
                throw new Exception("Erro ao remover o professor do curso.", ex);
            }
        }
    }
}
