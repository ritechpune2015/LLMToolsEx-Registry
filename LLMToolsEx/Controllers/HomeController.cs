using Microsoft.AspNetCore.Mvc;

namespace LLMToolsEx.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
