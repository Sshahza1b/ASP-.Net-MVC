using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
