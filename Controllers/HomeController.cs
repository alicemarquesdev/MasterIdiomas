using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using MasterIdiomas.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // HomeController é responsável por gerenciar as ações relacionadas à página inicial do sistema.
    // Ele fornece métodos GET para exibição de informações gerais e para pesquisa de cursos.

    // Métodos:

    // GET Index() - Exibe a página inicial do sistema, mostrando informações do usuário logado, como nome,
    // total de alunos, professores, idiomas e cursos. Caso o usuário não esteja logado, redireciona para a página de login.

    // GET BarraDePesquisa(string termo) - Realiza uma busca de cursos com base no termo digitado na barra de pesquisa.
    // Retorna os resultados da busca em formato JSON, contendo o id do curso e seu nome, que é uma concatenação de idioma,
    // turno e nível. Caso ocorra um erro na pesquisa, registra o erro no log e exibe uma mensagem genérica.

    [UsuarioLogado] // Garante que o usuário esteja logado para acessar
    public class HomeController : Controller
    {
        // Dependências injetadas no controlador
        private readonly ISessao _sessao;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly ILogger<HomeController> _logger;

        // Construtor que recebe as dependências
        public HomeController(ISessao sessao,
                              ICursoRepositorio cursoRepositorio,
                              IProfessorRepositorio professorRepositorio,
                              IAlunoRepositorio alunoRepositorio,
                              ILogger<HomeController> logger)
        {
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _cursoRepositorio = cursoRepositorio ?? throw new ArgumentNullException(nameof(cursoRepositorio));
            _professorRepositorio = professorRepositorio ?? throw new ArgumentNullException(nameof(professorRepositorio));
            _alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        // Método para exibir a página inicial
        public async Task<IActionResult> Index()
        {
            try
            {
                // Recupera as informações do usuário da sessão
                var usuario = _sessao.BuscarSessaoUsuario();

                // Se o usuário não estiver logado, redireciona para a página de login
                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de acesso à página inicial sem estar logado.");
                    return RedirectToAction("Login", "Login");
                }

                List<CursoModel> cursos = await _cursoRepositorio.BuscarTodosCursosAsync();

                var viewModel = new HomeViewModel
                {
                    UsuarioNome = usuario.Nome,
                    Cursos = cursos,
                    TotalAlunos = _alunoRepositorio.TotalAlunos(),
                    TotalProfessores = _professorRepositorio.TotalProfessores(),
                    TotalIdiomas = _cursoRepositorio.TotalIdiomas(),
                    TotalCursos = _cursoRepositorio.TotalCursos(),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Loga o erro e exibe uma mensagem genérica
                _logger.LogError(ex, "Erro ao carregar informações na página inicial");
                TempData["MensagemErro"] = "Ocorreu um erro ao carregar as informações. Tente novamente mais tarde.";
                return View("Error");
            }
        }

        // Exibe a página de erro
        public IActionResult Error()
        {
            return View();
        }

        // Método para buscar cursos na barra de pesquisa
        [HttpGet]
        public async Task<IActionResult> BarraDePesquisa(string termo)
        {
            try
            {
                if (string.IsNullOrEmpty(termo))
                {
                    return Json(new List<object>());
                }

                // Buscar cursos com base no termo de pesquisa
                var cursos = await _cursoRepositorio.BuscarCursosBarraDePesquisaAsync(termo);

                if (!cursos.Any()) // Verifica se algum curso foi encontrado
                {
                    TempData["MensagemErro"] = "Nenhum curso encontrado com o termo informado.";
                }

                // Retornar os cursos encontrados como um JSON
                var resultados = cursos.Select(c => new
                {
                    id = c.CursoId,
                    nome = $"{c.Idioma} - {c.Turno} ({c.Nivel})"  // Ajuste o nome conforme preferir
                }).ToList();

                return Json(resultados);
            }
            catch (Exception ex)
            {
                // Loga o erro e exibe uma mensagem genérica
                _logger.LogError(ex, "Erro ao carregar resultados da barra de pesquisa");
                TempData["MensagemErro"] = "Erro ao realizar a pesquisa, tente novamente.";
                return Json(new List<object>());
            }
        }

    }
}