using MasterIdiomas.Models;

namespace MasterIdiomas.Repositorio.Interfaces
{
    // Interface para operações relacionadas ao usuário, inclui métodos de alteração de senha do usuário.
    public interface IUsuarioRepositorio
    {
        // Retorna um usuário específico com base no seu ID
        Task<UsuarioModel?> BuscarUsuarioPorIdAsync(int id);

        // Verifica se um usuário já existe no sistema com base no seu e-mail e retorna o usuário
        Task<UsuarioModel?> VerificarUsuarioExistentePorEmailAsync(string email);

        // Adiciona um novo usuário ao sistema
        Task AddUsuarioAsync(UsuarioModel usuario);

        // Atualiza as informações de um usuário existente
        Task AtualizarUsuarioAsync(UsuarioSemSenhaModel usuarioSemSenha);

        // Remove um usuário do sistema com base no seu ID
        Task<bool> RemoverUsuario(int id);

        // Métodos para alterar a senha do usuário

        // Altera a senha de um usuário
        Task AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel);

        // Redefine a senha de um usuário com uma nova senha
        Task<bool> RedefinirSenha(int id, string novaSenha);
    }
}
