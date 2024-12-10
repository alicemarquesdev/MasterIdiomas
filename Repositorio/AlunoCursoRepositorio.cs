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
            try
            {
                // Retorna a lista de alunos matriculados no curso especificado
                return await _context.AlunoCurso
                    .Where(ac => ac.CursoId == cursoId)
                    .Select(ac => ac.Aluno)
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar os alunos do curso.");
            }
        }

        public async Task<List<CursoModel>> BuscarCursosDoAlunoAsync(int alunoId)
        {
            try
            {
                // Retorna os cursos nos quais o aluno está matriculado, incluindo informações do professor
                return await _context.AlunoCurso
                    .Where(ac => ac.AlunoId == alunoId)
                    .Include(ac => ac.Curso.Professor)
                    .Select(ac => ac.Curso)
                    .OrderBy(c => c.Idioma)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar os cursos do aluno.");
            }
        }

        public async Task<List<CursoModel>> BuscarCursosAlunoNaoInscritoAsync(int alunoId)
        {
            try
            {
                // Obtém os IDs dos cursos nos quais o aluno está inscrito
                var cursosInscritosIds = await _context.AlunoCurso
                    .Where(ac => ac.AlunoId == alunoId)
                    .Select(ac => ac.CursoId)
                    .ToListAsync();

                // Retorna os cursos que não estão nos cursos nos quais o aluno está inscrito
                return await _context.Cursos
                    .Where(c => !cursosInscritosIds.Contains(c.CursoId))
                    .OrderBy(c => c.Idioma)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar cursos não inscritos para o aluno.");
            }
        }

        public async Task<List<AlunoModel>> BuscarAlunosPorIdiomaAsync(CursoModel curso)
        {
            try
            {
                // Retorna os alunos matriculados no curso que possuem o mesmo idioma
                return await _context.AlunoCurso
                    .Where(x => x.Curso.Idioma == curso.Idioma)
                    .Select(x => x.Aluno)
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar alunos por idioma.");
            }
        }

        public async Task<List<AlunoModel>> BuscarAlunosNaoInscritosNoCurso(int cursoId)
        {
            try
            {
                // Obtém os IDs dos alunos inscritos no
                var alunosInscritosNoCurso = await _context.AlunoCurso
                    .Where(ac => ac.CursoId == cursoId)
                    .Select(ac => ac.AlunoId)
                    .ToListAsync();

                // Retorna os alunos que não estão nos curso
                return await _context.Alunos
                    .Where(a => !alunosInscritosNoCurso.Contains(a.AlunoId))
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao buscar alunos não inscritos no curso.");
            }
        }

        public async Task AddAlunoAoCursoAsync(int alunoId, int cursoId)
        {
            const int maximoCursosPorAluno = 3;
            const int maximoAlunosPorCurso = 30;
            try
            {
                // Obtém os cursos nos quais o aluno já está matriculado
                List<CursoModel> cursosDoAluno = await BuscarCursosDoAlunoAsync(cursoId);

                // Verifica o limite máximo de cursos permitidos por aluno
                if (cursosDoAluno.Count >= maximoCursosPorAluno)
                {
                    throw new InvalidOperationException("O aluno atingiu o limite máximo de matrículas permitido (3 cursos).");
                }
                var cursoAtual = await _context.AlunoCurso.FirstOrDefaultAsync(x => x.CursoId == cursoId);

                if (cursosDoAluno.Any(curso => curso.Idioma.Equals(cursoAtual.Curso.Idioma, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"O aluno já está matriculado em um curso de {cursoAtual.Curso.Idioma} e não pode se matricular em outro do mesmo idioma.");
                }

                var TotalAlunosDoCurso = await TotalAlunosCurso(cursoId);
                if (TotalAlunosDoCurso >= maximoAlunosPorCurso)
                {
                    throw new InvalidOperationException("O curso atingiu o limite máximo de matrículas permitido (30 alunos).");
                }

                // Cria a relação aluno-curso
                var alunoCurso = new AlunoCursoModel
                {
                    AlunoId = alunoId,
                    CursoId = cursoId
                };

                await _context.AlunoCurso.AddAsync(alunoCurso);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId)
        {
            try
            {
                // Encontra a relação aluno-curso
                var alunoCurso = await _context.AlunoCurso
                    .FirstOrDefaultAsync(ac => ac.CursoId == cursoId && ac.AlunoId == alunoId);

                if (alunoCurso != null)
                {
                    _context.AlunoCurso.Remove(alunoCurso);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("A relação aluno-curso não foi encontrada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task<int> TotalAlunosCurso(int cursoId)
        {
            return await _context.AlunoCurso.CountAsync(ac => ac.CursoId == cursoId);
        }
    }
}