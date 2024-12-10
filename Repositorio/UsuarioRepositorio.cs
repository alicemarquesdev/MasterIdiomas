using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _context;

        public UsuarioRepositorio(BancoContext context)
        {
            _context = context;
        }

        // Buscar usuário por e-mail
        public async Task<UsuarioModel> BuscarUsuarioExistenteAsync(string email)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                // Mensagem de erro amigável
                throw new Exception("Ocorreu um erro ao buscar o usuário. Tente novamente mais tarde.", ex);
            }
        }

        // Buscar usuário por ID
        public async Task<UsuarioModel> BuscarUsuarioPorIdAsync(int id)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.UsuarioId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao buscar o usuário. Tente novamente mais tarde.", ex);
            }
        }

        // Adicionar um novo usuário
        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            // Verificar se o usuário já existe com base no e-mail
            var usuarioExistente = await BuscarUsuarioExistenteAsync(usuario.Email);

            if (usuarioExistente != null)
            {
                throw new Exception("Já existe um usuário registrado com este e-mail.");
            }

            // Definir senha com hash
            usuario.SetSenhaHash();
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            usuario.Nome = textInfo.ToTitleCase(usuario.Nome.ToLower());
            usuario.Email = usuario.Email.ToLower();
            usuario.DataCadastro = DateTime.Now;

            // Adicionar o usuário no banco de dados
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Atualizar dados do usuário
        public async Task AtualizarUsuarioAsync(UsuarioModel usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "Os dados do usuário não foram fornecidos.");
            }

            // Verificar se o usuário já existe com base no e-mail
            var usuarioExistente = await BuscarUsuarioExistenteAsync(usuario.Email);

            if (usuarioExistente != null)
            {
                throw new Exception("Já existe um usuário registrado com este e-mail.");
            }

            try
            {
                // Busca o usuário no banco de dados
                UsuarioModel usuarioDb = await BuscarUsuarioPorIdAsync(usuario.UsuarioId);

                if (usuarioDb == null)
                {
                    throw new KeyNotFoundException("Usuário não encontrado para atualização.");
                }

                // Atualizar os dados do usuário (não altera DataCadastro, pois é a data de criação)
                usuarioDb.Nome = usuario.Nome;
                usuarioDb.Email = usuario.Email;

                // Salvar alterações no banco de dados
                _context.Usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Os dados fornecidos para atualização são inválidos.", ex);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("O usuário não foi encontrado no banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao atualizar os dados do usuário. Tente novamente mais tarde.", ex);
            }
        }

        // Alterar senha do usuário
        public async Task AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {
                UsuarioModel usuarioDB = await BuscarUsuarioPorIdAsync(alterarSenhaModel.Id);

                if (usuarioDB == null)
                {
                    throw new Exception("Usuário não encontrado para alteração de senha.");
                }

                // Verificar se a senha atual é válida
                if (!usuarioDB.SenhaValida(alterarSenhaModel.SenhaAtual))
                {
                    throw new Exception("A senha atual informada não está correta.");
                }

                // Verificar se a nova senha é diferente da atual
                if (usuarioDB.SenhaValida(alterarSenhaModel.NovaSenha))
                {
                    throw new Exception("A nova senha não pode ser igual à senha atual.");
                }

                // Definir a nova senha com hash
                usuarioDB.SetNovaSenha(alterarSenhaModel.NovaSenha);

                // Atualizar senha no banco de dados
                _context.Usuarios.Update(usuarioDB);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Melhorando o tratamento de exceções
                throw new Exception("Ocorreu um erro ao alterar a senha. Tente novamente mais tarde.", ex);
            }
        }

        public async Task<bool> RemoverUsuario(int id)
        {
            try
            {
                UsuarioModel usuario = await BuscarUsuarioPorIdAsync(id);

                if (usuario == null)
                    throw new Exception("Usuário não encontrado para remoção.");

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover usuário. Tente novamente mais tarde.", ex);
            }
        }
    }
}