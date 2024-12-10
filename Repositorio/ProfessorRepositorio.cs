using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    public class ProfessorRepositorio : IProfessorRepositorio
    {
        private readonly BancoContext _context;
        private readonly ICursoRepositorio _cursoRepositorio;

        public ProfessorRepositorio(BancoContext context,
                                    ICursoRepositorio cursoRepositorio)
        {
            _context = context;
            _cursoRepositorio = cursoRepositorio;
        }

        // Buscar todos os cursos associados a um professor
        public async Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId)
        {
            try
            {
                return await _context.Cursos
                    .Include(c => c.Professor)
                    .Where(c => c.ProfessorId == professorId)
                    .OrderBy(a => a.Idioma)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Mensagem de erro amigável
                throw new Exception("Ocorreu um erro ao buscar os cursos do professor. Tente novamente mais tarde.", ex);
            }
        }

        // Verificar se o professor já existe
        public async Task<ProfessorModel> BuscarProfessorExistenteAsync(string email, string nome, int professorIdIgnorar)
        {
            try
            {
                return await _context.Professores
                    .FirstOrDefaultAsync(x => x.Email == email && x.Nome == nome && x.ProfessorId != professorIdIgnorar);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao verificar a existência do professor. Tente novamente mais tarde.", ex);
            }
        }

        // Buscar professor por ID
        public async Task<ProfessorModel> BuscarProfessorPorIdAsync(int id)
        {
            try
            {
                return await _context.Professores
                    .FirstOrDefaultAsync(x => x.ProfessorId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao buscar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Buscar todos os professores
        public async Task<List<ProfessorModel>> BuscarTodosProfessoresAsync()
        {
            try
            {
                return await _context.Professores.OrderBy(p => p.Nome).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar os professores. Tente novamente mais tarde.", ex);
            }
        }

        // Contagem total de professores
        public int TotalProfessores()
        {
            try
            {
                return _context.Professores.Count();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao contar os professores. Tente novamente mais tarde.", ex);
            }
        }

        // Adicionar um novo professor
        public async Task AddProfessorAsync(ProfessorModel professor)
        {
            try
            {
                // Verificar se o professor já existe com base no e-mail e nome
                var professorExistente = await BuscarProfessorExistenteAsync(professor.Email, professor.Nome, professor.ProfessorId);

                if (professorExistente != null)
                {
                    throw new Exception("Já existe um professor registrado com este e-mail.");
                }

                // Definir valores padrão para campos obrigatórios
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                professor.Nome = textInfo.ToTitleCase(professor.Nome.ToLower());
                professor.Email = professor.Email.ToLower();
                professor.DataCadastro = DateTime.Now;
                professor.Status = StatusEnum.Ativo;

                // Adicionar o professor ao banco de dados
                await _context.Professores.AddAsync(professor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao adicionar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Atualizar dados de um professor
        public async Task AtualizarProfessorAsync(ProfessorModel professor)
        {
            if (professor == null)
            {
                throw new ArgumentNullException(nameof(professor), "Os dados do professor não foram fornecidos.");
            }

            try
            {
                // Buscar professor no banco
                var professorDB = await BuscarProfessorPorIdAsync(professor.ProfessorId);

                if (professorDB == null)
                {
                    throw new KeyNotFoundException("Professor não encontrado para atualização.");
                }

                // Verificar se o professor já existe com base no e-mail e nome
                var professorExistente = await BuscarProfessorExistenteAsync(professor.Email, professor.Nome, professor.ProfessorId);

                if (professorExistente != null)
                {
                    throw new Exception("Já existe um professor registrado com este e-mail.");
                }

                // Atualizar os dados básicos do professor
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                professorDB.Nome = textInfo.ToTitleCase(professor.Nome.ToLower());
                professorDB.Email = professor.Email.ToLower();
                professorDB.Genero = professor.Genero;
                professorDB.Status = professor.Status;
                professorDB.DataNascimento = professor.DataNascimento;

                // Verificar status do professor
                if (professorDB.Status == StatusEnum.Inativo)
                {
                    // Buscar cursos associados ao professor
                    List<CursoModel> cursosDoProfessor = await BuscarCursosDoProfessorAsync(professor.ProfessorId);

                    if (cursosDoProfessor != null && cursosDoProfessor.Any())
                    {
                        foreach (var curso in cursosDoProfessor)
                        {
                            await RemoverProfessorDoCursoAsync(professor.ProfessorId, curso.CursoId);
                        }
                    }
                }

                // Salvar as alterações
                _context.Professores.Update(professorDB);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao atualizar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Remover um professor
        public async Task<bool> RemoverProfessorAsync(int id)
        {
            try
            {
                ProfessorModel professor = await BuscarProfessorPorIdAsync(id);

                if (professor == null)
                {
                    throw new Exception("Professor não encontrado para remoção.");
                }

                List<CursoModel> cursos = await BuscarCursosDoProfessorAsync(professor.ProfessorId);

                if (cursos != null && cursos.Any())
                {
                    throw new Exception("O professor está associado a cursos e não pode ser removido.");
                }

                // Remover professor do banco de dados
                _context.Professores.Remove(professor);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddProfessorAoCursoAsync(int professorId, int cursoId)
        {
            const int maxCursosPorProfessor = 3;

            try
            {
                // Buscar curso e professor
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);
                if (curso == null)
                {
                    throw new Exception($"O curso com ID {cursoId} não foi encontrado.");
                }

                var professor = await BuscarProfessorPorIdAsync(professorId);
                if (professor == null)
                {
                    throw new Exception($"O professor com ID {professorId} não foi encontrado.");
                }

                // Validar se o curso já está vinculado a outro professor
                if (curso.ProfessorId != null)
                {
                    throw new Exception($"O curso '{curso.Idioma}' já está associado ao professor '{curso.Professor?.Nome ?? "desconhecido"}'.");
                }

                // Validar se o professor atingiu o limite de cursos
                var cursosDoProfessor = await BuscarCursosDoProfessorAsync(professorId);
                if (cursosDoProfessor.Count() >= maxCursosPorProfessor)
                {
                    throw new Exception($"O professor '{professor.Nome}' já está associado ao número máximo de cursos permitidos ({maxCursosPorProfessor}).");
                }

                // Associar professor ao curso
                curso.ProfessorId = professorId;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar o professor ao curso: {ex.Message}", ex);
            }
        }

        // Remover um professor de um curso
        public async Task RemoverProfessorDoCursoAsync(int professorId, int cursoId)
        {
            try
            {
                var curso = await _cursoRepositorio.BuscarCursoPorIdAsync(cursoId);
                var professor = await BuscarProfessorPorIdAsync(professorId);

                if (curso == null || professor == null)
                {
                    throw new Exception("Curso ou professor não encontrado. Verifique os dados e tente novamente.");
                }

                curso.ProfessorId = null;
                _context.Cursos.Update(curso);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover o professor do curso. Tente novamente.", ex);
            }
        }
    }
}