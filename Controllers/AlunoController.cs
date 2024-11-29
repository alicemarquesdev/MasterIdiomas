using MasterIdiomas.Filters;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class AlunoController : Controller
    {
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        public AlunoController(IAlunoRepositorio alunoRepositorio,
                                IAlunoCursoRepositorio alunoCursoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        public async Task<ActionResult> Index()
        {
            List<AlunoModel> alunos = await _alunoRepositorio.BuscarTodosAlunosAsync();
            return View(alunos);
        }

        public ActionResult AddAluno()
        {
            return View();
        }

        public async Task<ActionResult> AtualizarAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);
            return View(aluno);
        }

        public async Task<ActionResult> RemoverAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);
            return View(aluno);
        }

        public async Task<ActionResult> CursosDoAluno(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);

            if (aluno == null)
            {
                TempData["MensagemErro"] = "Aluno não encontrado!";
                return RedirectToAction("Index"); // Redireciona para a página inicial ou de listagem.
            }

            List<CursoModel> cursosAluno = await _alunoCursoRepositorio.BuscarCursosDoAlunoAsync(id);
            ViewBag.AlunoId = id;

            return View(cursosAluno);
        }

        public async Task<ActionResult> CursosDisponiveis(int id)
        {
            AlunoModel aluno = await _alunoRepositorio.BuscarAlunoPorIdAsync(id);

            if (aluno == null)
            {
                TempData["MensagemErro"] = "Aluno não encontrado!";
                return RedirectToAction("Index"); // Redireciona para a página inicial ou de listagem.
            }

            List<CursoModel> cursosDisponiveis = await _alunoCursoRepositorio.BuscarCursosAlunoNaoInscritoAsync(id);

            ViewBag.AlunoId = id;

            return View(cursosDisponiveis);
        }

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

        [HttpPost]
        public async Task<ActionResult> RemoverAlunoConfirmacao(int id)
        {
            try
            {
                await _alunoRepositorio.RemoverAlunoAsync(id);

                TempData["MensagemSucesso"] = "O aluno foi removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Desculpe, não conseguimos remover o aluno, tente novamente.";
                return RedirectToAction("Index");
            }
        }
    }
}