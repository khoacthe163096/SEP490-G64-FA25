using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
