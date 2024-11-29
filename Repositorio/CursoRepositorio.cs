using MasterIdiomas.Data;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Repositorio
{
    public class CursoRepositorio : ICursoRepositorio
    {
        private readonly BancoContext _context;
        private readonly IProfessorRepositorio _professorRepositorio;

        public CursoRepositorio(BancoContext context, IProfessorRepositorio professorRepositorio)
        {
            _context = context;
            _professorRepositorio = professorRepositorio;
        }

        public async Task<CursoModel> BuscarCursoPorIdAsync(int id)
        {
            return await _context.Cursos.Include(c => c.Professor)
                                        .Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno)
                                        .FirstOrDefaultAsync(x => x.CursoId == id);
        }

        public async Task<List<CursoModel>> BuscarIdiomasAsync()
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

        public async Task<List<CursoModel>> BuscarTodosCursosAsync()
        {
            return await _context.Cursos.Include(a => a.AlunoCurso).ThenInclude(a => a.Aluno).Include(c => c.Professor).ToListAsync();
        }

        public async Task<CursoModel> BuscarCursoExistenteAsync(string idioma, string turno, string nivel, int cursoIdIgnorar)
        {
            return await _context.Cursos.FirstOrDefaultAsync(x => x.Idioma == idioma && x.Turno == turno && x.Nivel == nivel && x.CursoId != cursoIdIgnorar);
        }

        public int CursosEmAndamento()
        {
            return _context.Cursos.Where(x => x.Status == Enums.StatusCursoEnum.EmAndamento).Select(c => c.Idioma).Distinct().Count();
        }

        public int TotalCursos()
        {
            return _context.Cursos.ToList().Count();
        }

        public int TotalIdiomas()
        {
            return _context.Cursos.Select(i => i.Idioma).Distinct().Count();
        }

        public async Task AddCursoAsync(CursoModel curso)
        {
            CursoModel cursoExistente = await BuscarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
            if (cursoExistente != null) throw new Exception("Infelizmente, não foi possível criar o curso, já existe um curso registrado com o mesmo idioma, turno e nível.");

            var cursosEmAndamento = CursosEmAndamento();
            if (cursosEmAndamento == 12) throw new Exception("Infelizmente, não foi possível criar o curso, já existe um curso igual a esse.");

            var professorExistente = await _professorRepositorio.BuscarProfessorExistenteAsync(curso.Professor.Email, curso.Professor.Nome, curso.Professor.ProfessorId);
            if (professorExistente != null) throw new Exception("Infelizmente, não foi possível criar o curso, já existe um professor registrado com o mesmo email e nome.");

            curso.Status = Enums.StatusCursoEnum.EmAndamento;

            await _context.Cursos.AddAsync(curso);
            await _professorRepositorio.AddProfessorAsync(curso.Professor);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarCursoAsync(CursoModel curso)
        {
            var cursoDb = await BuscarCursoPorIdAsync(curso.CursoId);
            if (cursoDb == null) throw new Exception("Não foi possível concluir a operação. Verifique os dados e tente novamente.");

            var cursoDuplicado = await BuscarCursoExistenteAsync(curso.Idioma, curso.Turno, curso.Nivel, curso.CursoId);
            if (cursoDuplicado != null) throw new Exception("Infelizmente, não foi possível editar o curso, já existe um curso registrado com o mesmo idioma, turno e nível.");

           
            var cursosEmAndamento = CursosEmAndamento();
            if ((curso.Status != Enums.StatusCursoEnum.EmAndamento && cursosEmAndamento == 1) || (cursosEmAndamento == 12 && curso.Status == Enums.StatusCursoEnum.EmAndamento)) throw new Exception("Infelizmente, não é possível atualizar o status devido aos limites do sistema.");

            cursoDb.Idioma = curso.Idioma;
            cursoDb.Turno = curso.Turno;
            cursoDb.Nivel = curso.Nivel;
            cursoDb.DataInicio = curso.DataInicio;
            cursoDb.CargaHoraria = curso.CargaHoraria;
            cursoDb.MaxAlunos = curso.MaxAlunos;
            cursoDb.DataAtualizacao = DateTime.Now;
            cursoDb.Status = curso.Status;
            cursoDb.ProfessorId = curso.ProfessorId;



            _context.Cursos.Update(cursoDb);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoverCursoAsync(int id)
        {
            CursoModel cursoDb = await BuscarCursoPorIdAsync(id);

            if (cursoDb == null) throw new Exception("Não foi possível concluir a operação. Verifique os dados e tente novamente.");

            var cursosEmAndamento = CursosEmAndamento();

            if (cursoDb.Status == Enums.StatusCursoEnum.EmAndamento && cursosEmAndamento == 1) throw new Exception("Infelizmente, não é possível deletar o curso devido aos limites do sistema.");

            _context.Remove(cursoDb);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}