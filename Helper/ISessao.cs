using MasterIdiomas.Models;

namespace MasterIdiomas.Helper
{
    public interface ISessao
    {
        void CriarSessaoUsuario(UsuarioModel usuario);

        void RemoverSessaoUsuario();

        UsuarioModel BuscarSessaoUsuario();
    }
}