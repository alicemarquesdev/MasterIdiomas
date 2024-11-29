using MasterIdiomas.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterIdiomas.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<CursoModel> Cursos { get; set; }
        public DbSet<AlunoModel> Alunos { get; set; }
        public DbSet<ProfessorModel> Professores { get; set; }
        public DbSet<AlunoCursoModel> AlunoCurso { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CursoModel>()
               .HasOne(c => c.Professor)
               .WithMany(x => x.Cursos)
               .HasForeignKey(c => c.ProfessorId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

            modelBuilder.Entity<AlunoCursoModel>()
                .HasKey(ac => new { ac.AlunoId, ac.CursoId });

            modelBuilder.Entity<AlunoCursoModel>()
                .HasOne(ac => ac.Aluno)
                .WithMany(a => a.AlunoCurso)
                .HasForeignKey(ac => ac.AlunoId);

            modelBuilder.Entity<AlunoCursoModel>()
                .HasOne(ac => ac.Curso)
                .WithMany(c => c.AlunoCurso)
                .HasForeignKey(ac => ac.CursoId);

            base.OnModelCreating(modelBuilder);
        }
    }
}