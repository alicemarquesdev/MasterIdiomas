using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Components
{
    public class UsuarioViewComponent : ViewComponent
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;

        public UsuarioViewComponent(IUsuarioRepositorio usuarioRepositorio,
                                    ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoUsuario();

                if (usuario != null)
                {
                    UsuarioModel usuarioDB = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(usuario.UsuarioId);

                    return View(usuarioDB);
                }

                return View("Error", new { Message = "Ocorreu um erro ao carregar o usuário. Tente novamente mais tarde." });
            }
            catch (Exception)
            {
                return View("Error", new { Message = "Ocorreu um erro ao carregar o usuário. Tente novamente mais tarde." });
            }
        }
    }
}