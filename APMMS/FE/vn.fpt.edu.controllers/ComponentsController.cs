using Microsoft.AspNetCore.Mvc;
using FE.vn.fpt.edu.services;
using FE.vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("Components")]
    public class ComponentsController : Controller
    {
        private readonly ComponentService _componentService;

        public ComponentsController(ComponentService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("~/vn.fpt.edu.views/Components/Index.cshtml");
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/vn.fpt.edu.views/Components/Create.cshtml");
        }


        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.ComponentId = id;
            return View("~/vn.fpt.edu.views/Components/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.ComponentId = id;
            return View("~/vn.fpt.edu.views/Components/Details.cshtml");
        }
    }
       
}