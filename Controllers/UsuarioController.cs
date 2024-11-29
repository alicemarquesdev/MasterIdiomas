using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ICursoRepositorio _cursoRepositorio;
        private readonly IProfessorRepositorio _professorRepositorio;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly ISessao _sessao;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioController(ICursoRepositorio cursoRepositorio,
                                ISessao sessao,
                                IProfessorRepositorio professorRepositorio,
                                IAlunoRepositorio alunoRepositorio,
                                IAlunoCursoRepositorio alunoCursoRepositorio,
                                IUsuarioRepositorio usuarioRepositorio)
        {
            _cursoRepositorio = cursoRepositorio;
            _sessao = sessao;
            _professorRepositorio = professorRepositorio;
            _alunoRepositorio = alunoRepositorio;
            _alunoCursoRepositorio = alunoCursoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<IActionResult> AlterarSenha(int id)
        {
            UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

            return View(usuario);
        }

        public async Task<IActionResult> AtualizarUsuario(int id)
        {
            UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

            return View(usuario);
        }

      

        [HttpPost]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {
                UsuarioModel usuarioLogado = _sessao.BuscarSessaoUsuario();

                if (usuarioLogado == null)
                {
                    TempData["MensagemErro"] = "Sessão do usuário não encontrada.";
                    return RedirectToAction("Index");
                }

                alterarSenhaModel.Id = usuarioLogado.UsuarioId;

                if (ModelState.IsValid)
                {
                    await _usuarioRepositorio.AlterarSenhaAsync(alterarSenhaModel);
                    TempData["MensagemSucesso"] = "Senha alterada com sucesso!";
                    return View("Index", alterarSenhaModel);
                }

                TempData["MensagemErro"] = "Ops, não conseguimos alterar sua senha, tente novamente.";
                return View("Index", alterarSenhaModel);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Ops, não conseguimos alterar sua senha, tente novamente.";
                return View("Index", alterarSenhaModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarUsuario(UsuarioSemSenhaModel usuarioSemSenha)
        {
            try
            {
                UsuarioModel usuario = null;

                if (ModelState.IsValid)
                {
                    
                    await _usuarioRepositorio.AtualizarUsuarioAsync(usuario);
                    TempData["MensagemSucesso"] = "Os dados foram atualizados com sucesso!";
                    return RedirectToAction("Index", "PerfilAdmin");
                }
                return View(usuario);
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Desculpe. Não conseguimos atualizar seus dados, tente novamente.";
                return RedirectToAction("Index", "PerfilAdmin");
            }
        }

       
    }
}