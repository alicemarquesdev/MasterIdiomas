using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas aos usuários
    // - VerificarUsuarioExistentePorEmailAsync(string email) - Verifica se já existe um usuário cadastrado com o e-mail fornecido
    // - BuscarUsuarioPorIdAsync(int id) - Busca um usuário específico pelo seu ID
    // - AddUsuarioAsync(UsuarioModel usuario) - Adiciona um novo usuário ao banco de dados, verificando se o e-mail já está em uso e definindo a senha com hash
    // - AtualizarUsuarioAsync(UsuarioModel usuario) - Atualiza os dados de um usuário existente, garantindo que o e-mail não seja duplicado
    // - RemoverUsuario(int id) - Remove um usuário do banco de dados com base no seu ID
    // - AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel) - Altera a senha do usuário, verificando se a senha atual é válida e garantindo que a nova senha seja diferente
    // - RedefinirSenha(int id, string novaSenha) - Redefine a senha de um usuário com base no seu ID e a nova senha fornecida

    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _context;
        private readonly ILogger<UsuarioRepositorio> _logger;

        public UsuarioRepositorio(BancoContext context, ILogger<UsuarioRepositorio> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Buscar usuário por e-mail
        public async Task<UsuarioModel?> VerificarUsuarioExistentePorEmailAsync(string email)
        {
            try
            {
                // Verifica se o e-mail já existe no banco de dados
                return await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar o usuário.");
                throw new Exception("Erro ao buscar o usuário.", ex);
            }
        }

        // Buscar usuário por ID
        public async Task<UsuarioModel?> BuscarUsuarioPorIdAsync(int id)
        {
            try
            {
                // Busca o usuário no banco com base no ID fornecido
                return await _context.Usuarios.FirstOrDefaultAsync(x => x.UsuarioId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar o usuário.");
                throw new Exception("Erro ao buscar o usuário.", ex);
            }
        }

        // Adicionar um novo usuário
        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                // Verifica se já existe um usuário com o mesmo e-mail
                var usuarioExistente = await VerificarUsuarioExistentePorEmailAsync(usuario.Email);
                if (usuarioExistente != null)
                {
                    throw new InvalidOperationException("Já existe um usuário com esse e-mail.");
                }

                // Define a senha com hash
                usuario.SetSenhaHash();

                // Formata nome e e-mail
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                usuario.Nome = textInfo.ToTitleCase(usuario.Nome.ToLower());
                usuario.Email = usuario.Email.ToLower();
                usuario.DataCadastro = DateTime.Now;

                // Adiciona o usuário ao banco de dados
                _context.Usuarios.Add(usuario);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados.");
                }
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar usuário.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar usuário.");
                throw new Exception("Erro ao adicionar usuário.", ex);
            }
        }

        // Atualizar dados do usuário
        public async Task AtualizarUsuarioAsync(UsuarioSemSenhaModel usuarioSemSenha)
        {
            try
            {
                // Verifica se o usuário existe com o ID fornecido
                var usuarioDb = await BuscarUsuarioPorIdAsync(usuarioSemSenha.Id);
                if (usuarioDb == null)
                {
                    throw new Exception("Usuário não encontrado.");
                }

                // Verifica se o e-mail já está cadastrado para outro usuário, excluindo o do usuário atual
                var usuarioComEmailExistente = await VerificarUsuarioExistentePorEmailAsync(usuarioSemSenha.Email);
                if (usuarioComEmailExistente != null && usuarioComEmailExistente.UsuarioId != usuarioDb.UsuarioId)
                {
                    throw new InvalidOperationException("Já existe um usuário com esse e-mail.");
                }

                // Formata nome e e-mail para garantir consistência
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                usuarioDb.Nome = textInfo.ToTitleCase(usuarioSemSenha.Nome.ToLower());
                usuarioDb.Email = usuarioSemSenha.Email.ToLower();
                usuarioDb.DataCadastro = DateTime.Now;
                usuarioDb.Genero = usuarioSemSenha.Genero;
                usuarioDb.DataNascimento = usuarioSemSenha.DataNascimento;

                // Atualiza o usuário no banco de dados
                _context.Usuarios.Update(usuarioDb);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados.");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário.");
                throw new Exception("Erro ao atualizar usuário.", ex);
            }
        }

        // Remover usuário
        public async Task<bool> RemoverUsuario(int id)
        {
            try
            {
                // Verifica se o usuário existe com o ID fornecido
                var usuarioExistente = await BuscarUsuarioPorIdAsync(id);
                if (usuarioExistente == null)
                {
                    throw new Exception("Usuário não encontrado.");
                }

                // Remove o usuário do banco de dados
                _context.Usuarios.Remove(usuarioExistente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover usuário.");
                throw new Exception("Erro ao remover usuário", ex);
            }
        }

        // Alterar senha do usuário
        public async Task AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {
                // Busca o usuário no banco de dados usando o ID fornecido
                UsuarioModel? usuarioDb = await BuscarUsuarioPorIdAsync(alterarSenhaModel.Id);

                if (usuarioDb == null)
                {
                    throw new Exception("Usuário não encontrado para alteração de senha.");
                }

                // Verifica se a senha atual informada é válida
                if (!usuarioDb.SenhaValida(alterarSenhaModel.SenhaAtual))
                {
                    throw new InvalidOperationException("A senha atual informada não está correta.");
                }

                // Verifica se a nova senha é diferente da senha atual
                if (usuarioDb.SenhaValida(alterarSenhaModel.NovaSenha))
                {
                    throw new InvalidOperationException("A nova senha não pode ser igual à senha atual.");
                }

                // Define a nova senha com hash
                usuarioDb.SetNovaSenha(alterarSenhaModel.NovaSenha);

                // Atualiza a senha no banco de dados
                _context.Usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync();
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha.");
                throw new Exception("Erro ao alterar senha.", ex);
            }
        }

        // Redefinir senha do usuário
        public async Task<bool> RedefinirSenha(int id, string novaSenha)
        {
            try
            {
                // Verifica se o usuário existe com o ID fornecido
                var usuarioDb = await BuscarUsuarioPorIdAsync(id);
                if (usuarioDb == null)
                {
                    throw new ArgumentNullException("Usuário não encontrado para redefinir a senha.");
                }

                // Define a nova senha com hash
                usuarioDb.SetNovaSenha(novaSenha);

                // Atualiza a senha do usuário no banco de dados
                _context.Usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha.");
                throw new Exception("Erro ao redefinir senha.", ex);
            }
        }
    }
}
