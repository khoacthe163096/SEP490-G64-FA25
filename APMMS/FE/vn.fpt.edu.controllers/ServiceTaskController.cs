using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("ServiceTasks")]
    public class ServiceTaskController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/ServiceTasks/Index.cshtml");
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
			return View("~/vn.fpt.edu.views/ServiceTasks/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.ServiceTaskId = id;
			return View("~/vn.fpt.edu.views/ServiceTasks/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.ServiceTaskId = id;
			return View("~/vn.fpt.edu.views/ServiceTasks/Details.cshtml");
        }
    }
}
