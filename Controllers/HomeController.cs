using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ISessao _sessao;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public HomeController(ILogger<LoginController> logger,
                                ISessao sessao,
                                ICursoRepositorio cursoRepositorio,
                                IProfessorRepositorio professorRepositorio,
                                IAlunoRepositorio alunoRepositorio,
                                IAlunoCursoRepositorio alunoCursoRepositorio,
                                IUsuarioRepositorio usuarioRepositorio)
        {
            _logger = logger;
            _sessao = sessao;
            _cursoRepositorio = cursoRepositorio;
            _professorRepositorio = professorRepositorio;
            _alunoRepositorio = alunoRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<IActionResult> Index(int cursoId)
        {
            var usuario = _sessao.BuscarSessaoUsuario();

            if (usuario == null) return RedirectToAction("Index", "Usuario");

            ViewBag.Usuario = usuario.Nome;

            List<CursoModel> cursos = await _cursoRepositorio.BuscarTodosCursosAsync();

            int totalAlunos = _alunoRepositorio.TotalAlunos();
            int totalProfessores = _professorRepositorio.TotalProfessores();
            int totalIdiomas = _cursoRepositorio.TotalIdiomas();
            int totalCursos = _cursoRepositorio.TotalCursos();

            ViewBag.TotalAlunos = totalAlunos;
            ViewBag.TotalProfessores = totalProfessores;
            ViewBag.ToTalIdiomas = totalIdiomas;
            ViewBag.TotalCursos = totalCursos;
            ViewBag.TotalDeAlunosCurso = await _alunoCursoRepositorio.TotalAlunosCurso(cursoId);

            return View(cursos);
        }
    }
}