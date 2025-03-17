using MasterIdiomas.Filters;
using MasterIdiomas.Repositorio;
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

        // Método para adicionar um professor a um curso
        [HttpPost]
        public async Task<IActionResult> AddProfessorAoCurso(int professorId, int cursoId)
        {
            try
            {
                // Validação para garantir que os IDs não sejam nulos ou vazios
                if (professorId <= 0 || cursoId <= 0)
                {
                    throw new ArgumentException("Os IDs do professor e do curso devem ser maiores que zero.");
                }

                // Buscar professor e curso no banco de dados
                var professor = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                // Verificar se o curso e o professor existem
                if (curso == null || professor == null)
                {
                    throw new Exception("Professor/Curso não encontrados no banco de dados");
                }

                // Verificar se o curso já está associado a outro professor
                if (curso.ProfessorId != null)
                {
                    throw new Exception($"O curso '{curso.Idioma}' já está associado ao professor '{curso.Professor?.Nome ?? "desconhecido"}'.");
                }

                // Adicionar professor ao curso
                await _professorCursoRepositorio.AddProfessorAoCursoAsync(curso, professor);
                TempData["MensagemSucesso"] = $"O professor foi adicionado ao curso com sucesso.";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                // Log de erro e mensagem de erro na tela
                _logger.LogError(ex, "Erro ao tentar adicionar professor ao curso.");
                TempData["MensagemErro"] = "Erro ao tentar adicionar professor ao curso.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        // Método para remover um professor de um curso
        [HttpPost]
        public async Task<IActionResult> RemoverProfessorDoCurso(int professorId, int cursoId)
        {
            try
            {
                // Validação para garantir que os IDs não sejam nulos ou vazios
                if (professorId <= 0 || cursoId <= 0)
                {
                    throw new ArgumentException("Os IDs do professor e do curso devem ser maiores que zero.");
                }

                // Verificar se o professor e o curso existem
                var professor = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                if (professor == null || curso == null)
                {
                    throw new Exception("Professor/Curso não encontrados no banco de dados.");
                }

                // Remover professor do curso
                await _professorCursoRepositorio.RemoverProfessorDoCursoAsync(curso, professorId);
                TempData["MensagemSucesso"] = "Professor removido do curso com sucesso!";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                // Log de erro e mensagem de erro na tela
                TempData["MensagemErro"] = $"Erro ao remover professor do curso.";
                _logger.LogError(ex, "Erro ao tentar remover professor do curso.");
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
