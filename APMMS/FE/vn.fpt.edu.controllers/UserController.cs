using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FE.vn.fpt.edu.controllers
{
    [Route("Users")]
    // [Authorize] // Tạm thời comment để test
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.UserId = id;
            return View();
        }
        [HttpGet]
        [Route("Car/{id}")]
        public IActionResult Car(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

    }
}
