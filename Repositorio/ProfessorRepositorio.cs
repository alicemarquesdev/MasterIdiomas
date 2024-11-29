using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    public class ProfessorRepositorio : IProfessorRepositorio
    {
        private readonly BancoContext _context;

        public ProfessorRepositorio(BancoContext context)
        {
            _context = context;
        }

        public async Task<List<CursoModel>> BuscarCursosDoProfessorAsync(int professorId)
        {
            return await _context.Cursos.Include(c => c.Professor).Where(c => c.ProfessorId == professorId).ToListAsync();
        }

        public async Task<ProfessorModel> BuscarProfessorExistenteAsync(string email, string nome, int professorIdIgnorar)
        {
            return await _context.Professores.FirstOrDefaultAsync(x => x.Email == email && x.Nome == nome && x.ProfessorId != professorIdIgnorar);
        }

        public async Task<ProfessorModel> BuscarProfessorPorIdAsync(int id)
        {
            return await _context.Professores.FirstOrDefaultAsync(x => x.ProfessorId == id);
        }

        public async Task<List<ProfessorModel>> BuscarTodosProfessoresAsync()
        {
            return await _context.Professores.ToListAsync();
        }

        public int TotalProfessores()
        {
            return _context.Professores.Count();
        }

        public async Task AddProfessorAsync(ProfessorModel professor)
        {
            professor.DataCadastro = DateTime.Now;
            professor.Status = StatusEnum.Ativo;
            await _context.Professores.AddAsync(professor);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarProfessorAsync(ProfessorModel professor)
        {
            if (professor == null)
            {
                throw new Exception("Houve um erro ao editar os dados do professor: o objeto professor é nulo.");
            }

            ProfessorModel professorDB = await BuscarProfessorPorIdAsync(professor.ProfessorId);

            if (professorDB == null)
            {
                throw new Exception("Houve um erro ao editar os dados do professor: o professor não foi encontrado.");
            }
            professorDB.Nome = professor.Nome;
            professorDB.Email = professor.Email;
            professorDB.Genero = professor.Genero;
            professorDB.Status = professor.Status;
            professorDB.DataNascimento = professor.DataNascimento;


            _context.Professores.Update(professorDB);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoverProfessorAsync(int id)
        {
            ProfessorModel professor = await BuscarProfessorPorIdAsync(id);

            if (professor == null) throw new Exception("Houve um erro na deleção do aluno.");

            _context.Professores.Remove(professor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}