using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly BancoContext _context;

        public AlunoRepositorio(BancoContext context)
        {
            _context = context;
        }

        public async Task<List<AlunoModel>> BuscarTodosAlunosAsync()
        {
            try
            {
                // Retorna todos os alunos com seus cursos relacionados
                return await _context.Alunos
                    .Include(a => a.AlunoCurso)
                    .ThenInclude(a => a.Curso)
                    .OrderBy(n => n.Nome)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar todos os alunos. Tente novamente mais tarde.");
            }
        }

        public async Task<AlunoModel> BuscarAlunoPorIdAsync(int id)
        {
            try
            {
                // Retorna um aluno pelo seu ID
                return await _context.Alunos
                    .FirstOrDefaultAsync(x => x.AlunoId == id);
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar o aluno. Tente novamente mais tarde.");
            }
        }

        public async Task<AlunoModel> BuscarAlunoExistenteAsync(string nome, DateTime dataNascimento)
        {
            try
            {
                // Verifica se o aluno existe pelo nome e data de nascimento
                return await _context.Alunos
                    .FirstOrDefaultAsync(x => x.Nome == nome && x.DataNascimento == dataNascimento);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar o aluno existente. Tente novamente mais tarde.", ex);
            }
        }

        public int TotalAlunos()
        {
            try
            {
                // Retorna o total de alunos cadastrados
                return _context.Alunos.Count();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao contar o total de alunos. Tente novamente mais tarde.", ex);
            }
        }

        public async Task AddAlunoAsync(AlunoModel aluno)
        {
            try
            {
                var alunoExistente = await BuscarAlunoExistenteAsync(aluno.Nome, aluno.DataNascimento);
                if (alunoExistente != null)
                {
                    throw new Exception($"O aluno {aluno.Nome} com data de nascimento {aluno.DataNascimento} já está registrado.");
                }

                // Adiciona um novo aluno
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                aluno.Nome = textInfo.ToTitleCase(aluno.Nome.ToLower());
                aluno.DataCadastro = DateTime.Now;
                aluno.Status = StatusEnum.Ativo;

                await _context.Alunos.AddAsync(aluno);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task AtualizarAlunoAsync(AlunoModel aluno)
        {
            try
            {
                // Atualiza os dados de um aluno
                AlunoModel alunoDB = await BuscarAlunoPorIdAsync(aluno.AlunoId);

                if (alunoDB == null)
                    throw new Exception("Aluno não encontrado para atualização.");

                var alunoExistente = await BuscarAlunoExistenteAsync(aluno.Nome, aluno.DataNascimento);
                if (alunoExistente != null)
                {
                    throw new Exception($"O aluno {aluno.Nome} com data de nascimento {aluno.DataNascimento} já está registrado.");
                }

                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                alunoDB.Nome = textInfo.ToTitleCase(aluno.Nome.ToLower());
                alunoDB.DataNascimento = aluno.DataNascimento;
                alunoDB.Status = aluno.Status;
                alunoDB.Genero = aluno.Genero;

                _context.Update(alunoDB);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task<bool> RemoverAlunoAsync(int id)
        {
            try
            {
                // Remove um aluno pelo ID
                AlunoModel alunoDb = await BuscarAlunoPorIdAsync(id);

                if (alunoDb == null)
                    throw new Exception("Aluno não encontrado para remoção.");

                _context.Alunos.Remove(alunoDb);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
    }
}