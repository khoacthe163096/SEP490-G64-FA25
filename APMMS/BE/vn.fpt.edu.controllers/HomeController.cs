     using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
