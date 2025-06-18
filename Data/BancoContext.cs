using MasterIdiomas.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Data
{
    // Classe que representa o contexto do banco de dados, derivada de DbContext
    public class BancoContext : DbContext
    {
        // Construtor que recebe as opções de configuração do contexto
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        // DbSets para representar as tabelas do banco de dados
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<CursoModel> Cursos { get; set; }
        public DbSet<AlunoModel> Alunos { get; set; }
        public DbSet<ProfessorModel> Professores { get; set; }
        public DbSet<AlunoCursoModel> AlunoCurso { get; set; }

        // Método para configurar o modelo de dados e as relações entre as entidades
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                // Configuração da relação entre Curso e Professor
                modelBuilder.Entity<CursoModel>()
                    .HasOne(c => c.Professor)  // Um curso tem um professor
                    .WithMany(p => p.Cursos)   // Um professor tem muitos cursos
                    .HasForeignKey(c => c.ProfessorId) // Chave estrangeira para ProfessorId
                    .IsRequired(false)          // A relação é opcional
                    .OnDelete(DeleteBehavior.Restrict);  // Não permite deleção em cascata               

                // Configuração da chave composta entre AlunoCurso
                modelBuilder.Entity<AlunoCursoModel>()
                    .HasKey(ac => new { ac.AlunoId, ac.CursoId }); // Chave composta entre AlunoId e CursoId

                // Configuração das relações de AlunoCurso com Aluno e Curso
                modelBuilder.Entity<AlunoCursoModel>()
                    .HasOne(ac => ac.Aluno)  // Cada AlunoCurso está relacionado a um Aluno
                    .WithMany(a => a.AlunoCurso) // Cada Aluno pode ter muitos AlunoCursos
                    .HasForeignKey(ac => ac.AlunoId) // Chave estrangeira para AlunoId
                    .OnDelete(DeleteBehavior.Restrict);  // Configuração para excluir registros relacionados ao Aluno


                modelBuilder.Entity<AlunoCursoModel>()
                    .HasOne(ac => ac.Curso)  // Cada AlunoCurso está relacionado a um Curso
                    .WithMany(c => c.AlunoCurso) // Cada Curso pode ter muitos AlunoCursos
                    .HasForeignKey(ac => ac.CursoId) // Chave estrangeira para CursoId
                    .OnDelete(DeleteBehavior.Restrict);  // Configuração para excluir registros relacionados ao Curso

                base.OnModelCreating(modelBuilder);  // Chama a implementação base para garantir que todas as configurações sejam aplicadas
            }
            catch (Exception ex)
            {
                // Captura qualquer erro durante a configuração do modelo
                Console.WriteLine($"Erro na configuração do modelo: {ex.Message}");
                throw;
            }
        }
    }
}