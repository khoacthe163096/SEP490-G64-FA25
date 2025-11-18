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
            // ✅ Mặc định chỉ hiển thị check-in đã xác nhận (CONFIRMED) nếu không có statusCode
            if (string.IsNullOrWhiteSpace(statusCode))
            {
                statusCode = "CONFIRMED";
            }

            // ✅ Lấy branchId của user đang đăng nhập
            long? branchId = null;
            
            // Thử lấy từ session trước (nếu có)
            var branchIdFromSession = HttpContext.Session.GetString("BranchId");
            if (!string.IsNullOrEmpty(branchIdFromSession) && long.TryParse(branchIdFromSession, out var sessionBranchId))
            {
                branchId = sessionBranchId;
                Console.WriteLine($"[FE VehicleCheckinController] Got branchId from session: {branchId}");
            }
            else
            {
                // Thử lấy từ JWT claim (nếu có)
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (long.TryParse(branchIdClaim, out var claimBranchId))
                {
                    branchId = claimBranchId;
                    Console.WriteLine($"[FE VehicleCheckinController] Got branchId from JWT claim: {branchId}");
                }
                else
                {
                    Console.WriteLine("[FE VehicleCheckinController] No branchId in session or JWT claim, trying Employee/me API");
                    // Nếu không có trong session hoặc claim, lấy từ Employee/me API
                    try
                    {
                        var employeeResponse = await _service.GetEmployeeInfoAsync();
                        if (employeeResponse != null)
                        {
                            // Response format: { success: true, data: { id, branchId, branchName } }
                            var successProperty = employeeResponse.GetType().GetProperty("success");
                            var dataProperty = employeeResponse.GetType().GetProperty("data");
                            
                            if (successProperty != null && dataProperty != null)
                            {
                                var success = successProperty.GetValue(employeeResponse);
                                var employeeData = dataProperty.GetValue(employeeResponse);
                                
                                if (success != null && success.ToString() == "True" && employeeData != null)
                                {
                                    var branchIdProp = employeeData.GetType().GetProperty("branchId") 
                                        ?? employeeData.GetType().GetProperty("BranchId");
                                    if (branchIdProp != null)
                                    {
                                        var branchIdValue = branchIdProp.GetValue(employeeData);
                                        if (branchIdValue != null && long.TryParse(branchIdValue.ToString(), out var empBranchId))
                                        {
                                            branchId = empBranchId;
                                            // Lưu vào session để lần sau không cần gọi API
                                            HttpContext.Session.SetString("BranchId", branchId.Value.ToString());
                                            Console.WriteLine($"[FE VehicleCheckinController] Got branchId from Employee/me API: {branchId}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error nhưng không throw
                        Console.WriteLine($"[FE VehicleCheckinController] Error getting employee info: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"[FE VehicleCheckinController] Final branchId: {branchId}, statusCode: {statusCode}");
            var data = await _service.GetAllAsync(page, pageSize, searchTerm, statusCode, fromDate, toDate, branchId);
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
