using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas à associação de alunos com cursos,
    // incluindo busca de alunos e cursos, adição e remoção de alunos em cursos
    // - AlunoECursoExistentesAsync(int alunoId, int cursoId) - verifica se existe uma associação entre um aluno e um curso
    // - BuscarAlunosDoCursoAsync(int cursoId) - retorna todos os alunos de um determinado curso
    // - BuscarCursosDoAlunoAsync(int alunoId) - retorna todos os cursos que um determinado aluno está matriculado
    // - BuscarCursosAlunoNaoEstaInscritoAsync(int alunoId) - retorna todos os cursos que um aluno não está matriculado
    // - BuscarAlunosNaoInscritosNoCursoAsync(int cursoId) - retorna todos os alunos que não estão matriculados em um determinado curso
    // - AddAlunoAoCursoAsync(AlunoModel aluno, CursoModel curso) - adiciona um aluno a um curso
    // - RemoverAlunoDoCursoAsync(int alunoId, int cursoId) - remove um aluno de um curso
    public class AlunoCursoRepositorio : IAlunoCursoRepositorio
    {
        private readonly BancoContext _context;


        // Construtor que injeta o contexto do banco de dados
        public AlunoCursoRepositorio(BancoContext context)
        {
            _context = context;
        }

        // Método que verifica se um aluno está matriculado em um curso específico
        public async Task<AlunoCursoModel?> AlunoECursoExistentesAsync(int alunoId, int cursoId)
        {
            try
            {
                // Verifica se existe uma associação entre o aluno e o curso na tabela AlunoCurso
                return await _context.AlunoCurso
                    .FirstOrDefaultAsync(ac => ac.AlunoId == alunoId && ac.CursoId == cursoId);
            }
            catch (Exception ex)
            {
                // Loga a exceção e lança uma nova com mais detalhes
                Console.WriteLine($"Erro: {ex.Message}"); // Substitua por um logger em produção
                throw new Exception("Erro ao verificar se o aluno está matriculado no curso.", ex);
            }
        }

        // Método que busca todos os alunos de um determinado curso
        public async Task<List<AlunoModel>> BuscarAlunosDoCursoAsync(int cursoId)
        {
            try
            {
                // Busca alunos associados ao curso e ordena pelo nome
                var alunos = await _context.AlunoCurso
                    .Where(ac => ac.CursoId == cursoId)
                    .Include(ac => ac.Aluno)  // Inclui os dados do aluno
                    .Select(ac => ac.Aluno)   // Seleciona apenas o aluno
                    .OrderBy(a => a.Nome)
                    .ToListAsync();

                // Verifica se há algum aluno nulo
                if (alunos.Any(a => a == null))
                {
                    throw new InvalidOperationException("A lista de alunos contém alunos nulos.");
                }

                return alunos;
            }
            catch (Exception ex)
            {
                // Lança a exceção, fornecendo mais detalhes sobre o erro
                throw new Exception($"Erro ao buscar os alunos do curso. Detalhes: {ex.Message}", ex);
            }
        }

        // Método que busca todos os cursos de um aluno, incluindo informações do professor
        public async Task<List<CursoModel>> BuscarCursosDoAlunoAsync(int alunoId)
        {
            try
            {
                // Retorna os cursos nos quais o aluno está matriculado, incluindo informações do professor
                var cursos = await _context.AlunoCurso
                    .Where(ac => ac.AlunoId == alunoId)
                    .Include(ac => ac.Curso)
                    .ThenInclude(c => c.Professor)  // Inclui o professor do curso
                    .Select(ac => ac.Curso)
                    .OrderBy(c => c.Idioma)
                    .ToListAsync();

                // Verifica se algum curso está nulo
                if (cursos == null || cursos.Any(c => c == null))
                {
                    throw new InvalidOperationException("A lista de cursos contém cursos nulos.");
                }

                return cursos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os cursos do aluno.", ex);
            }
        }

        // Método que busca todos os cursos em que um aluno ainda não está inscrito
        public async Task<List<CursoModel>> BuscarCursosAlunoNaoEstaInscritoAsync(int alunoId)
        {
            try
            {
                // Obtém os IDs dos cursos nos quais o aluno está inscrito
                var cursosInscritosIds = await _context.AlunoCurso
                    .Where(ac => ac.AlunoId == alunoId)
                    .Select(ac => ac.CursoId)
                    .ToListAsync();

                // Se o aluno não estiver inscrito em nenhum curso, retorna todos os cursos
                if (!cursosInscritosIds.Any())
                {
                    return await _context.Cursos
                        .OrderBy(c => c.Idioma)
                        .ToListAsync();
                }

                // Retorna os cursos que não estão nos cursos nos quais o aluno está inscrito
                return await _context.Cursos
                    .Where(c => !cursosInscritosIds.Contains(c.CursoId))
                    .OrderBy(c => c.Idioma)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar cursos não inscritos para o aluno.", ex);
            }
        }

        // Método que busca todos os alunos que não estão inscritos em um determinado curso
        public async Task<List<AlunoModel>> BuscarAlunosNaoInscritosNoCursoAsync(int cursoId)
        {
            try
            {
                // Obtém os IDs dos alunos inscritos no curso
                var alunosInscritosNoCurso = await _context.AlunoCurso
                    .Where(ac => ac.CursoId == cursoId)
                    .Select(ac => ac.AlunoId)
                    .ToListAsync();

                // Se nenhum aluno estiver inscrito no curso, retorna todos os alunos
                if (!alunosInscritosNoCurso.Any())
                {
                    return await _context.Alunos
                        .OrderBy(a => a.Nome)
                        .ToListAsync();
                }

                // Retorna os alunos que não estão inscritos no curso
                return await _context.Alunos
                    .Where(a => !alunosInscritosNoCurso.Contains(a.AlunoId))
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar alunos não inscritos no curso.", ex);
            }
        }

        // Método que adiciona um aluno a um curso
        public async Task AddAlunoAoCursoAsync(AlunoModel aluno, CursoModel curso)
        {
            try
            {
                const int MAX_CURSOS_POR_ALUNO = 3;

                // Verifica se o curso atingiu o limite de alunos
                if (curso.QuantidadeAlunos >= curso.MaxAlunos)
                {
                    throw new InvalidOperationException($"O curso {curso.Idioma} atingiu o limite máximo de matrículas ({curso.MaxAlunos} alunos).");
                }

                // Verifica se o aluno atingiu o limite de matrículas
                if (aluno.QuantidadeCursos >= MAX_CURSOS_POR_ALUNO)
                {
                    throw new InvalidOperationException($"O aluno {aluno.Nome} já está matriculado no máximo permitido de {MAX_CURSOS_POR_ALUNO} cursos.");
                }

                var cursos = await BuscarCursosDoAlunoAsync(aluno.AlunoId);

                // Verifica se o aluno já está matriculado no curso selecionado
                if (cursos.Any(c => c.CursoId == curso.CursoId))
                {
                    throw new InvalidOperationException($"O aluno {aluno.Nome} já está matriculado no curso {curso.Idioma}.");
                }

                // Verifica se o aluno já está matriculado em um curso do mesmo idioma
                if (cursos.Any(curso => curso.Idioma.Equals(curso.Idioma, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"O aluno {aluno.Nome} já está matriculado em um curso de {curso.Idioma} e não pode se matricular em outro do mesmo idioma.");
                }

                // Adiciona o aluno ao curso
                var alunoCurso = new AlunoCursoModel
                {
                    AlunoId = aluno.AlunoId,
                    CursoId = curso.CursoId
                };

                _context.AlunoCurso.Add(alunoCurso);
                var result = await _context.SaveChangesAsync();

                // Verifica se a operação foi concluída com sucesso
                if (result <= 0)
                {
                    throw new InvalidOperationException("Erro ao adicionar aluno ao curso. Nenhuma alteração foi realizada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao adicionar aluno ao curso: {ex.Message}", ex);
            }
        }


        // Método que remove um aluno de um curso
        public async Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId)
        {
            try
            {
                var alunoCurso = await AlunoECursoExistentesAsync(alunoId, cursoId);

                if (alunoCurso == null)
                {
                    throw new InvalidOperationException("O aluno não está matriculado neste curso.");
                }

                // Remove o aluno do curso
                _context.AlunoCurso.Remove(alunoCurso);
                var result = await _context.SaveChangesAsync();

                // Verifica se nenhum aluno foi removido
                if (result <= 0)
                {
                    throw new Exception("Nenhum aluno removido do curso.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Nenhum aluno foi removido do curso. Verifique o banco de dados.", ex);
            }
        }
    }
}
