using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<UsuarioModel> BuscarUsuarioPorIdAsync(int id);

        Task<UsuarioModel> BuscarUsuarioExistenteAsync(string email);

        Task AddUsuarioAsync(UsuarioModel usuario);

        Task AtualizarUsuarioAsync(UsuarioModel usuario);

        Task AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel);
    }
}