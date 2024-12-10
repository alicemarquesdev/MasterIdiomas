using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Components
{
    // ViewComponent para exibir a lista de cursos de idiomas na SideBar
    public class IdiomasViewComponent : ViewComponent
    {
        private readonly ICursoRepositorio _cursoRepositorio;

        public IdiomasViewComponent(ICursoRepositorio cursoRepositorio)
        {
            _cursoRepositorio = cursoRepositorio;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Aguarda a chamada assíncrona para obter os idiomas
                var idiomas = await _cursoRepositorio.BuscarIdiomasAsync();

                return View(idiomas);
            }
            catch (Exception)
            {
                return View("Error", new { Message = "Ocorreu um erro ao carregar os idiomas. Tente novamente mais tarde." });
            }
        }
    }
}