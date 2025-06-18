using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using MasterIdiomas.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // Controller é responsável por gerenciar as ações relacionadas aos professores, como exibir a lista de professores, 
    // detalhes de um professor, adicionar, editar, remover e atualizar os dados de professores.

    // Listagem de métodos:
    // Professores - Exibe todos os professores.
    // DetalhesProfessor - Exibe os cursos de um professor.
    // (POST) AddProfessor - Adicionar um novo professor.
    // (POST) AtualizarProfessor - Atualiza os dados do professor.
    // (POST) RemoverProfessor - Confirma a remoção de um professor.

    [UsuarioLogado] // Filtro que garante que o usuário precisa estar logado
    public class ProfessorController : Controller
    {
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorCursoRepositorio _professorCursoRepositorio;
        private readonly ILogger<ProfessorController> _logger; // Logger para capturar erros

        // Construtor do Controller. Lança exceção caso alguma dependência não seja passada.
        public ProfessorController(IProfessorRepositorio professorRepositorio,
                                    ICursoRepositorio cursoRepositorio,
                                    IProfessorCursoRepositorio professorCursoRepositorio,
                                    ILogger<ProfessorController> logger)
        {
            _professorRepositorio = professorRepositorio ?? throw new ArgumentNullException(nameof(professorRepositorio));
            _cursoRepositorio = cursoRepositorio ?? throw new ArgumentNullException(nameof(cursoRepositorio));
            _professorCursoRepositorio = professorCursoRepositorio ?? throw new ArgumentNullException(nameof(professorCursoRepositorio));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método para exibir todos os professores
        public async Task<IActionResult> Professores()
        {
            try
            {
                List<ProfessorModel> professores = await _professorRepositorio.BuscarTodosProfessoresAsync();

                var professor = new ProfessorModel
                {
                    Nome = string.Empty,
                    Genero = Enums.GeneroEnum.Outro,
                    DataNascimento = new DateTime(1950, 1, 1),
                    Email = string.Empty
                };

                var viewModel = new ProfessorViewModel
                {
                    Professor = professor,
                    Professores = professores,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a lista de professores.");
                TempData["MensagemErro"] = "Erro ao carregar a página de Professores, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para exibir os cursos de um professor
        public async Task<IActionResult> DetalhesProfessor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentNullException("Id é nulo");
                }

                ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);

                if (professor == null)
                {
                    TempData["MensagemErro"] = "Professor não encontrado!";
                    return RedirectToAction("Index", "Home");
                }

                var viewModel = new ProfessorViewModel
                {
                    Professor = professor,
                    CursosDoProfessor = await _professorCursoRepositorio.BuscarCursosDoProfessorAsync(id) ?? throw new ArgumentNullException("Lista de cursos do professor retornou null"),
                    CursosProfessorNaoInscrito = await _professorCursoRepositorio.BuscarCursosProfessorNaoInscritoAsync(id) ?? throw new ArgumentNullException("Lista de cursos do professor não inscrito retornou null")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar cursos do professor.");
                TempData["MensagemErro"] = "Erro ao carregar página com dados do Professor";
                return RedirectToAction("Index", "Home");
            }
        }
        // Método POST para adicionar um novo professor
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AddProfessor(ProfessorModel professor)
        {
            try
            {
                if (professor ==  null)
                {
                    throw new ArgumentNullException("Professor é nulo");
                }

                if (ModelState.IsValid)
                {
                    await _professorRepositorio.AddProfessorAsync(professor);
                    TempData["MensagemSucesso"] = "Professor criado com sucesso!";
                    return RedirectToAction("Professores");
                }

                TempData["MensagemErro"] = "Dados inválidos, por favor verifique e tente novamente.";
                return RedirectToAction("Professores");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar professor.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao adicionar professor, tente novamente.";
                }

                return RedirectToAction("Professores");
            }
        }

        // Método POST para atualizar os dados do professor
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AtualizarProfessor(ProfessorModel professor)
        {
            try
            {
                if (professor == null)
                {
                    throw new ArgumentNullException("Professor é nulo");
                }
              
                if (ModelState.IsValid)
                {
                    await _professorRepositorio.AtualizarProfessorAsync(professor);
                    if (professor.Status == Enums.StatusEnum.Inativo)
                    {
                        List<CursoModel> cursosDoProfessor = await _professorCursoRepositorio.BuscarCursosDoProfessorAsync(professor.ProfessorId);

                        foreach (var curso in cursosDoProfessor)
                        {
                            await _professorCursoRepositorio.RemoverProfessorDoCursoAsync(curso);
                        }
                        TempData["MensagemSucesso"] = "Professor atualizado com sucesso! O status foi alterado para Inativo.";
                    }
                    else
                    {
                        TempData["MensagemSucesso"] = "Professor atualizado com sucesso!";
                    }
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                TempData["MensagemErro"] = $"Verifique os dados inseridos, erro ao tentar atualizar dados.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar professor.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar professor, tente novamente.";
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        // Método POST para remover um professor
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> RemoverProfessor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentNullException("Id é nulo");
                }

                List<CursoModel> cursos = await _professorCursoRepositorio.BuscarCursosDoProfessorAsync(id);

                if (cursos != null && cursos.Any())
                {
                    throw new InvalidOperationException("O professor está associado a cursos e não pode ser removido.");
                }

                await _professorRepositorio.RemoverProfessorAsync(id);
                TempData["MensagemSucesso"] = "Professor removido com sucesso!";
                return RedirectToAction("Professores");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover professor.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao remover professor, tente novamente.";
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
