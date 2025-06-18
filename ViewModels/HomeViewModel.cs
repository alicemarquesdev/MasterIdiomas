using MasterIdiomas.Models;

namespace MasterIdiomas.ViewModels
{
    // ViewModel que contém os dados necessários para a tela principal da aplicação (Home)
    public class HomeViewModel
    {
        // Propriedade para armazenar o nome do usuário (pode ser utilizado para saudações ou personalização da interface)
        public string? UsuarioNome { get; set; }

        // Propriedade para armazenar o total de alunos registrados no sistema
        public int TotalAlunos { get; set; }

        // Propriedade para armazenar o total de professores registrados no sistema
        public int TotalProfessores { get; set; }

        // Propriedade para armazenar o total de idiomas disponíveis no sistema
        public int TotalIdiomas { get; set; }

        // Propriedade para armazenar o total de cursos oferecidos no sistema
        public int TotalCursos { get; set; }

        // Propriedade que armazena a lista de cursos para exibição na página inicial
        public List<CursoModel> Cursos { get; set; } = new List<CursoModel>();
    }
}
