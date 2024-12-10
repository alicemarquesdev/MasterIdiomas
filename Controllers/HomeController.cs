using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class HomeController : Controller
    {
        // Dependências injetadas no controlador
        private readonly ISessao _sessao;

        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        // Construtor que recebe as dependências
        public HomeController(ISessao sessao,
                              ICursoRepositorio cursoRepositorio,
                              IProfessorRepositorio professorRepositorio,
                              IAlunoRepositorio alunoRepositorio,
                              IAlunoCursoRepositorio alunoCursoRepositorio)
        {
            _sessao = sessao;
            _cursoRepositorio = cursoRepositorio;
            _professorRepositorio = professorRepositorio;
            _alunoRepositorio = alunoRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        // Método para exibir a página inicial
        public async Task<IActionResult> Index(int cursoId)
        {
            try
            {
                // Recupera as informações do usuário da sessão
                var usuario = _sessao.BuscarSessaoUsuario();

                // Se o usuário não estiver logado, redireciona para a página de login
                if (usuario == null)
                {
                    return RedirectToAction("Index", "Usuario");
                }

                // Define o nome do usuário na view
                ViewBag.Usuario = usuario.Nome;

                // Carrega a lista de cursos do repositório
                List<CursoModel> cursos = await _cursoRepositorio.BuscarTodosCursosAsync();

                Dictionary<int, int> totalAlunosPorCurso = new();

                foreach (var curso in cursos)
                {
                    totalAlunosPorCurso[curso.CursoId] = await _alunoCursoRepositorio.TotalAlunosCurso(curso.CursoId);
                }

                ViewBag.TotalAlunosPorCurso = totalAlunosPorCurso;

                // Passa esses valores para a View
                ViewBag.TotalAlunos = _alunoRepositorio.TotalAlunos();
                ViewBag.TotalProfessores = _professorRepositorio.TotalProfessores();
                ViewBag.TotalIdiomas = _cursoRepositorio.TotalIdiomas();
                ViewBag.TotalCursos = _cursoRepositorio.TotalCursos();

                return View(cursos);
            }
            catch (Exception)
            {
                // Caso ocorra algum erro durante a execução, exibe uma mensagem de erro
                TempData["MensagemErro"] = "Ocorreu um erro ao carregar as informações. Tente novamente mais tarde.";
                return View();
            }
        }
    }
}