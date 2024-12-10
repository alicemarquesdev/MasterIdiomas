using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // Filtra ações apenas para usuários logados
    [UsuarioLogado]
    public class ProfessorController : Controller
    {
        // Injeção de dependência
        private readonly IProfessorRepositorio _professorRepositorio;

        private readonly ICursoRepositorio _cursoRepositorio;

        // Construtor do Controller
        public ProfessorController(IProfessorRepositorio professorRepositorio,
                                    ICursoRepositorio cursoRepositorio)
        {
            _professorRepositorio = professorRepositorio;
            _cursoRepositorio = cursoRepositorio;
        }

        // Ação para exibir todos os professores
        public async Task<IActionResult> Index()
        {
            try
            {
                List<ProfessorModel> professores = await _professorRepositorio.BuscarTodosProfessoresAsync();
                return View(professores);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar a lista de professores: {ex.Message}";
                return View();
            }
        }

        // Ação para exibir os cursos de um professor
        public async Task<IActionResult> CursosDoProfessor(int id)
        {
            try
            {
                // Buscar professor pelo ID
                ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);

                // Verificar se o professor existe
                if (professor == null)
                {
                    TempData["MensagemErro"] = "Professor não encontrado!";
                    return View();
                }

                // Buscar cursos do professor
                List<CursoModel> cursos = await _professorRepositorio.BuscarCursosDoProfessorAsync(id);

                // Passar o ID do professor para a View
                ViewBag.ProfessorId = professor.ProfessorId;

                return View(cursos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar cursos do professor: {ex.Message}";
                return View();
            }
        }

        // Ação para exibir todos os cursos disponíveis
        public async Task<IActionResult> CursosDisponiveis(int id)
        {
            try
            {
                // Buscar professor pelo ID
                ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);

                // Verificar se o professor existe
                if (professor == null)
                {
                    TempData["MensagemErro"] = "Professor não encontrado!";
                    return View();
                }

                // Buscar todos os cursos disponíveis
                List<CursoModel> cursosDisponiveis = await _cursoRepositorio.BuscarTodosCursosAsync();

                // Passar o ID do professor para a View
                ViewBag.ProfessorId = professor.ProfessorId;

                return View(cursosDisponiveis);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar cursos disponíveis: {ex.Message}";
                return View();
            }
        }

        // Ação para exibir o formulário de cadastro de professor
        public IActionResult AddProfessor()
        {
            return View();
        }

        // Ação para exibir o formulário de atualização de professor
        public async Task<IActionResult> AtualizarProfessor(int id)
        {
            try
            {
                ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);
                return View(professor);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar professor para atualização: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Ação para exibir a confirmação de remoção de professor
        public async Task<IActionResult> RemoverProfessor(int id)
        {
            try
            {
                ProfessorModel professor = await _professorRepositorio.BuscarProfessorPorIdAsync(id);
                return View(professor);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar professor para remoção: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Ação POST para adicionar um novo professor
        [HttpPost]
        public async Task<IActionResult> AddProfessor(ProfessorModel professor)
        {
            try
            {
                // Verificar se o modelo é válido
                if (ModelState.IsValid)
                {
                    await _professorRepositorio.AddProfessorAsync(professor);
                    TempData["MensagemSucesso"] = "Professor criado com sucesso!";
                    return RedirectToAction("Index");
                }

                TempData["MensagemErro"] = "Dados inválidos, por favor verifique e tente novamente.";
                return View(professor);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao criar professor: {ex.Message}";
                return View(professor);
            }
        }

        // Ação POST para atualizar os dados do professor
        [HttpPost]
        public async Task<IActionResult> AtualizarProfessor(ProfessorModel professor)
        {
            try
            {
                // Verificar se o modelo é válido
                if (ModelState.IsValid)
                {
                    await _professorRepositorio.AtualizarProfessorAsync(professor);
                    if (professor.Status == Enums.StatusEnum.Inativo)
                    {
                        TempData["MensagemSucesso"] = "Professor atualizado com sucesso! O status do professor foi alterado para Inativo. O professor foi removido de todos os cursos.";
                    }
                    else
                    {
                        TempData["MensagemSucesso"] = "Professor atualizado com sucesso!";
                    }
                    return RedirectToAction("Index");
                }

                return View(professor);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar professor: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Ação POST para remover um professor
        [HttpPost]
        public async Task<IActionResult> RemoverProfessorConfirmacao(int id)
        {
            try
            {
                await _professorRepositorio.RemoverProfessorAsync(id);
                TempData["MensagemSucesso"] = "Professor removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao remover professor: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProfessorAoCurso(int professorId, int cursoId)
        {
            var urlAnterior = Request.Headers["Referer"].ToString();

            try
            {
                // Validar se o professor e o curso existem
                var professorExiste = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var cursoExiste = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                if (professorExiste == null)
                {
                    TempData["MensagemErro"] = "O professor especificado não foi encontrado. Verifique os dados e tente novamente.";
                    return Redirect(urlAnterior);
                }

                if (cursoExiste == null)
                {
                    TempData["MensagemErro"] = "O curso especificado não foi encontrado. Verifique os dados e tente novamente.";
                    return Redirect(urlAnterior);
                }

                // Adicionar professor ao curso
                await _professorRepositorio.AddProfessorAoCursoAsync(professorId, cursoId);
                TempData["MensagemSucesso"] = $"O professor '{professorExiste.Nome}' foi adicionado ao curso '{cursoExiste.Idioma}' com sucesso.";
                TempData["ProfessorId"] = professorId;

                return Redirect(urlAnterior);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"{ex.Message}";
                return Redirect(urlAnterior);
            }
        }

        // Método para remover um professor de um curso
        [HttpPost]
        public async Task<IActionResult> RemoverProfessorDoCurso(int professorId, int cursoId)
        {
            var urlAnterior = Request.Headers["Referer"].ToString();

            try
            {
                var professorExiste = await _professorRepositorio.BuscarProfessorPorIdAsync(professorId);
                var cursoExiste = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                if (professorExiste == null || cursoExiste == null)
                {
                    TempData["MensagemErro"] = "Professor ou curso não encontrado.";
                    return Redirect(urlAnterior);
                }

                await _professorRepositorio.RemoverProfessorDoCursoAsync(professorId, cursoId);
                TempData["MensagemSucesso"] = "Professor removido do curso com sucesso!";
                return Redirect(urlAnterior);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao remover professor do curso: {ex.Message}";
                return Redirect(urlAnterior);
            }
        }
    }
}