using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas aos cursos, incluindo busca de cursos, adição e remoção
    // - BuscarIdiomasAsync() - retorna uma lista com todos os idiomas cadastrados nos cursos
    // - BuscarCursoPorIdAsync(int id) - busca um curso específico pelo seu ID, incluindo informações do professor e alunos
    // - BuscarTodosCursosAsync() - retorna todos os cursos, com informações de alunos e professores
    // - BuscarCursosPorIdiomaAsync(string idioma) - busca todos os cursos de um idioma específico
    // - BuscarCursosBarraDePesquisaAsync(string termo) - permite buscar cursos com base em um termo específico na barra de pesquisa
    // - VerificarCursoExistenteAsync(string idioma, TurnoEnum turno, NivelEnum nivel) - verifica se já existe um curso com o mesmo idioma, turno e nível
    // - AddCursoAsync(CursoModel curso) - adiciona um novo curso ao banco de dados
    // - AtualizarCursoAsync(CursoModel curso) - atualiza os dados de um curso, incluindo o cancelamento de alunos associados, se necessário
    // - RemoverCursoAsync(int id) - remove um curso do banco de dados
    // - CursosEmAndamento() - retorna a quantidade de cursos com o status "Em Andamento"
    // - TotalCursos() - retorna o total de cursos cadastrados no banco
    // - TotalIdiomas() - retorna o total de idiomas distintos cadastrados no banco

    public class CursoRepositorio : ICursoRepositorio
    {
        private readonly BancoContext _context;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;
        private readonly IProfessorCursoRepositorio _professorCursoRepositorio;
        private readonly ILogger<CursoRepositorio> _logger;

        // Construtor que recebe o contexto do banco e o repositório de alunos associados a cursos
        public CursoRepositorio(BancoContext context,
                                IAlunoCursoRepositorio alunoCursoRepositorio,
                                IProfessorCursoRepositorio professorCursoRepositorio,
                                ILogger<CursoRepositorio> logger)
        {
            _context = context;
            _alunoCursoRepositorio = alunoCursoRepositorio;
            _professorCursoRepositorio = professorCursoRepositorio;
            _logger = logger;
        }

        // Buscar todos os idiomas cadastrados no banco de dados
        public async Task<List<string>> BuscarIdiomasAsync()
        {
            try
            {
                // Retorna todos os idiomas distintos dos cursos cadastrados
                return await _context.Cursos.Select(x => x.Idioma).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao buscar os idiomas.");
                throw new Exception("Erro ao buscar os idiomas.", ex);
            }
        }

        // Buscar curso por ID, incluindo informações do professor e alunos
        public async Task<CursoModel?> BuscarCursoPorIdAsync(int id)
        {
            try
            {
                // Busca o curso pelo ID, incluindo informações do professor
                var curso = await _context.Cursos
                    .Include(c => c.Professor)
                    .FirstOrDefaultAsync(x => x.CursoId == id);

                return curso;
            }
            catch (Exception ex)
            {
                // Lança uma exceção detalhada
                _logger.LogError(ex, "Erro ao buscar o curso.");
                throw new Exception("Erro ao buscar o curso.", ex);
            }
        }


        // Buscar todos os cursos com informações de alunos e professores
        public async Task<List<CursoModel>> BuscarTodosCursosAsync()
        {
            try
            {
                // Retorna todos os cursos, incluindo alunos e professores, ordenados pelo idioma
                return await _context.Cursos
                    .Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno)
                    .Include(c => c.Professor)
                    .OrderBy(x => x.Idioma)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao buscar os cursos.");
                throw new Exception("Erro ao buscar os cursos.", ex);
            }
        }

        // Buscar cursos de um idioma específico
        public async Task<List<CursoModel>> BuscarCursosPorIdiomaAsync(string idioma)
        {
            try
            {
                // Busca todos os cursos de um idioma específico
                return await _context.Cursos
                    .Where(x => x.Idioma == idioma)
                    .Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno)
                    .Include(c => c.Professor)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar os cursos.");
                throw new Exception("Erro ao buscar os cursos.", ex);
            }
        }

        // Buscar cursos usando a barra de pesquisa
        public async Task<List<CursoModel>> BuscarCursosBarraDePesquisaAsync(string termo)
        {
            try
            {
                // Filtra somente pelo campo Idioma
                return await _context.Cursos
                    .Where(x => x.Idioma.Contains(termo))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao buscar os cursos.");
                throw new Exception("Erro ao buscar os cursos.", ex);
            }
        }

        // Verificar se existe um curso com as mesmas características
        public async Task<bool> VerificarCursoExistenteAsync(string idioma, TurnoEnum turno, NivelEnum nivel, int cursoId)
        {
            try
            {
                // Verifica se já existe um curso com o mesmo idioma, turno e nível
               return await _context.Cursos
                    .AnyAsync(x => x.Idioma == idioma && x.Turno == turno && x.Nivel == nivel && x.CursoId != cursoId);

            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao verificar a existência do curso.");
                throw new Exception("Erro ao verificar a existência do curso.", ex);
            }
        }

        // Adicionar um novo curso ao banco de dados
        public async Task AddCursoAsync(CursoModel curso)
        {
            try
            {
                // Definindo o número máximo de idiomas diferentes permitidos para cursos
                const int MAX_IDIOMAS_PARA_CURSOS = 12;

                // Verifica se já existe um curso com o mesmo idioma, turno e nível
                var cursoExistente = await VerificarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
                if (cursoExistente)
                {
                    // Se um curso já existir, lança uma exceção
                    throw new InvalidOperationException("Já existe um curso registrado com o mesmo idioma, turno e nível.");
                }

                // Verifica se o número de idiomas já cadastrados é maior ou igual ao limite
                var idiomasExistentes = await BuscarIdiomasAsync();
                if (idiomasExistentes.Count >= MAX_IDIOMAS_PARA_CURSOS)
                {
                    // Lança uma exceção se o limite de idiomas for atingido
                    throw new InvalidOperationException($"Não é possível criar mais de {MAX_IDIOMAS_PARA_CURSOS} cursos de idiomas diferentes.");
                }

                // Define o status do curso como "Em Andamento" antes de adicionar ao banco de dados
                curso.Status = StatusCursoEnum.Ativo;

                // Adiciona o curso ao contexto do banco de dados
                await _context.Cursos.AddAsync(curso);
                // Salva as mudanças no banco de dados
                var result = await _context.SaveChangesAsync();

                // Verifica se o curso foi adicionado com sucesso
                if (result <= 0)
                {
                    // Se a operação de inserção falhar, lança uma exceção
                    throw new Exception("Erro ao adicionar o curso. Tente novamente.");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Lança uma exceção com a mensagem de erro, incluindo a exceção original
                _logger.LogError(ex, "Erro ao adicionar o curso.");
                throw new Exception("Erro ao adicionar o curso.", ex);
            }
        }

        // Atualizar um curso existente
        public async Task AtualizarCursoAsync(CursoModel curso)
        {
            try
            {
                // Define o número máximo de idiomas permitidos para cursos
                const int MAX_IDIOMAS_PARA_CURSOS = 12;

                // Busca o curso existente no banco de dados
                var cursoDb = await BuscarCursoPorIdAsync(curso.CursoId);
                // Se o curso não for encontrado, lança uma exceção
                if (cursoDb == null)
                    throw new Exception("Curso não encontrado. Verifique os dados e tente novamente.");

                // Verifica se já existe um curso com o mesmo idioma, turno e nível
                var cursoDuplicado = await VerificarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
                if (cursoDuplicado)
                    throw new InvalidOperationException("Já existe um curso registrado com o mesmo idioma, turno e nível.");

                // Verifica se o número de idiomas já cadastrados é maior ou igual ao limite
                var idiomasExistentes = await BuscarIdiomasAsync();
                if (idiomasExistentes.Count > MAX_IDIOMAS_PARA_CURSOS)
                {
                    // Lança uma exceção se o limite de idiomas for atingido
                    throw new InvalidOperationException($"Não é possível criar mais de {MAX_IDIOMAS_PARA_CURSOS} cursos de idiomas diferentes.");
                }

                // Atualiza os dados do curso no banco de dados
                cursoDb.Idioma = curso.Idioma;
                cursoDb.Turno = curso.Turno;
                cursoDb.Nivel = curso.Nivel;
                cursoDb.DataInicio = curso.DataInicio;
                cursoDb.CargaHoraria = curso.CargaHoraria;
                cursoDb.MaxAlunos = curso.MaxAlunos;
                cursoDb.DataAtualizacao = DateTime.Now;
                cursoDb.Status = curso.Status;

                // Atualiza o curso no banco de dados
                _context.Cursos.Update(cursoDb);
                var result = await _context.SaveChangesAsync();

                // Verifica se a atualização foi bem-sucedida
                if (result <= 0)
                {
                    // Se a atualização não afetar nenhuma linha, lança uma exceção
                    throw new Exception("Nenhuma alteração no banco de dados");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Lança uma exceção com a mensagem de erro caso ocorra alguma falha durante a atualização
                _logger.LogError(ex, "Erro ao atualizar o curso.");
                throw new Exception("Erro ao atualizar o curso.", ex);
            }
        }


        // Remover um curso do banco de dados
        public async Task<bool> RemoverCursoAsync(int id)
        {
            try
            {
                var cursoDb = await BuscarCursoPorIdAsync(id);
                if (cursoDb == null)
                    throw new Exception("Curso não encontrado. Verifique os dados e tente novamente.");

                // Verificar a quantidade de cursos em andamento antes de remover
                var cursosEmAndamento = CursosEmAndamento();
                if (cursoDb.Status == StatusCursoEnum.Ativo && cursosEmAndamento == 1)
                {
                    throw new InvalidOperationException("Limites do sistema foram atingidos. Não é possível excluir o curso.");
                }

                var alunosDoCurso = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(id);
                foreach(var aluno in alunosDoCurso)
                {
                    await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(aluno.AlunoId, id);
                }

                await _professorCursoRepositorio.RemoverProfessorDoCursoAsync(cursoDb);

                // Remove o curso do banco de dados
                _context.Cursos.Remove(cursoDb);
                var result = await _context.SaveChangesAsync();

                // Verifica se a remoção foi bem-sucedida
                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados");
                }

                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao remover o curso.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao remover o curso.");
                throw new Exception("Erro ao remover o curso.", ex);
            }
        }

        // Retorna a quantidade de cursos em andamento
        public int CursosEmAndamento()
        {
            try
            {
                // Conta os cursos com o status "Em Andamento"
                return _context.Cursos
                    .Where(x => x.Status == StatusCursoEnum.Ativo)
                    .Count();
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao contar os cursos em andamento.");
                throw new Exception("Erro ao contar os cursos em andamento.", ex);
            }
        }

        // Retorna o total de cursos cadastrados
        public int TotalCursos()
        {
            try
            {
                // Conta o número total de cursos cadastrados
                return _context.Cursos.Count();
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao contar o total de cursos.");
                throw new Exception("Erro ao contar o total de cursos.", ex);
            }
        }

        // Retorna o total de idiomas cadastrados
        public int TotalIdiomas()
        {
            try
            {
                // Conta o número total de idiomas distintos
                return _context.Cursos.Select(i => i.Idioma).Distinct().Count();
            }
            catch (Exception ex)
            {
                // Lança uma exceção caso ocorra erro
                _logger.LogError(ex, "Erro ao contar os idiomas.");
                throw new Exception("Erro ao contar os idiomas.", ex);
            }
        }
    }
}
