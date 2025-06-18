using MasterIdiomas.Models;

namespace MasterIdiomas.Helper
{
    // Interface que define os métodos necessários para o gerenciamento da sessão do usuário
    public interface ISessao
    {
        // Cria uma sessão de usuário com as informações fornecidas
        void CriarSessaoUsuario(UsuarioModel usuario);

        // Remove a sessão do usuário
        void RemoverSessaoUsuario();

        // Busca as informações do usuário na sessão
        UsuarioModel BuscarSessaoUsuario();
    }
}