using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _context;

        public UsuarioRepositorio(BancoContext context)
        {
            _context = context;
        }

        public async Task<UsuarioModel> BuscarUsuarioExistenteAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(x => x.Email.ToUpper() == email.ToUpper());
        }

        public async Task<UsuarioModel> BuscarUsuarioPorIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(x => x.UsuarioId == id);
        }

        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            usuario.SetSenhaHash();
            usuario.DataCadastro = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarUsuarioAsync(UsuarioModel usuario)
        {
            UsuarioModel usuarioDb = await BuscarUsuarioPorIdAsync(usuario.UsuarioId);

            if (usuarioDb == null) throw new Exception("Houve um erro,tente novamente");

            usuarioDb.UsuarioId = usuario.UsuarioId;
            usuarioDb.Nome = usuario.Nome;
            usuarioDb.Email = usuario.Email;
            usuarioDb.DataCadastro = usuario.DataCadastro;

            _context.Usuarios.Update(usuarioDb);
            await _context.SaveChangesAsync();
        }

        public async Task AlterarSenhaAsync(AlterarSenhaModel alterarSenhaModel)
        {
            UsuarioModel usuarioDB = await BuscarUsuarioPorIdAsync(alterarSenhaModel.Id);

            if (usuarioDB == null) throw new Exception("Houve um erro na atualização da senha, usuário não encontrado!");

            if (!usuarioDB.SenhaValida(alterarSenhaModel.SenhaAtual)) throw new Exception("Senha atual não confere!");

            if (usuarioDB.SenhaValida(alterarSenhaModel.NovaSenha)) throw new Exception("Nova senha deve ser diferente da senha atual!");

            usuarioDB.SetNovaSenha(alterarSenhaModel.NovaSenha);

            _context.Usuarios.Update(usuarioDB);
            await _context.SaveChangesAsync();
        }
    }
}