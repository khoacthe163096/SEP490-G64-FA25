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
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10, string? searchTerm = null, string? statusCode = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = await _service.GetAllAsync(page, pageSize, searchTerm, statusCode, fromDate, toDate);
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

        [HttpGet]
        [Route("GetDetails/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return Json(data);
        }

        [HttpPost]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                // Lấy dữ liệu thật từ form
                var formData = new Dictionary<string, object>();
                foreach (var key in Request.Form.Keys)
                {
                    var value = Request.Form[key].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        formData[key] = value;
                    }
                }
                
                Console.WriteLine($"Updating checkin {id} with real data:");
                foreach (var kv in formData)
                {
                    Console.WriteLine($"  {kv.Key}: {kv.Value}");
                }
                
                var result = await _service.UpdateAsync(id, formData);
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
