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
        private readonly ILogger<AlunoCursoRepositorio> _logger;


        // Construtor que injeta o contexto do banco de dados
        public AlunoCursoRepositorio(BancoContext context, ILogger<AlunoCursoRepositorio> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método que verifica se um aluno está matriculado em um curso específico
        public async Task<AlunoCursoModel?> AlunoECursoExistentesAsync(int alunoId, int cursoId)
        {
            try
            {
                // Verifica se existe uma associação entre o aluno e o curso na tabela AlunoCurso
                return await _context.AlunoCurso
                    .Include(x => x.Curso)
                    .Include(x => x.Aluno)
                    .FirstOrDefaultAsync(ac => ac.AlunoId == alunoId && ac.CursoId == cursoId);
            }
            catch (Exception ex)
            {
                // Loga a exceção e lança uma nova com mais detalhes
                _logger.LogError(ex, "Erro ao verificar se o aluno está matriculado no curso.");
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
                _logger.LogError(ex, "Erro ao buscar os alunos do curso.");
                throw new Exception("Erro ao buscar os alunos do curso.", ex);
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
                _logger.LogError(ex, "Erro ao buscar os cursos do aluno.");
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
                        .Where(c => c.QuantidadeAlunos < c.MaxAlunos && c.Status == Enums.StatusCursoEnum.Ativo)
                        .OrderBy(c => c.Idioma)
                        .ToListAsync();
                }

                // Retorna os cursos que não estão nos cursos nos quais o aluno está inscrito, que não atingiram o limite de alunos e estão em andamento
                return await _context.Cursos
                    .Where(c => !cursosInscritosIds.Contains(c.CursoId) && c.QuantidadeAlunos < c.MaxAlunos && c.Status == Enums.StatusCursoEnum.Ativo)
                    .OrderBy(c => c.Idioma)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cursos não inscritos para o aluno.");
                throw new Exception("Erro ao buscar cursos não inscritos para o aluno.", ex);
            }
        }

        // Método que busca todos os alunos que não estão inscritos em um determinado curso
        public async Task<List<AlunoModel>> BuscarAlunosNaoInscritosNoCursoAsync(int cursoId)
        {
            try
            {
                var curso = await _context.Cursos.FirstOrDefaultAsync(x => x.CursoId == cursoId);

                // Obtém os IDs dos alunos inscritos no curso
                var alunosInscritosNoCurso = await _context.AlunoCurso
                    .Where(ac => ac.CursoId == cursoId || ac.Curso.Idioma == curso.Idioma) // Não retornar alunos que estão incritos no curso com memsmo idioma.
                    .Select(ac => ac.AlunoId)
                    .ToListAsync();

                // Se nenhum aluno estiver inscrito no curso, retorna todos os alunos
                if (!alunosInscritosNoCurso.Any())
                {
                    return await _context.Alunos
                        .Where(a => a.QuantidadeCursos < 3 && a.Status == Enums.StatusEnum.Ativo)
                        .OrderBy(a => a.Nome)
                        .ToListAsync();
                }

                // Retorna os alunos que não estão inscritos no curso
                return await _context.Alunos
                    .Where(a => !alunosInscritosNoCurso.Contains(a.AlunoId) && a.QuantidadeCursos < 3 && a.Status == Enums.StatusEnum.Ativo)
                    .OrderBy(a => a.Nome)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar alunos não inscritos no curso.");
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
                if (cursos.Any(x => x.Idioma.Equals(curso.Idioma, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"O aluno {aluno.Nome} já está matriculado em um curso de {curso.Idioma} e não pode se matricular em outro do mesmo idioma.");
                }

                // Adiciona o aluno ao curso
                var alunoCurso = new AlunoCursoModel
                {
                    AlunoId = aluno.AlunoId,
                    CursoId = curso.CursoId
                };

                aluno.QuantidadeCursos++;
                curso.QuantidadeAlunos++;

                _context.AlunoCurso.Add(alunoCurso);
                var result = await _context.SaveChangesAsync();

                // Verifica se a operação foi concluída com sucesso
                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração foi realizada no banco de dados.");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar aluno ao curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar aluno ao curso.");
                throw new Exception("Desculpe. Houve um erro ao adicionar o aluno ao curso.", ex);
            }
        }


        // Método que remove um aluno de um curso
        public async Task RemoverAlunoDoCursoAsync(int alunoId, int cursoId)
        {
            try
            {
                // Verifica se o aluno está matriculado no curso
                var alunoCurso = await AlunoECursoExistentesAsync(alunoId, cursoId);

                if (alunoCurso == null)
                {
                    throw new InvalidOperationException("O aluno não está matriculado neste curso.");
                }

                // Atualiza as quantidades de alunos e cursos diretamente no contexto
                var cursoDb = alunoCurso.Curso; // Acessando o curso relacionado ao aluno
                var alunoDb = alunoCurso.Aluno; // Acessando o aluno relacionado ao curso

                // Garante que as propriedades não sejam null antes de decrementar
                cursoDb.QuantidadeAlunos = cursoDb.QuantidadeAlunos > 0 ? cursoDb.QuantidadeAlunos - 1 : 0;
                alunoDb.QuantidadeCursos = alunoDb.QuantidadeCursos > 0 ? alunoDb.QuantidadeCursos - 1 : 0;


                // Remove a associação entre o aluno e o curso
                _context.AlunoCurso.Remove(alunoCurso);

                // Salva as alterações no banco de dados
                var result = await _context.SaveChangesAsync();

                // Verifica se nenhuma alteração foi realizada
                if (result <= 0)
                {
                    throw new Exception("Nenhum aluno foi removido do curso.");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao remover aluno do curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Lança uma exceção detalhada com a mensagem do erro
                _logger.LogError(ex, "Erro ao remover aluno do curso.");
                throw new Exception("Erro ao remover aluno do curso.", ex);
            }
        }

    }
}
