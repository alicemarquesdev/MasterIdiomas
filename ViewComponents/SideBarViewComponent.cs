using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Components
{
    // ViewComponent para exibir a lista de cursos de idiomas na SideBar
    public class SideBarViewComponent : ViewComponent
    {
        // Declaração do campo privado para acessar o repositório de cursos
        private readonly ICursoRepositorio _cursoRepositorio;

        // Construtor que recebe a dependência do repositório de cursos
        public SideBarViewComponent(ICursoRepositorio cursoRepositorio)
        {
            _cursoRepositorio = cursoRepositorio; // Atribui o repositório de cursos
        }

        // Método que é chamado para gerar o componente da sidebar
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Chama o repositório de cursos de forma assíncrona para buscar a lista de idiomas
                var idiomas = await _cursoRepositorio.BuscarIdiomasAsync();

                // Retorna a view com a lista de idiomas recuperada
                return View(idiomas);
            }
            catch (Exception)
            {
                // Se ocorrer algum erro durante o processo, retorna uma view de erro com uma mensagem
                return View("Error", new { Message = "Ocorreu um erro ao carregar os idiomas. Tente novamente mais tarde." });
            }
        }
    }
}
