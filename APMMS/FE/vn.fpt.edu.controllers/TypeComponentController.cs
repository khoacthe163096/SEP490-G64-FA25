using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("TypeComponents")]
    public class TypeComponentController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/TypeComponents/Index.cshtml");
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
			return View("~/vn.fpt.edu.views/TypeComponents/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.TypeComponentId = id;
			return View("~/vn.fpt.edu.views/TypeComponents/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.TypeComponentId = id;
			return View("~/vn.fpt.edu.views/TypeComponents/Details.cshtml");
        }
    }
}
