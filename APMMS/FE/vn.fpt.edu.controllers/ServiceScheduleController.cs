using Microsoft.AspNetCore.Mvc;
using FE.vn.fpt.edu.services;

namespace FE.vn.fpt.edu.controllers
{
    [Route("ServiceSchedules")]
    public class ServiceScheduleController : Controller
    {
        private readonly ServiceScheduleService _service;
        private readonly IConfiguration _configuration;

        public ServiceScheduleController(ServiceScheduleService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173/api";
            return View("~/vn.fpt.edu.views/ServiceSchedules/Index.cshtml");
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173/api";
            return View("~/vn.fpt.edu.views/ServiceSchedules/Create.cshtml");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            ViewBag.ServiceScheduleId = id;
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173/api";
            return View("~/vn.fpt.edu.views/ServiceSchedules/Edit.cshtml");
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.ServiceScheduleId = id;
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173/api";
            return View("~/vn.fpt.edu.views/ServiceSchedules/Details.cshtml");
        }

        [HttpGet]
        [Route("ListData")]
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10, string? status = null, string? date = null)
        {
            try
            {
                object? data;
                if (!string.IsNullOrEmpty(status))
                {
                    data = await _service.GetByStatusAsync(status);
                }
                else if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime dateValue))
                {
                    var startDate = dateValue.Date;
                    var endDate = startDate.AddDays(1);
                    data = await _service.GetByDateRangeAsync(startDate, endDate);
                }
                else
                {
                    data = await _service.GetAllAsync(page, pageSize);
                }
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
