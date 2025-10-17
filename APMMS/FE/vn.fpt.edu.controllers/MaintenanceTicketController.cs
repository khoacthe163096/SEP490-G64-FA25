using Microsoft.AspNetCore.Mvc;
using FE.vn.fpt.edu.services;

namespace FE.vn.fpt.edu.controllers
{
    [Route("MaintenanceTickets")]
    public class MaintenanceTicketController : Controller
    {
        private readonly MaintenanceTicketService _service;

        public MaintenanceTicketController(MaintenanceTicketService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/MaintenanceTickets/Index.cshtml");
        }

        [HttpGet]
        [Route("ListData")]
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetAllAsync(page, pageSize);
            return Json(data);
        }

        [HttpPost]
        [Route("CreateFromCheckin")] 
        public async Task<IActionResult> CreateFromCheckin([FromBody] object payload)
        {
            var result = await _service.CreateFromCheckinAsync(payload);
            return Json(result);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
			return View("~/vn.fpt.edu.views/MaintenanceTickets/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.MaintenanceTicketId = id;
			return View("~/vn.fpt.edu.views/MaintenanceTickets/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.MaintenanceTicketId = id;
			return View("~/vn.fpt.edu.views/MaintenanceTickets/Details.cshtml");
        }
    }
}
