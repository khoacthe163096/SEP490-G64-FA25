using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class ServiceScheduleController : Controller
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
            ViewBag.ServiceScheduleId = id;
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ServiceScheduleId = id;
            return View();
        }
    }
}
