using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class AlunoCursoController : Controller
    {
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        public AlunoCursoController(IAlunoCursoRepositorio alunoCursoRepositorio)
        {
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        [HttpPost]
        public async Task<IActionResult> AddAlunoAoCurso(int alunoId, int cursoId)
        {
            await _alunoCursoRepositorio.AddAlunoAoCursoAsync(alunoId, cursoId);
            return RedirectToAction("CursosDisponiveis", "Aluno");
        }

        [HttpPost]
        public async Task<IActionResult> RemoverAlunoDoCurso(int alunoId, int cursoId)
        {
            await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(alunoId, cursoId);

            return RedirectToAction("CursosDoAluno", new { id = alunoId });
        }
    }
}