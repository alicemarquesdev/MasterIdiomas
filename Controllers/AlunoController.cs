using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using MasterIdiomas.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // AlunoController responsável por gerenciar as ações relacionadas aos alunos no sistema.
    // Ele fornece métodos GET para exibição de informações e métodos POST para manipulação de dados.

    // Métodos:

    // GET Alunos() - Retorna a view com a lista de todos os alunos cadastrados no sistema.
    // GET DetalhesAluno(int id) - Retorna a view com os detalhes de um aluno específico,
    // incluindo os cursos em que ele está matriculado e os cursos disponíveis para inscrição.

    // POST AddAluno(AlunoModel aluno) - Adiciona um novo aluno ao banco de dados caso os dados sejam válidos.
    // POST AtualizarAluno(AlunoModel aluno) - Atualiza as informações de um aluno existente no banco de dados.
    // POST RemoverAluno(int id) - Remove um aluno do banco de dados com base no ID fornecido.

    [UsuarioLogado] // Garante que o usuário esteja logado para acessar
    public class AlunoController : Controller
    {
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly ILogger<AlunoController> _logger;

        // Construtor que injeta as dependências e verifica se são nulas
        public AlunoController(IAlunoRepositorio alunoRepositorio,
                                IAlunoCursoRepositorio alunoCursoRepositorio,
                                ILogger<AlunoController> logger)
        {
            _alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
            _alunoCursoRepositorio = alunoCursoRepositorio ?? throw new ArgumentNullException(nameof(alunoCursoRepositorio));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método para listar todos os alunos
        public async Task<ActionResult> Alunos()
        {
            try
            {
                var viewModel = new AlunoViewModel
                {
                    Alunos = await _alunoRepositorio.BuscarTodosAlunosAsync() ?? throw new ArgumentNullException("Lista de alunos retornou null"),
                    Aluno = new AlunoModel
                    {
                        Nome = string.Empty,
                        Genero = Enums.GeneroEnum.Outro,
                        DataNascimento = new DateTime(1950, 1, 1)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir a view Alunos.");
                TempData["MensagemErro"] = "Houve um erro ao exibir a página de Alunos, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para exibir os detalhes de um aluno
        public async Task<ActionResult> DetalhesAluno(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentNullException(nameof(id), "O id é inválido");
                }

                var aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id)
                    ?? throw new ArgumentNullException("Aluno não encontrado!");

                var viewModel = new AlunoViewModel
                {
                    Aluno = aluno,
                    CursosDoAluno = await _alunoCursoRepositorio.BuscarCursosDoAlunoAsync(aluno.AlunoId)
                        ?? throw new ArgumentNullException("Lista de cursos do aluno retornou null"),
                    CursosDisponiveisParaOAluno = await _alunoCursoRepositorio.BuscarCursosAlunoNaoEstaInscritoAsync(aluno.AlunoId)
                        ?? throw new ArgumentNullException("Lista de cursos disponíveis retornou null")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir a view DetalhesAluno.");
                TempData["MensagemErro"] = "Houve um erro ao exibir a página, tente novamente.";
                return RedirectToAction("Alunos");
            }
        }

        // Método para adicionar um novo aluno
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<ActionResult> AddAluno(AlunoModel aluno)
        {
            try
            {
                if (aluno == null)
                {
                    throw new ArgumentNullException(nameof(aluno), "Aluno é nulo");
                }

                if (ModelState.IsValid)
                {
                    await _alunoRepositorio.AddAlunoAsync(aluno);
                    TempData["MensagemSucesso"] = "O Aluno foi criado com sucesso!";
                }
                return RedirectToAction("Alunos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar aluno.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao adicionar aluno, tente novamente.";
                }

                return RedirectToAction("Alunos");
            }
        }

        // Método para atualizar os dados de um aluno
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<ActionResult> AtualizarAluno(AlunoModel aluno)
        {
            try
            {
                if (aluno == null)
                {
                    throw new ArgumentNullException(nameof(aluno), "Aluno é nulo");
                }

                if (ModelState.IsValid)
                {
                    await _alunoRepositorio.AtualizarAlunoAsync(aluno);
                    TempData["MensagemSucesso"] = "Os dados do aluno foram atualizados com sucesso!";
                }
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar os dados do aluno, tente novamente.";
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }

        }

        // Método para remover um aluno
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<ActionResult> RemoverAluno(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentNullException(nameof(id), "O id é inválido");
                }

               await _alunoRepositorio.RemoverAlunoAsync(id);

                TempData["MensagemSucesso"] = "O aluno foi removido com sucesso!";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover aluno.");
                TempData["MensagemErro"] = "Erro ao remover o aluno, tente novamente.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
