using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class AlunoController : Controller
    {
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        // Construtor que injeta os repositórios necessários
        public AlunoController(IAlunoRepositorio alunoRepositorio,
                                IAlunoCursoRepositorio alunoCursoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        // Método para listar todos os alunos
        public async Task<ActionResult> Index()
        {
            List<AlunoModel> alunos = await _alunoRepositorio.BuscarTodosAlunosAsync();
            return View(alunos);
        }
         
        // Método para exibir o formulário de adição de um novo aluno
        public ActionResult AddAluno()
        {
            return View();
        }

        // Método para buscar e exibir os dados de um aluno para edição
        public async Task<ActionResult> AtualizarAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);
            return View(aluno);
        }

        // Método para buscar e exibir os dados de um aluno para remoção
        public async Task<ActionResult> RemoverAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);
            return View(aluno);
        }

        // Método para listar os cursos em que um aluno está matriculado
        public async Task<ActionResult> CursosDoAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);

            if (aluno == null)
            {
                TempData["MensagemErro"] = "Aluno não encontrado!";
                return RedirectToAction("Index");
            }

            List<CursoModel> cursosAluno = await _alunoCursoRepositorio.BuscarCursosDoAlunoAsync(aluno.AlunoId);
            ViewBag.AlunoId = aluno.AlunoId;

            // Verifica se existe um AlunoId na TempData e o passa para a ViewBag
            if (TempData["AlunoId"] != null)
            {
                ViewBag.AlunoId = TempData["AlunoId"];
            }

            return View(cursosAluno);
        }

        // Método para listar cursos disponíveis para um aluno se matricular
        public async Task<ActionResult> CursosDisponiveis(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);

            if (aluno == null)
            {
                TempData["MensagemErro"] = "Aluno não encontrado!";
                return View();
            }

            List<CursoModel> cursosDisponiveis = await _alunoCursoRepositorio.BuscarCursosAlunoNaoInscritoAsync(aluno.AlunoId);

            ViewBag.AlunoId = aluno.AlunoId;  // Passando o AlunoId para a view

            // Verificando se TempData contém o AlunoId, para garantir que o valor seja repassado
            if (TempData["AlunoId"] != null)
            {
                ViewBag.AlunoId = TempData["AlunoId"];
            }

            return View(cursosDisponiveis);
        }

        // Ação POST para adicionar um novo aluno
        [HttpPost]
        public async Task<ActionResult> AddAluno(AlunoModel aluno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _alunoRepositorio.AddAlunoAsync(aluno);

                    TempData["MensagemSucesso"] = "O Aluno foi criado com sucesso!";
                    return RedirectToAction("Index");
                }
                return View(aluno);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Desculpe, ocorreu um erro no cadastro do aluno, tente novamente.";
                return View(aluno);
            }
        }

        // Ação POST para atualizar os dados de um aluno
        [HttpPost]
        public async Task<ActionResult> AtualizarAluno(AlunoModel aluno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _alunoRepositorio.AtualizarAlunoAsync(aluno);

                    TempData["MensagemSucesso"] = "Os dados do aluno foram atualizados com sucesso!";
                    return RedirectToAction("Index");
                }
                return View(aluno);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Desculpe, não conseguimos atualizar os dados do aluno, tente novamente.";
                return View(aluno);
            }
        }

        // Ação POST para confirmar a remoção de um aluno
        [HttpPost]
        public async Task<ActionResult> RemoverAlunoConfirmacao(int id)
        {
            try
            {
                await _alunoRepositorio.RemoverAlunoAsync(id);

                TempData["MensagemSucesso"] = "O aluno foi removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Desculpe, não conseguimos remover o aluno, tente novamente. {ex}";
                return RedirectToAction("Index");
            }
        }
    }
}