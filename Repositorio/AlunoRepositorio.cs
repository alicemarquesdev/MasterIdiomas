using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas à alunos, incluindo busca de alunos, adição e remoção
    // - BuscarAlunoPorIdAsync(int id) -  buscar um aluno específico pelo seu ID
    // - BuscarTodosAlunosAsync() -  buscar todos os alunos, com seus cursos associados
    // - VerificarAlunoExistenteAsync(string nome, DateTime dataDascimento) -  para verificar se um aluno já está registrado com o mesmo nome e data de nascimento
    // - AddAlunoAsync(AlunoModel aluno) - adicionar aluno.
    // - AtualizarAlunoAsync(AlunoModel aluno) - atualizar os dados do aluno
    // - RemoverAlunoAsync(int id) - remover o aluno do banco de dados.
    // - TotalAlunos() - para contar o total de alunos cadastrados no banco
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly BancoContext _context;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly ILogger<AlunoRepositorio> _logger;

        // Construtor para injeção do contexto do banco de dados
        public AlunoRepositorio(BancoContext context, IAlunoCursoRepositorio alunoCursoRepositorio,ILogger<AlunoRepositorio> logger)
        {
            _context = context;
            _alunoCursoRepositorio = alunoCursoRepositorio;
            _logger = logger;
        }

        // Método para buscar um aluno específico pelo seu ID
        public async Task<AlunoModel?> BuscarAlunoPorIdAsync(int id)
        {
            try
            {
                // Busca o aluno pelo ID e inclui os cursos associados
                var aluno = await _context.Alunos.FirstOrDefaultAsync(x => x.AlunoId == id);

                return aluno;
            }
            catch (Exception ex)
            {
                // Tratamento de exceção em caso de falha na busca
                _logger.LogError(ex, "Erro ao buscar o aluno.");
                throw new Exception("Erro ao buscar o aluno.", ex);
            }
        }

        // Método para buscar todos os alunos, com seus cursos associados
        public async Task<List<AlunoModel>> BuscarTodosAlunosAsync()
        {
            try
            {
                // Obtém todos os alunos e seus cursos relacionados
                return await _context.Alunos
                    .Include(a => a.AlunoCurso)
                    .ThenInclude(ac => ac.Curso)
                    .OrderBy(n => n.Nome)
                    .ToListAsync();

             
            }
            catch (Exception ex)
            {
                // Caso ocorra erro, lançamos uma exceção personalizada
                _logger.LogError(ex, "Erro ao buscar todos os alunos.");
                throw new Exception("Erro ao buscar todos os alunos.", ex);
            }
        }

        // Método para verificar se um aluno já está registrado com o mesmo nome e data de nascimento
        public async Task<AlunoModel?> VerificarAlunoExistenteAsync(string nome, DateTime dataNascimento)
        {
            try
            {
                // Verifica se existe um aluno com o nome e a data de nascimento fornecidos
                return await _context.Alunos
                    .FirstOrDefaultAsync(x => x.Nome == nome && x.DataNascimento == dataNascimento);

            }
            catch (Exception ex)
            {
                // Caso ocorra erro, lançamos uma exceção personalizada
                _logger.LogError(ex, "Erro ao verificar a existência do aluno.");
                throw new Exception("Erro ao verificar a existência do aluno.", ex);
            }
        }

        // Método para adicionar um novo aluno
        public async Task AddAlunoAsync(AlunoModel aluno)
        {
            try
            {
                var alunoExistente = await VerificarAlunoExistenteAsync(aluno.Nome, aluno.DataNascimento);

                if (alunoExistente != null)
                {
                    throw new InvalidOperationException($"Já existe um aluno com o mesmo nome e data de nascimento.");
                }

                // Normaliza o nome do aluno, para garantir que esteja em formato de título
                aluno.Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(aluno.Nome);
                aluno.DataCadastro = DateTime.Now;
                aluno.Status = StatusEnum.Ativo;

                // Adiciona o novo aluno ao banco de dados

                _context.Alunos.Add(aluno);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Nenhum aluno adicionado ao banco de dados");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar aluno.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Caso ocorra erro na adição do aluno, lança uma exceção personalizada
                _logger.LogError(ex, "Erro ao adicionar aluno.");   
                throw new Exception("Erro ao adicionar aluno.", ex);
            }
        }

        // Método para atualizar os dados de um aluno existente
        public async Task AtualizarAlunoAsync(AlunoModel aluno)
        {
            try
            {// Busca o aluno existente no banco pelo ID
                var alunoDb = await BuscarAlunoPorIdAsync(aluno.AlunoId);

                if (alunoDb == null)
                {
                    throw new Exception("Aluno não encontrado.");
                }

                // Verifica se já existe um aluno com o mesmo nome e data de nascimento, mas com ID diferente
                var alunoDuplicado = await VerificarAlunoExistenteAsync(aluno.Nome, aluno.DataNascimento);
                if (alunoDuplicado != null && alunoDuplicado.AlunoId != aluno.AlunoId)
                {
                    throw new InvalidOperationException("Já existe um aluno com o mesmo nome e data de nascimento.");
                }

                // Capitaliza o nome do aluno
                alunoDb.Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(aluno.Nome);
                alunoDb.DataNascimento = aluno.DataNascimento;
                alunoDb.Status = aluno.Status;
                alunoDb.Genero = aluno.Genero;

                // Marca as mudanças no banco e salva
                _context.Update(alunoDb);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception($"Nenhum aluno atualizado ao banco de dados");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Caso ocorra erro na atualização, lança uma exceção personalizada
                _logger.LogError(ex, "Erro ao atualizar aluno.");
                throw new Exception("Erro ao atualizar aluno.", ex);
            }
        }

        // Método para remover um aluno do banco de dados pelo ID
        public async Task<bool> RemoverAlunoAsync(int id)
        {
            try
            {
                var alunoDb = await BuscarAlunoPorIdAsync(id);

                if (alunoDb == null)
                {
                    throw new Exception("Aluno não encontrado no banco de dados");
                }

                var cursosDoAluno = await _alunoCursoRepositorio.BuscarCursosDoAlunoAsync(id);
                foreach(var curso in cursosDoAluno)
                {
                    await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(id, curso.CursoId);
                }

                // Remove o aluno do banco de dados
                _context.Alunos.Remove(alunoDb);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Caso ocorra erro na remoção, lança uma exceção personalizada
                _logger.LogError(ex, "Erro ao remover aluno.");
                throw new Exception("Erro ao remover aluno.", ex);
            }
        }

        // Método para contar o total de alunos cadastrados no banco
        public int TotalAlunos()
        {
            try
            {
                // Retorna a contagem total de alunos no banco de dados
                return _context.Alunos.Count();
            }
            catch (Exception ex)
            {
                // Caso ocorra erro na contagem, lança uma exceção personalizada
                _logger.LogError(ex, "Erro ao contar o total de alunos.");
                throw new Exception("Erro ao contar o total de alunos.", ex);
            }
        }

    }
}
