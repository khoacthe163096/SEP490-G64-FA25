using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
