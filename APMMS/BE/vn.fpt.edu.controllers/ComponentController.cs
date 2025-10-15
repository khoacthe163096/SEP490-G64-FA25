using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    public class ComponentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
