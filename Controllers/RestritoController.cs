using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class RestritoController : Controller
    {
        // Ação que exibe a página inicial do módulo restrito, em caso de acesso a página restrita [UsuarioLogado]
        public IActionResult Index()
        {
            return View();
        }
    }
}