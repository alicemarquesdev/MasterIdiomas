using MasterIdiomas.Models;
using Newtonsoft.Json;

namespace MasterIdiomas.Helper
{
    // Implementação da interface ISessao, responsável pela gestão da sessão do usuário
    public class Sessao : ISessao
    {
        private readonly IHttpContextAccessor _httpContext;

        // Construtor que recebe a dependência IHttpContextAccessor para acessar o contexto da sessão
        public Sessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        // Método para buscar as informações do usuário na sessão
        public UsuarioModel BuscarSessaoUsuario()
        {
            // Recupera o valor da sessão 'sessaoUsuarioLogado'
            string sessaoUsuario = _httpContext.HttpContext.Session.GetString("sessaoUsuarioLogado");

            // Se a sessão estiver vazia ou nula, retorna null
            if (string.IsNullOrEmpty(sessaoUsuario)) return null;

            // Desserializa o valor da sessão para o objeto UsuarioModel
            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
        }

        // Método para criar a sessão do usuário com as informações fornecidas
        public void CriarSessaoUsuario(UsuarioModel usuario)
        {
            // Serializa o objeto UsuarioModel para string
            string valor = JsonConvert.SerializeObject(usuario);

            // Define o valor na sessão
            _httpContext.HttpContext.Session.SetString("sessaoUsuarioLogado", valor);
        }

        // Método para remover a sessão do usuário
        public void RemoverSessaoUsuario()
        {
            // Remove a sessão do usuário
            _httpContext.HttpContext.Session.Remove("sessaoUsuarioLogado");
        }
    }
}