using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Components
{
    public class IdiomasViewComponent : ViewComponent
    {
        private readonly ICursoRepositorio _cursoRepositorio;

        public IdiomasViewComponent(ICursoRepositorio cursoRepositorio)
        {
            _cursoRepositorio = cursoRepositorio;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Aguarde a chamada assíncrona para obter os idiomas
            var idiomas = await _cursoRepositorio.BuscarIdiomasAsync();

            return View(idiomas);
        }
    }
}