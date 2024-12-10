using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class CursoController : Controller
    {
        // Declaração de dependências
        private readonly ICursoRepositorio _cursoRepositorio;

        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        // Construtor que injeta as dependências necessárias
        public CursoController(ICursoRepositorio cursoRepositorio,
                              IAlunoCursoRepositorio alunoCursoRepositorio,
                              IProfessorRepositorio professorRepositorio)
        {
            _cursoRepositorio = cursoRepositorio;
            _professorRepositorio = professorRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        // Método para exibir a lista de cursos
        public async Task<IActionResult> Index(int CursoId)
        {
            try
            {
                List<CursoModel> cursos = await _cursoRepositorio.BuscarTodosCursosAsync();
                return View(cursos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar cursos: {ex.Message}";
                return View(new List<CursoModel>());
            }
        }

        // Método para exibir alunos de um curso específico
        public async Task<IActionResult> AlunosDoCurso(int CursoId)
        {
            try
            {
                CursoModel curso = await _cursoRepositorio.BuscarCursoPorIdAsync(CursoId);
                if (curso == null)
                {
                    TempData["MensagemErro"] = "Curso não encontrado.";
                    return RedirectToAction("Index");
                }

                List<AlunoModel> alunos = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(curso.CursoId);
                ViewBag.CursoId = curso.CursoId;
                return View(alunos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar alunos do curso: {ex.Message}";
                return View(new List<AlunoModel>());
            }
        }

        // Método para exibir alunos de um curso filtrados por idioma
        public async Task<IActionResult> Idioma(CursoModel curso)
        {
            try
            {
                List<AlunoModel> alunosPorIdioma = await _alunoCursoRepositorio.BuscarAlunosPorIdiomaAsync(curso);
                ViewBag.IdiomaSelecionado = curso.Idioma;

                return View(alunosPorIdioma);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar alunos por idioma: {ex.Message}";
                return View(new List<AlunoModel>());
            }
        }

        // Método para associar um professor a um curso
        public async Task<IActionResult> AddAlunoAoCurso(int id)
        {
            try
            {
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);
                if (curso == null)
                {
                    TempData["MensagemErro"] = "Curso não encontrado.";
                    return RedirectToAction("Index");
                }

                List<AlunoModel> alunos = await _alunoCursoRepositorio.BuscarAlunosNaoInscritosNoCurso(id);
                ViewBag.CursoId = curso.CursoId;

                return View(alunos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar professores para o curso: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Método para exibir o formulário de cadastro de curso
        public async Task<IActionResult> AddCurso()
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
                ViewBag.Professores = await _professorRepositorio.BuscarTodosProfessoresAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar dados para adicionar curso: {ex.Message}";
                return View();
            }
        }

        // Método para associar um professor a um curso
        public async Task<IActionResult> AddProfessorAoCurso(int id)
        {
            try
            {
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);
                if (curso == null)
                {
                    TempData["MensagemErro"] = "Curso não encontrado.";
                    return RedirectToAction("Index");
                }

                List<ProfessorModel> professores = await _professorRepositorio.BuscarTodosProfessoresAsync();
                ViewBag.Professores = professores;

                return View(curso);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar professores para o curso: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Método para atualizar os dados de um curso
        public async Task<IActionResult> AtualizarCurso(int id)
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

                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);
                if (curso == null)
                {
                    TempData["MensagemErro"] = "Curso não encontrado.";
                    return RedirectToAction("Index");
                }

                if (curso.ProfessorId != null)
                {
                    ViewBag.ProfessorId = curso.ProfessorId;
                }

                return View(curso);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar dados do curso: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Método para remover curso
        public async Task<IActionResult> RemoverCurso(int id)
        {
            CursoModel curso = await _cursoRepositorio.BuscarCursoPorIdAsync(id);

            return View(curso);
        }

        // Método para adicionar um novo curso
        [HttpPost]
        public async Task<IActionResult> AddCurso(CursoModel curso, string ProfessorOp, int? professorId)
        {
            try
            {
                await _cursoRepositorio.AddCursoAsync(curso);
                TempData["MensagemSucesso"] = "Curso adicionado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao adicionar curso: {ex.Message}";
                return View(curso);
            }
        }

        // Método para atualizar um curso existente
        [HttpPost]
        public async Task<IActionResult> AtualizarCurso(CursoModel curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _cursoRepositorio.AtualizarCursoAsync(curso);
                    if (curso.Status == Enums.StatusCursoEnum.Cancelado)
                    {
                        TempData["MensagemSucesso"] = "Curso atualizado com sucesso! O status do curso foi alterado para Cancelado. Os alunos do curso foram todos removidos.";
                    }
                    else
                    {
                        TempData["MensagemSucesso"] = "Curso atualizado com sucesso!";
                    }
                    TempData["MensagemSucesso"] = "Curso atualizado com sucesso!";
                    return RedirectToAction("Index");
                }

                TempData["MensagemErro"] = "Ocorreu um erro ao atualizar o curso. Por favor, verifique os dados.";
                return View(curso);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar curso: {ex.Message}";
                return View(curso);
            }
        }

        // Método para remover um curso
        [HttpPost]
        public async Task<IActionResult> RemoverCursoConfirmacao(int id)
        {
            try
            {
                await _cursoRepositorio.RemoverCursoAsync(id);
                TempData["MensagemSucesso"] = "Curso removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao remover curso: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}