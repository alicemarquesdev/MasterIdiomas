using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Components
{
    // Define um componente de exibição (ViewComponent) para a barra de navegação (NavBar)
    public class NavBarViewComponent : ViewComponent
    {
        // Declaração dos campos privados para repositórios e serviços necessários
        private readonly IUsuarioRepositorio _usuarioRepositorio; // Repositório para manipulação de dados de usuários
        private readonly ISessao _sessao; // Serviço para acessar a sessão do usuário

        // Construtor que recebe as dependências via injeção de dependência
        public NavBarViewComponent(IUsuarioRepositorio usuarioRepositorio,
                                    ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio; // Atribui o repositório de usuário
            _sessao = sessao; // Atribui o serviço de sessão
        }

        // Método que é chamado para gerar a view do componente de navegação
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Recupera o usuário da sessão, que contém as informações do usuário logado
                var usuario = _sessao.BuscarSessaoUsuario();

                // Verifica se há um usuário na sessão
                if (usuario != null)
                {
                    // Se o usuário estiver na sessão, busca os dados completos do usuário no banco de dados
                    UsuarioModel usuarioDB = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(usuario.UsuarioId);

                    // Retorna a view com os dados do usuário recuperado
                    return View(usuarioDB);
                }

                // Se não encontrar um usuário na sessão, retorna uma view de erro com uma mensagem
                return View("Error", new { Message = "Ocorreu um erro ao carregar o usuário. Tente novamente mais tarde." });
            }
            catch (Exception)
            {
                // Se ocorrer algum erro durante o processo, retorna uma view de erro
                return View("Error", new { Message = "Ocorreu um erro ao carregar o usuário. Tente novamente mais tarde." });
            }
        }
    }
}
