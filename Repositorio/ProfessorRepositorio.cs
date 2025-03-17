using MasterIdiomas.Data;
using MasterIdiomas.Enums;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MasterIdiomas.Repositorio
{
    // Classe responsável pelas operações de acesso ao banco de dados relacionadas aos professores, incluindo verificação, adição, atualização e remoção de professores
    // - VerificarProfessorExistenteAsync(string email, string nome) - verifica se um professor já está registrado com o mesmo e-mail e nome
    // - BuscarProfessorPorIdAsync(int id) - busca um professor pelo ID, incluindo a quantidade de cursos associados
    // - BuscarTodosProfessoresAsync() - busca todos os professores, incluindo a quantidade de cursos associados
    // - AddProfessorAsync(ProfessorModel professor) - adiciona um novo professor, realizando verificações de duplicidade e definindo valores padrão
    // - AtualizarProfessorAsync(ProfessorModel professor) - atualiza os dados de um professor, verificando a existência e realizando ajustes conforme necessário
    // - RemoverProfessorAsync(ProfessorModel professor) - remove um professor, verificando se ele está associado a cursos antes de removê-lo
    // - TotalProfessores() - retorna a quantidade total de professores cadastrados
    public class ProfessorRepositorio : IProfessorRepositorio
    {
        private readonly BancoContext _context;

        public ProfessorRepositorio(BancoContext context)
        {
            _context = context;
        }

        // Verificar se o professor já existe com base em email e nome
        public async Task<bool> VerificarProfessorExistenteAsync(string email, string nome)
        {
            try
            {
                return await _context.Professores
                    .Select(x => x.Email == email && x.Nome == nome).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao verificar a existência do professor. Tente novamente mais tarde.", ex);
            }
        }

        // Buscar professor por ID, incluindo a quantidade de cursos
        public async Task<ProfessorModel?> BuscarProfessorPorIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID inválido.");
                }

                // Buscar o professor com seus cursos
                var professor = await _context.Professores
                    .FirstOrDefaultAsync(x => x.ProfessorId == id);

                if (professor != null)
                {
                    // Calcula a quantidade de cursos
                    professor.QuantidadeCursos = professor.Cursos?.Count() ?? 0;
                }

                return professor;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao buscar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Buscar todos os professores, incluindo a quantidade de cursos
        public async Task<List<ProfessorModel>> BuscarTodosProfessoresAsync()
        {
            try
            {
                // Buscar todos os professores e seus cursos
                var professores = await _context.Professores
                    .Include(p => p.Cursos)  // Inclui os cursos do professor
                    .OrderBy(p => p.Nome)
                    .ToListAsync();

                // Adiciona a quantidade de cursos para cada professor
                foreach (var professor in professores)
                {
                    professor.QuantidadeCursos = professor.Cursos?.Count() ?? 0;
                }

                return professores;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar os professores. Tente novamente mais tarde.", ex);
            }
        }

        // Adicionar um novo professor
        public async Task AddProfessorAsync(ProfessorModel professor)
        {
            try
            {
                // Definir valores padrão para campos obrigatórios
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                professor.Nome = textInfo.ToTitleCase(professor.Nome.ToLower());
                professor.Email = professor.Email.ToLower();
                professor.DataCadastro = DateTime.Now;
                professor.Status = StatusEnum.Ativo;

                // Adicionar o professor ao banco de dados
                _context.Professores.Add(professor);
                var result = await _context.SaveChangesAsync(); // Salva as alterações

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados."); // Garante que a operação foi bem-sucedida
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao adicionar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Atualizar dados de um professor
        public async Task AtualizarProfessorAsync(ProfessorModel professor)
        {
            try
            {
                var professorDb = await BuscarProfessorPorIdAsync(professor.ProfessorId);
                if(professorDb == null)
                {
                    throw new Exception("Professor não encontrado no banco de dados");
                }
                // Atualizar os dados básicos do professor
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                professorDb.Nome = textInfo.ToTitleCase(professor.Nome.ToLower());
                professorDb.Email = professor.Email.ToLower();
                professorDb.Genero = professor.Genero;
                professorDb.Status = professor.Status;
                professorDb.DataNascimento = professor.DataNascimento;

                          // Salvar as alterações
                _context.Professores.Update(professorDb);
                var result = await _context.SaveChangesAsync(); // Salva as alterações

                if (result <= 0)
                {
                    throw new Exception("Nenhuma alteração no banco de dados."); // Garante que a operação foi bem-sucedida
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao atualizar o professor. Tente novamente mais tarde.", ex);
            }
        }

        // Remover um professor se não estiver associado em nenhum curso.
        public async Task<bool> RemoverProfessorAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID inválido.");
                }

                // Buscar o professor pelo ID
                var professor = await _context.Professores.FindAsync(id);
                if (professor == null)
                {
                    throw new Exception("Professor não encontrado.");
                }

                // Remover professor do banco de dados
                _context.Professores.Remove(professor);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover o professor.", ex);
            }
        }

        // Contagem total de professores no banco de dados
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
    }
}
