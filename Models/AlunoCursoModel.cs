namespace MasterIdiomas.Models
{
    // Modelo que representa uma tabela de junção entre alunos e cursos.
    // Esse modelo é usado para estabelecer a relação muitos-para-muitos entre Aluno e Curso.
    public class AlunoCursoModel
    {
        public int AlunoId { get; set; } // Identificador do aluno
        public virtual AlunoModel? Aluno { get; set; } // Relacionamento com o modelo de Aluno

        public int CursoId { get; set; } // Identificador do curso
        public virtual CursoModel? Curso { get; set; } // Relacionamento com o modelo de Curso
    }
}
