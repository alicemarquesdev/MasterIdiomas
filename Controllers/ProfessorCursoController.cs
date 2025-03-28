using MasterIdiomas.Filters;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // Controller gerencia a associação entre professores e cursos. Ele oferece funcionalidades para adicionar e remover professores aos cursos.
    // Os métodos implementados são:
    // - AddProfessorAoCurso: Adiciona um professor a um curso.
    // - RemoverProfessorDoCurso: Remove um professor de um curso.

    [UsuarioLogado] // Filtro que garante que o usuário precisa estar logado
    public class ProfessorCursoController : Controller
    {
        private readonly IProfessorCursoRepositorio _professorCursoRepositorio;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly ILogger<ProfessorCursoController> _logger; // Logger para registrar as ações e erros

        // Construtor do Controller, com injeção de dependência dos repositórios e do logger.
        public ProfessorCursoController(IProfessorCursoRepositorio professorCursoRepositorio,
            ICursoRepositorio cursoRepositorio,
            IProfessorRepositorio professorRepositorio,
            ILogger<ProfessorCursoController> logger)
        {
            // Inicializa as dependências com a injeção de dependência fornecida ao controlador
            _cursoRepositorio = cursoRepositorio ?? throw new ArgumentNullException(nameof(cursoRepositorio)); // Verifica se o repositório de Curso foi passado
            _professorRepositorio = professorRepositorio ?? throw new ArgumentNullException(nameof(professorRepositorio)); // Verifica se o repositório de Professor foi passado
            _professorCursoRepositorio = professorCursoRepositorio ?? throw new ArgumentNullException(nameof(professorCursoRepositorio)); // Verifica se o repositório de ProfessorCurso foi passado
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Verifica se o logger foi passado
        }

        // Método para adicionar um professor a um curso, usando fetch
        [HttpPost]
        public async Task<IActionResult> AddProfessorAoCurso(int professorId, int cursoId)
        {
            try
            {
                // Validação para garantir que os IDs não sejam nulos ou vazios
                if (professorId <= 0 || cursoId <= 0)
                {
                    return BadRequest(new { mensagem = "Id de professor/curso inválido." });
                }

                // Buscar professor e curso no banco de dados
                var professor = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                // Verificar se o curso e o professor existem
                if (curso == null || professor == null)
                {
                    return NotFound(new { mensagem = "Professor/Curso não encontrado." });
                }

                if(professor.Status == Enums.StatusEnum.Inativo)
                {
                    TempData["MensagemErro"] = "O status do professor é inativo. Não poderá ser adicionado ao curso.";
                    return BadRequest(new { mensagem = "O status do professor é inativo. Não poderá ser adicionado ao curso." });
                }

                // Verificar se o curso já está associado a outro professor
                if (curso.ProfessorId != null)
                {
                    TempData["MensagemErro"] = $"O curso '{curso.Idioma}' já está associado ao professor '{curso.Professor?.Nome}'.";
                    return BadRequest(new { mensagem = $"O curso '{curso.Idioma}' já está associado ao professor '{curso.Professor?.Nome}'." });
                }

                // Adicionar professor ao curso
                await _professorCursoRepositorio.AddProfessorAoCursoAsync(curso, professor);

                return Ok(new { mensagem = "Professor adicionado ao curso com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar adicionar professor ao curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar adicionar professor ao curso, tente novamente.";
                }

                return StatusCode(500, new { mensagem = "Erro interno no servidor." });
            }
        }

        // Método para remover um professor de um curso, usando fetch
        [HttpPost]
        public async Task<IActionResult> RemoverProfessorDoCurso(int professorId, int cursoId)
        {
            try
            {
                // Validação para garantir que os IDs não sejam nulos ou vazios
                if (professorId <= 0 || cursoId <= 0)
                {
                    return BadRequest(new { mensagem = "Id de professor/curso inválido." });
                }

                // Verificar se o professor e o curso existem
                var professor = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                if (professor == null || curso == null)
                {
                    return NotFound(new { mensagem = $"Professor/Curso não encontrado." });
                }

                // Remover professor do curso
                await _professorCursoRepositorio.RemoverProfessorDoCursoAsync(curso);

                return Ok(new { mensagem = "Professor removido do curso com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar remover professor ao curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar remover professor ao curso, tente novamente.";
                }

                return StatusCode(500, new { mensagem = "Erro interno no servidor." });
            }
        }
    }
}
