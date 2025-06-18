using MasterIdiomas.Models;

namespace MasterIdiomas.ViewModels
{
    // ViewModel que representa os dados necessários para a tela de informações do professor
    public class ProfessorViewModel
    {
        // Propriedade para armazenar um único professor (caso seja necessário exibir ou editar)
        public ProfessorModel? Professor { get; set; }

        // Propriedade que contém uma lista de todos os professores disponíveis
        public List<ProfessorModel> Professores { get; set; } = new List<ProfessorModel>();

        // Propriedade que contém a lista de cursos aos quais o professor está inscrito
        public List<CursoModel> CursosDoProfessor { get; set; } = new List<CursoModel>();

        // Propriedade que contém a lista de cursos aos quais o professor não está inscrito
        public List<CursoModel> CursosProfessorNaoInscrito { get; set; } = new List<CursoModel>();
    }
}
