using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class CursoController : Controller
    {
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        public CursoController(ICursoRepositorio cursoRepositorio,
                              IAlunoCursoRepositorio alunoCursoRepositorio,
                              IProfessorRepositorio professorRepositorio)
        {
            _cursoRepositorio = cursoRepositorio;
            _professorRepositorio = professorRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        public async Task<IActionResult> Index(int CursoId)
        {
            List<CursoModel> cursos = await _cursoRepositorio.BuscarTodosCursosAsync();


            return View(cursos);
        }

        public async Task<IActionResult> AlunosDoCurso(int CursoId)
        {
            CursoModel curso = await _cursoRepositorio.BuscarCursoPorIdAsync(CursoId);

            List<AlunoModel> alunos = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(curso.CursoId);
            ViewBag.CursoId = curso.CursoId;


            return View(alunos);
        }

        public async Task<IActionResult> Idioma(CursoModel curso)
        {
            List<AlunoModel> alunosPorIdioma = await _alunoCursoRepositorio.BuscarAlunosPorIdiomaAsync(curso);

            ViewBag.IdiomaSelecionado = curso.Idioma;

            return View(alunosPorIdioma);
        }

        public IActionResult AddCurso()
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

        public async Task<IActionResult> AtualizarCurso(int id)
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

            var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);

            if ((curso == null))
            {
                TempData["MensagemErro"] = "Curso não encontrado/Professor associado ao curso não encontrado.";
                return RedirectToAction("Index");
            }

            return View(curso);
        }

        public async Task<IActionResult> RemoverCurso(int id)
        {
            CursoModel curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);

            return View(curso);
        }

        [HttpPost]
        public async Task<IActionResult> AddCurso(CursoModel curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _cursoRepositorio.AddCursoAsync(curso);
                    TempData["MensagemSucesso"] = "O curso foi criado com sucesso e está pronto para receber alunos.";
                    return RedirectToAction("Index");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    TempData["MensagemErro"] = string.Join(" | ", errors); // Mostrar erros para depuração
                    return View(curso);
                }

                TempData["MensagemErro"] = "Ocorreu um erro ao tentar criar o curso. Por favor, verifique os dados informados e tente novamente.";
                return View(curso);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Desculpe. Houve um erro na operação: {ex.Message}";

                return View(curso);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarCurso(CursoModel curso)
        {
            try
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

                if (ModelState.IsValid)
                {
                    await _cursoRepositorio.AtualizarCursoAsync(curso);
                    TempData["MensagemSucesso"] = "O Curso foi atualizado com sucesso";
                    return RedirectToAction("Index");
                }
                TempData["MensagemErro"] = "Ocorreu um erro ao atualizar o curso. Por favor, verifique os dados informados e tente novamente.";
                return View(curso);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Desculpe. Houve um erro na operação: {ex.Message}";
                return View(curso);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoverCursoConfirmacao(int id)
        {
            try
            {
                await _cursoRepositorio.RemoverCursoAsync(id);

                TempData["MensagemSucesso"] = "O curso foi removido com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Desculpe. Houve um erro na operação: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}