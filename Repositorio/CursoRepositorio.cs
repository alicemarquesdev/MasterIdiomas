using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    public class CursoRepositorio : ICursoRepositorio
    {
        private readonly BancoContext _context;
        private readonly IAlunoCursoRepositorio _alunoCursoRepositorio;

        // Construtor que recebe o contexto do banco e o repositório de professores
        public CursoRepositorio(BancoContext context,
                                IAlunoCursoRepositorio alunoCursoRepositorio)
        {
            _context = context;
            _alunoCursoRepositorio = alunoCursoRepositorio;
        }

        // Buscar curso por ID, incluindo informações do professor e alunos
        public async Task<CursoModel> BuscarCursoPorIdAsync(int id)
        {
            try
            {
                return await _context.Cursos
                    .Include(c => c.Professor)
                    .Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno)
                    .FirstOrDefaultAsync(x => x.CursoId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar o curso. Tente novamente.", ex);
            }
        }

        // Buscar todos os idiomas distintos
        public async Task<List<CursoModel>> BuscarIdiomasAsync()
        {
            try
            {
                var idiomasDistintos = await _context.Cursos
                   .Select(i => i.Idioma)
                   .Distinct()
                   .ToListAsync();

                // Criar uma lista de CursoModel a partir dos idiomas distintos
                var cursosModelList = idiomasDistintos.Select(idioma => new CursoModel
                {
                    Idioma = idioma // Supondo que CursoModel tem uma propriedade chamada Idioma
                }).ToList();

                return cursosModelList;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os idiomas. Tente novamente.", ex);
            }
        }

        // Buscar todos os cursos com informações de alunos e professores
        public async Task<List<CursoModel>> BuscarTodosCursosAsync()
        {
            try
            {
                return await _context.Cursos
                    .Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno)
                    .Include(c => c.Professor)
                    .OrderBy(x => x.Idioma)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os cursos. Tente novamente.", ex);
            }
        }

        // Verificar se existe um curso com as mesmas características, mas ignorando um ID de curso
        public async Task<CursoModel> BuscarCursoExistenteAsync(string idioma, string turno, string nivel, int cursoIdIgnorar)
        {
            try
            {
                return await _context.Cursos.FirstOrDefaultAsync(x => x.Idioma == idioma && x.Turno == turno && x.Nivel == nivel && x.CursoId != cursoIdIgnorar);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao verificar a existência do curso. Tente novamente.", ex);
            }
        }

        // Retornar a quantidade de cursos em andamento
        public int CursosEmAndamento()
        {
            try
            {
                return _context.Cursos
                    .Where(x => x.Status == StatusCursoEnum.EmAndamento)
                    .Select(c => c.Idioma)
                    .Distinct()
                    .Count();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao contar os cursos em andamento. Tente novamente.", ex);
            }
        }

        // Retornar o total de cursos cadastrados
        public int TotalCursos()
        {
            try
            {
                return _context.Cursos.Count();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao contar o total de cursos. Tente novamente.", ex);
            }
        }

        // Retornar o total de idiomas cadastrados
        public int TotalIdiomas()
        {
            try
            {
                return _context.Cursos.Select(i => i.Idioma).Distinct().Count();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao contar os idiomas. Tente novamente.", ex);
            }
        }

        // Adicionar um novo curso ao banco de dados
        public async Task AddCursoAsync(CursoModel curso)
        {
            const int numeroMaxDeCursos = 10;
            try
            {
                var cursoExistente = await BuscarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
                if (cursoExistente != null)
                {
                    throw new Exception("Já existe um curso registrado com o mesmo idioma, turno e nível.");
                }

                var idiomasExistentes = await BuscarIdiomasAsync();
                if (idiomasExistentes.Count >= numeroMaxDeCursos)
                {
                    throw new Exception($"Não é possível criar mais de {numeroMaxDeCursos} cursos de idiomas diferentes.");
                }

                curso.Status = StatusCursoEnum.EmAndamento; // Definir o status como "Em Andamento"
                await _context.Cursos.AddAsync(curso);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar o curso. Tente novamente.", ex);
            }
        }

        // Atualizar as informações de um curso existente
        public async Task AtualizarCursoAsync(CursoModel curso)
        {
            const int numeroMaxDeCursos = 10;

            try
            {
                var cursoDb = await BuscarCursoPorIdAsync(curso.CursoId);
                if (cursoDb == null)
                    throw new Exception("Curso não encontrado. Verifique os dados e tente novamente.");

                // Verificar se existe um curso com o mesmo idioma, turno e nível
                var cursoDuplicado = await BuscarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
                if (cursoDuplicado != null)
                    throw new Exception("Já existe um curso registrado com o mesmo idioma, turno e nível.");

                // Verificar a quantidade de cursos em andamento antes de atualizar o status
                var idiomasExistentes = await BuscarIdiomasAsync();
                if (idiomasExistentes.Count >= numeroMaxDeCursos)
                {
                    throw new Exception($"Não é possível criar mais de {numeroMaxDeCursos} cursos de idiomas diferentes.");
                }

                // Atualizar os dados do curso
                cursoDb.Idioma = curso.Idioma;
                cursoDb.Turno = curso.Turno;
                cursoDb.Nivel = curso.Nivel;
                cursoDb.DataInicio = curso.DataInicio;
                cursoDb.CargaHoraria = curso.CargaHoraria;
                cursoDb.MaxAlunos = curso.MaxAlunos;
                cursoDb.DataAtualizacao = DateTime.Now;
                cursoDb.Status = curso.Status;

                if (cursoDb.Status == StatusCursoEnum.Cancelado)
                {
                    // Buscar alunos associados ao curso
                    List<AlunoModel> alunosDoCurso = await _alunoCursoRepositorio.BuscarAlunosDoCursoAsync(curso.CursoId);

                    if (alunosDoCurso != null && alunosDoCurso.Any())
                    {
                        foreach (var aluno in alunosDoCurso)
                        {
                            await _alunoCursoRepositorio.RemoverAlunoDoCursoAsync(aluno.AlunoId, curso.CursoId);
                        }
                    }
                }

                _context.Cursos.Update(cursoDb);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o curso. Tente novamente.", ex);
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
                if (cursoDb.Status == StatusCursoEnum.EmAndamento && cursosEmAndamento == 1)
                {
                    throw new Exception("Limites do sistema foram atingidos. Não é possível excluir o curso.");
                }

                _context.Cursos.Remove(cursoDb);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover o curso. Tente novamente.", ex);
            }
        }
    }
}