using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class ComponentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.ComponentId = id;
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ComponentId = id;
            return View();
        }
    }
}
