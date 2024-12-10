namespace MasterIdiomas.Models
{
    public class AlunoCursoModel
    {
        public int AlunoId { get; set; }
        public virtual AlunoModel Aluno { get; set; }

        public int CursoId { get; set; }
        public virtual CursoModel Curso { get; set; }
    }
}