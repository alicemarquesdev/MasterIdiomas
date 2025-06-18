using MasterIdiomas.Enums;
using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using MasterIdiomas.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // CursoController responsável por gerenciar as ações relacionadas aos cursos no sistema.
    // Ele fornece métodos GET para exibição de informações e métodos POST para manipulação de dados.

    // Métodos:

    // GET Cursos() - Retorna a view com a lista de todos os cursos cadastrados no sistema.
    // GET DetalhesCurso(int id) - Retorna a view com os detalhes de um curso específico,
    // incluindo os alunos matriculados e os alunos disponíveis para inscrição.
    // GET Idioma(string idioma) - Retorna a view com cursos de um idioma passado como parametro.

    // POST AddCurso(CursoModel curso) - Adiciona um novo curso ao banco de dadosk caso os dados sejam válidos.
    // POST AtualizarCurso(CursoModel curso) - Atualiza as informações de um curso existente no banco de dados.
    // POST RemoverCurso(int id) - Remove um curso do banco de dados com base no ID fornecido.

    [UsuarioLogado] // Garante que o usuário esteja logado para acessar
    public class CursoController : Controller
    {
        // Declaração de dependências
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IdiomasSettings _idiomasSettings;
        private readonly ILogger<CursoController> _logger;

        // Construtor que injeta as dependências necessárias
        public CursoController(ICursoRepositorio cursoRepositorio,
                              IAlunoCursoRepositorio alunoCursoRepositorio,
                              IProfessorRepositorio professorRepositorio,
                              IdiomasSettings idiomasSettings,
                              ILogger<CursoController> logger)
        {
            _cursoRepositorio = cursoRepositorio ?? throw new ArgumentNullException(nameof(cursoRepositorio));
            _alunoCursoRepositorio = alunoCursoRepositorio ?? throw new ArgumentNullException(nameof(alunoCursoRepositorio));
            _professorRepositorio = professorRepositorio ?? throw new ArgumentNullException(nameof(professorRepositorio));
            _idiomasSettings = idiomasSettings ?? throw new ArgumentNullException(nameof(idiomasSettings)); // Garantindo que IdiomasSettings não seja nulo
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método para exibir a lista de cursos
        public async Task<IActionResult> Cursos()
        {
            try
            {
                // Criando um modelo de curso para exibição inicial na view
                var curso = new CursoModel
                {
                    Idioma = "Alemão",
                    Turno = TurnoEnum.Manha,
                    Nivel = NivelEnum.Iniciante,
                    DataInicio = DateTime.UtcNow,
                    CargaHoraria = 0,
                    MaxAlunos = 0,
                };

                // Preenchendo o ViewModel com cursos e professores para a view
                var viewModel = new CursoViewModel
                {
                    Curso = curso,
                    Idiomas = _idiomasSettings.Idiomas, // Passando a lista de idiomas para o ViewModel
                    Cursos = await _cursoRepositorio.BuscarTodosCursosAsync()
                    ?? throw new ArgumentNullException("Lista de cursos retornou null"),
                    Professores = await _professorRepositorio.BuscarProfessorSemCursoAsync()
                    ?? throw new ArgumentNullException("Lista de professores retornou null")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log de erro em caso de falha no carregamento
                _logger.LogError(ex, "Erro ao carregar view Cursos");
                TempData["MensagemErro"] = "Erro ao carregar página de Cursos, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para exibir alunos de um curso específico
        public async Task<IActionResult> DetalhesCurso(int id)
        {
            try
            {
                // Verifica se o id do curso é válido
                if (id <= 0)
                {
                    throw new Exception("Id é null");
                }

                // Buscar o curso pelo id
                CursoModel curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);
                if (curso == null)
                {
                    throw new Exception("Curso não encontrado");
                }

                // Preparando o ViewModel com detalhes do curso, alunos e professores
                var viewModel = new CursoViewModel
                {
                    Curso = curso,
                    Idiomas = _idiomasSettings.Idiomas, // Passando a lista de idiomas para o ViewModel
                    AlunosDoCurso = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(curso.CursoId)
                    ?? throw new ArgumentNullException("Lista de alunos do curso retornou null"),
                    AlunosNaoInscritosNoCurso = await _alunoCursoRepositorio.BuscarAlunosNaoInscritosNoCursoAsync(curso.CursoId)
                    ?? throw new ArgumentNullException("Lista de alunos não inscritos no curso retornou null"),
                    Professores = await _professorRepositorio.BuscarProfessorSemCursoAsync()
                    ?? throw new ArgumentNullException("Lista de professores retornou null")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log de erro ao tentar carregar os detalhes do curso
                _logger.LogError(ex, "Erro ao carregar view DetalhesCurso");
                TempData["MensagemErro"] = "Erro ao carregar página de Detalhes de Curso, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para exibir alunos de um curso filtrados por idioma
        public async Task<IActionResult> Idioma(string idioma)
        {
            try
            {
                if (Idioma == null)
                {
                    throw new Exception("idioma é null");
                }


                var curso = new CursoModel
                {
                    Idioma = "Alemão",
                    Turno = TurnoEnum.Manha,
                    Nivel = NivelEnum.Iniciante,
                    DataInicio = DateTime.UtcNow,
                    CargaHoraria = 0,
                    MaxAlunos = 0,
                };

                var professor = new ProfessorModel
                {
                    Nome = string.Empty,
                    Genero = Enums.GeneroEnum.Outro,
                    DataNascimento = new DateTime(1950, 1, 1),
                    Email = string.Empty
                };

                var viewModel = new CursoViewModel
                {
                    Curso = curso,
                    Professor = professor,
                    Idiomas = _idiomasSettings.Idiomas, // Passando a lista de idiomas para o ViewModel
                    Cursos = await _cursoRepositorio.BuscarCursosPorIdiomaAsync(idioma)
                    ?? throw new ArgumentNullException("Lista de cursos por idioma retornou null"),
                    Professores = await _professorRepositorio.BuscarProfessorSemCursoAsync()
                    ?? throw new ArgumentNullException("Lista de professores retornou null")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar view Idioma");
                TempData["MensagemErro"] = "Erro ao carregar página Idioma, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para adicionar um novo curso
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AddCurso(CursoModel curso, int? professorId)
        {
            try
            {
                // Verifica se algum parâmetro obrigatório está nulo
                if (curso == null)
                {
                    throw new ArgumentNullException("ProfessorId/Curso é nulo");
                }

                // Verifica se o modelo é válido
                if (ModelState.IsValid)
                {
                    // Adiciona o novo curso ao repositório
                    await _cursoRepositorio.AddCursoAsync(curso);
                    TempData["MensagemSucesso"] = "Curso adicionado com sucesso!";
                    return RedirectToAction("Cursos");
                }

                // Se o modelo não for válido, redireciona para a página de cursos
                return RedirectToAction("Cursos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao adicionar curso, tente novamente.";
                }

                return RedirectToAction("Cursos");
            }
        }

        // Método para atualizar um curso existente
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AtualizarCurso(CursoModel curso)
        {
            try
            {
                // Verifica se o curso está nulo
                if (curso == null)
                {
                    throw new ArgumentNullException("Curso é nulo");
                }

                // Verifica se o modelo é válido
                if (ModelState.IsValid)
                {
                    // Atualiza o curso no repositório
                    await _cursoRepositorio.AtualizarCursoAsync(curso);

                    // Mensagem de sucesso conforme o status do curso
                    if (curso.Status == Enums.StatusCursoEnum.Cancelado)
                    {
                        var cursoAtualizado = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(curso.CursoId);
                        if (cursoAtualizado != null)
                        {
                            foreach (var aluno in cursoAtualizado)
                            {
                                await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(aluno.AlunoId, curso.CursoId);
                            }
                        }

                        TempData["MensagemSucesso"] = "Curso atualizado com sucesso! O status do curso foi alterado para Cancelado. Os alunos do curso foram todos removidos.";
                    }
                    else
                    {
                        TempData["MensagemSucesso"] = "Curso atualizado com sucesso!";
                    }

                    return Redirect(Request.Headers["Referer"].ToString());
                }

                // Se o modelo não for válido, exibe mensagem de erro
                TempData["MensagemErro"] = "Por favor, verifique os dados.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar curso, tente novamente.";
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        // Método para remover um curso
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> RemoverCurso(int id)
        {
            try
            {
                // Verifica se o id do curso é válido
                if (id <= 0)
                {
                    throw new ArgumentNullException("Id é nulo");
                }

                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);
                if (curso == null)
                {
                    throw new ArgumentNullException("Curso não encontrado no banco de dados");

                }

                await _cursoRepositorio.RemoverCursoAsync(id);

                TempData["MensagemSucesso"] = "Curso removido com sucesso!";
                return RedirectToAction("Cursos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover curso.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao remover curso, tente novamente.";
                }

                return RedirectToAction("Cursos");
            }
        }
    }
}