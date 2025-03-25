using MasterIdiomas.Filters;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // AlunoCursoController é responsável por gerenciar a associação entre alunos e cursos no sistema.
    // Ele oferece métodos para adicionar ou remover um aluno de um curso.

    // Métodos:

    // POST AddAlunoAoCurso(int alunoId, int cursoId) - Adiciona um aluno a um curso,
    // verificando se os IDs são válidos e realizando a operação através do repositório.

    // POST RemoverAlunoDoCurso(int alunoId, int cursoId) - Remove um aluno de um curso,
    // verificando se os IDs são válidos e realizando a operação através do repositório.

    [UsuarioLogado] // Garante que o usuário esteja logado para acessar
    public class AlunoCursoController : Controller
    {
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly ILogger<AlunoCursoController> _logger;

        // Construtor que injeta as dependências necessárias
        public AlunoCursoController(IAlunoCursoRepositorio alunoCursoRepositorio,
                                    ICursoRepositorio cursoRepositorio,
                                    IAlunoRepositorio alunoRepositorio,
                                    ILogger<AlunoCursoController> logger)
        {
            // Inicializa as dependências com a injeção de dependência fornecida ao controlador
            _alunoCursoRepositorio = alunoCursoRepositorio ?? throw new ArgumentNullException(nameof(alunoCursoRepositorio));
            _cursoRepositorio = cursoRepositorio ?? throw new ArgumentNullException(nameof(cursoRepositorio)); // Verifica se o repositório de Curso foi passado
            _alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio)); // Verifica se o repositório de Professor foi passado
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método para adicionar um aluno a um curso, usando fetch
        [HttpPost]
        public async Task<IActionResult> AddAlunoAoCurso(int alunoId, int cursoId)
        {
            try
            {
                // Valida os IDs fornecidos
                if (alunoId <= 0 || cursoId <= 0)
                {
                    return BadRequest(new { mensagem = "Id de aluno/curso inválido." });
                }
                // Verifica se o aluno e o curso existem no banco de dados
                var alunoDb = await _alunoRepositorio.BuscarAlunoPorIdAsync(alunoId);
                var cursoDb = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                if (alunoDb == null)
                {
                    return NotFound(new { mensagem = $"Aluno com ID {alunoId} não encontrado." });
                }

                if (cursoDb == null)
                {
                    return NotFound(new { mensagem = $"Curso com ID {cursoId} não encontrado." });
                }

                // Adiciona o aluno ao curso
                await _alunoCursoRepositorio.AddAlunoAoCursoAsync(alunoDb, cursoDb);

                return Ok(new { mensagem = "Aluno adicionado ao curso com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover aluno do curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao adicionar aluno do curso, tente novamente.";
                }

                return StatusCode(500, new { mensagem = "Erro interno no servidor." });
            }
        }        

        // Método para remover um aluno de um curso, usando fetch
        [HttpPost]
        public async Task<IActionResult> RemoverAlunoDoCurso(int alunoId, int cursoId)
        {
            try
            {
                // Valida os IDs fornecidos
                if (alunoId <= 0 || cursoId <= 0)
                {
                    return BadRequest(new { mensagem = "Id de aluno/curso inválido." });
                }

                // Remove o aluno do curso
                await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(alunoId, cursoId);

                return Ok(new { mensagem = "Aluno removido do curso com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover aluno do curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao remover aluno do curso, tente novamente.";
                }

                return StatusCode(500, new { mensagem = "Erro interno no servidor." });
            }
        }
    }
}
