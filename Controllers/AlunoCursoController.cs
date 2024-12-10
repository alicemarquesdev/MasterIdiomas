using MasterIdiomas.Filters;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class AlunoCursoController : Controller
    {
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IAlunoRepositorio _alunoRepositorio;

        // Construtor da controller, injetando as dependências
        public AlunoCursoController(IAlunoCursoRepositorio alunoCursoRepositorio,
                                    ICursoRepositorio cursoRepositorio,
                                    IAlunoRepositorio alunoRepositorio)
        {
            _alunoCursoRepositorio = alunoCursoRepositorio;
            _cursoRepositorio = cursoRepositorio;
            _alunoRepositorio = alunoRepositorio;
        }

        // Ação para adicionar um aluno a um curso
        [HttpPost]
        public async Task<IActionResult> AddAlunoAoCurso(int alunoId, int cursoId)
        {
            try
            {
                var urlAnterior = Request.Headers["Referer"].ToString();

                // Verificando se o aluno e o curso existem
                var alunoExiste = await _alunoRepositorio.BuscarAlunoPorIdAsync(alunoId);
                var cursoExiste = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                // Se aluno ou curso não forem encontrados, redireciona para a página de cursos disponíveis
                if (alunoExiste == null || cursoExiste == null)
                {
                    TempData["MensagemErro"] = "Aluno/Curso não encontrado!";
                    return Redirect(urlAnterior);
                }

                // Adicionando o aluno ao curso
                await _alunoCursoRepositorio.AddAlunoAoCursoAsync(alunoId, cursoId);

                // Mensagem de sucesso e armazenamento do AlunoId
                TempData["MensagemSucesso"] = "O Aluno foi adicionado ao curso com sucesso!";
                TempData["AlunoId"] = alunoId; // Armazenando o AlunoId em TempData

                // Redireciona de volta para a página de cursos disponíveis para o aluno
                return Redirect(urlAnterior);
            }
            catch (Exception ex)
            {
                // Log do erro (opcional, dependendo da implementação de log no projeto)
                TempData["MensagemErro"] = $"Erro ao adicionar o aluno ao curso: {ex.Message}";
                return RedirectToAction("CursosDisponiveis", "Aluno");
            }
        }

        // Ação para remover um aluno de um curso
        [HttpPost]
        public async Task<IActionResult> RemoverAlunoDoCurso(int alunoId, int cursoId)
        {
            try
            {
                var urlAnterior = Request.Headers["Referer"].ToString();

                // Verificando se o aluno e o curso existem
                var alunoExiste = await _alunoRepositorio.BuscarAlunoPorIdAsync(alunoId);
                var cursoExiste = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);

                // Se aluno ou curso não forem encontrados, redireciona para a página de cursos do aluno
                if (alunoExiste == null || cursoExiste == null)
                {
                    TempData["MensagemErro"] = "Aluno/Curso não encontrado!";
                    return Redirect(urlAnterior);
                }

                // Removendo o aluno do curso
                await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(alunoId, cursoId);

                // Mensagem de sucesso e armazenamento do AlunoId
                TempData["MensagemSucesso"] = "O Aluno foi removido do curso.";
                TempData["AlunoId"] = alunoId; // Armazenando o AlunoId em TempData

                // Redireciona de volta para a página de cursos do aluno
                return Redirect(urlAnterior);
            }
            catch (Exception ex)
            {
                // Log do erro (opcional, dependendo da implementação de log no projeto)
                TempData["MensagemErro"] = $"Desculpe, houve erro ao remover o aluno do curso: {ex.Message}";
                return RedirectToAction("CursosDoAluno", "Aluno");
            }
        }
    }
}