using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("ServicePackages")]
    public class ServicePackageController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/ServicePackages/Index.cshtml");
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
			return View("~/vn.fpt.edu.views/ServicePackages/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.ServicePackageId = id;
			return View("~/vn.fpt.edu.views/ServicePackages/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.ServicePackageId = id;
			return View("~/vn.fpt.edu.views/ServicePackages/Details.cshtml");
        }
    }
}
