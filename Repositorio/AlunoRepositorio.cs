using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Alunos.Include(a => a.AlunoCurso).ThenInclude(a => a.Curso).OrderBy(n => n.Nome).ToListAsync();
        }

        public async Task<AlunoModel> BuscarAlunoPorIdAsync(int id)
        {
            return await _context.Alunos.FirstOrDefaultAsync(x => x.AlunoId == id);
        }

        public async Task<AlunoModel> BuscarAlunoExistenteAsync(string nome)
        {
            return await _context.Alunos.FirstOrDefaultAsync(x => x.Nome == nome);
        }

        public int TotalAlunos()
        {
            return _context.Alunos.Count();
        }

        public async Task AddAlunoAsync(AlunoModel aluno)
        {
            aluno.DataCadastro = DateTime.Now;
            aluno.Status = StatusEnum.Ativo;
            await _context.Alunos.AddAsync(aluno);
            await _context.SaveChangesAsync();
        }

        public async Task AddAlunoExistenteCursoExistenteAync(int alunoId, int cursoId)
        {
            var alunoExistente = _context.Alunos.Find(alunoId);
            var cursoExistente = _context.Cursos.Find(cursoId);

            if ((cursoExistente == null) || (alunoExistente == null))
                throw new Exception("Não conseguimos encontrar os registros.");

            var inscricaoExistente = _context.AlunoCurso.FirstOrDefault(ac => ac.AlunoId == alunoId && ac.CursoId == cursoId);
            if (inscricaoExistente != null)
                throw new Exception("O aluno já está inscrito neste curso.");

            // Criar uma nova inscrição
            var novaInscricao = new AlunoCursoModel
            {
                AlunoId = alunoId,
                CursoId = cursoId
            };

            // Adicionar ao contexto e salvar as mudanças
            _context.AlunoCurso.Add(novaInscricao);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAlunoAsync(AlunoModel aluno)
        {
            AlunoModel alunoDB = await BuscarAlunoPorIdAsync(aluno.AlunoId);

            if (alunoDB == null) throw new Exception("Não foi possível encontrar os dados do aluno para atualização.");

            alunoDB.Nome = aluno.Nome;
            alunoDB.DataNascimento = aluno.DataNascimento;
            alunoDB.Status = aluno.Status;
            alunoDB.Genero = aluno.Genero;

            _context.Update(alunoDB);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoverAlunoAsync(int id)
        {
            AlunoModel alunoDb = await BuscarAlunoPorIdAsync(id);
            if (alunoDb == null) throw new Exception("Registro não encontrado. Verifique se o aluno existe e tente novamente");

            _context.Alunos.Remove(alunoDb);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}