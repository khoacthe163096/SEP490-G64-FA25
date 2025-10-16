using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FE.vn.fpt.edu.controllers
{
    // [Authorize] // Tạm thời comment để test
    public class VehicleCheckinController : Controller
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
            ViewBag.CheckinId = id;
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.CheckinId = id;
            return View();
        }
    }
}
