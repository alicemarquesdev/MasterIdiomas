using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    public class AlunoCursoRepositorio : IAlunoCursoRepositorio
    {
        private readonly BancoContext _context;

        public AlunoCursoRepositorio(BancoContext context)
        {
            _context = context;
        }

        public async Task<List<AlunoModel>> BuscarAlunosDoCursoAsync(int cursoId)
        {
            return await _context.AlunoCurso
                         .Where(ac => ac.CursoId == cursoId)
                         .Select(ac => ac.Aluno)
                         .ToListAsync();
        }
    

        public async Task<List<CursoModel>> BuscarCursosDoAlunoAsync(int alunoId)
        {
            return await _context.AlunoCurso.Where(ac => ac.AlunoId == alunoId).Include(ac => ac.Curso.Professor)
                .Select(ac => ac.Curso).OrderBy(c => c.Idioma).ToListAsync();
        }

        public async Task<List<CursoModel>> BuscarCursosAlunoNaoInscritoAsync(int alunoId)
        {
            
                // Obtém os IDs dos cursos nos quais o aluno está inscrito
                var cursosInscritosIds = await _context.AlunoCurso
                    .Where(ac => ac.AlunoId == alunoId)
                    .Select(ac => ac.CursoId)
                    .ToListAsync();

                // Retorna os cursos que não estão nos cursos inscritos
                var cursosNaoInscritos = await _context.Cursos
                    .Where(c => !cursosInscritosIds.Contains(c.CursoId)) // Exclui os cursos já inscritos
                    .ToListAsync();

                return cursosNaoInscritos;
            
        }

        public async Task<List<AlunoModel>> BuscarAlunosPorIdiomaAsync(CursoModel curso)
        {
            return await _context.AlunoCurso.Where(x => x.Curso.Idioma == curso.Idioma).Select(x => x.Aluno).ToListAsync();
        }

        public async Task AddAlunoAoCursoAsync(int alunoId, int cursoId)
        {
            var alunoExiste = await _context.Alunos.AnyAsync(a => a.AlunoId == alunoId);
            var cursoExiste = await _context.Cursos.AnyAsync(c => c.CursoId == cursoId);

            if (!alunoExiste || !cursoExiste)
                throw new ArgumentException("Aluno ou Curso não encontrado.");

            var alunoCurso = new AlunoCursoModel
            {
                AlunoId = alunoId,
                CursoId = cursoId
            };

            _context.AlunoCurso.Add(alunoCurso);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId)
        {
            var alunoCurso = await _context.AlunoCurso.FirstOrDefaultAsync(ac => ac.CursoId == cursoId && ac.AlunoId == alunoId);

            if (alunoCurso != null)
            {
                _context.AlunoCurso.Remove(alunoCurso);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> TotalAlunosCurso(int cursoId)
        {
            var alunos = await BuscarAlunosDoCursoAsync(cursoId);
            return alunos.Count();
        }
    }
}