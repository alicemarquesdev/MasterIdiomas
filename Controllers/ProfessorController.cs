using MasterIdiomas.Data;
using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class ProfessorController : Controller
    {
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly BancoContext _context;

        public ProfessorController(IProfessorRepositorio professorRepositorio,
                                    BancoContext context,
                                    ICursoRepositorio cursoRepositorio)
        {
            _professorRepositorio = professorRepositorio;
            _context = context;
            _cursoRepositorio = cursoRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            List<ProfessorModel> professores = await _professorRepositorio.BuscarTodosProfessoresAsync();
            return View(professores);
        }

        public async Task<IActionResult> CursosDoProfessor(int id)
        {
            List<CursoModel> cursos = await _professorRepositorio.BuscarCursosDoProfessorAsync(id);

            return View(cursos);
        }

        public IActionResult AddProfessor(int id)
        {
            var idiomas = new List<string>
            {
            "Inglês", "Espanhol", "Francês", "Alemão", "Italiano",
            "Mandarim", "Japonês", "Coreano", "Russo", "Árabe",
            "Português", "Hindi", "Turco", "Grego", "Holandês",
            "Polonês", "Sueco", "Dinamarquês", "Norueguês", "Finlandês",
            "Tcheco", "Hebraico", "Indonésio", "Tailandês", "Vietnamita",
            "Romeno", "Húngaro", "Búlgaro", "Croata", "Eslovaco"
            };

            ViewBag.Idiomas = idiomas;

            return View();
        }

        public async Task<IActionResult> AtualizarProfessor(int id)
        {
            ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);
            return View(professor);
        }

        public async Task<IActionResult> RemoverProfessor(int id)
        {
            ProfessorModel professordb = await _professorRepositorio.BuscarProfessorPorIdAsync(id);
            return View(professordb);
        }

        [HttpPost]
        public async Task<IActionResult> AddProfessor(CursoModel professor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _cursoRepositorio.AddCursoAsync(professor);
                    TempData["MensagemSucesso"] = "O professor foi criado com sucesso e está pronto para receber alunos.";
                    return RedirectToAction("Index");
                }

                TempData["MensagemErro"] = "Ocorreu um erro ao tentar cadastrar um novo professor. Por favor, verifique os dados informados e tente novamente.";
                return View(professor);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Desculpe. Houve um erro na operação: {ex.Message}";

                return View(professor);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarProfessor(ProfessorModel professor)
        {
            try
            {
                if (ModelState.IsValid)

                {
                    await _professorRepositorio.AtualizarProfessorAsync(professor);

                    TempData["MensagemSucesso"] = "Os dados do professor foram atualizados com sucesso!";

                    return RedirectToAction("Index");
                }
                return View(professor);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos atualizar os dados do professor, tente novamante, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoverProfessorConfirmacao(int id)
        {
            await _professorRepositorio.RemoverProfessorAsync(id);
            return RedirectToAction("Index");
        }
    }
}