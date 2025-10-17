using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FE.vn.fpt.edu.services;

namespace FE.vn.fpt.edu.controllers
{
    // [Authorize] // Tạm thời comment để test
    [Route("VehicleCheckins")]
    public class VehicleCheckinController : Controller
    {
        private readonly VehicleCheckinService _service;

        public VehicleCheckinController(VehicleCheckinService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/VehicleCheckins/Index.cshtml");
        }

        [HttpGet]
        [Route("ListData")]
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetAllAsync(page, pageSize);
            return Json(data);
        }

        [HttpGet]
        [Route("SearchVehicle")]
        public async Task<IActionResult> SearchVehicle(string searchTerm)
        {
            var result = await _service.SearchVehicleAsync(searchTerm);
            return Json(result);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
			return View("~/vn.fpt.edu.views/VehicleCheckins/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.CheckinId = id;
			return View("~/vn.fpt.edu.views/VehicleCheckins/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.CheckinId = id;
			return View("~/vn.fpt.edu.views/VehicleCheckins/Details.cshtml");
        }
    }
}
