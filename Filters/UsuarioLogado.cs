using MasterIdiomas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace MasterIdiomas.Filters
{
    // Filtro personalizado para verificar se o usuário está logado
    // Para permitir acesso a views
    public class UsuarioLogado : ActionFilterAttribute
    {
       
        // Método que é executado antes da ação ser chamada
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                // Recupera a sessão do usuário
                string sessaoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");

                // Verifica se a sessão do usuário está vazia ou nula
                if (string.IsNullOrEmpty(sessaoUsuario))
                {
                    // Se a sessão estiver vazia, redireciona para a página de login
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Login" }
                    });
                }
                else
                {
                    // Desserializa a sessão do usuário para um objeto UsuarioModel
                    UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);

                    // Verifica se o usuário não foi encontrado após a desserialização
                    if (usuario == null)
                    {
                        // Se não encontrar o usuário, redireciona para a página de login
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Login" }
                        });
                    }
                }

                // Chama o método base para continuar o fluxo normal
                base.OnActionExecuting(context);
            }
            catch (Exception ex)
            {
                // Em caso de erro, redireciona para a página de login
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Login" },
                    { "action", "Login" }
                });

                throw new Exception("Erro ao verificar a sessão do usuário.", ex);
            }
        }
    }
}