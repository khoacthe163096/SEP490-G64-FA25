using FE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("TypeComponents")]
    public class TypeComponentController : Controller
    {
        private readonly TypeComponentService _service;

        public TypeComponentController(TypeComponentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("~/vn.fpt.edu.views/TypeComponents/Index.cshtml");
        }

        [HttpGet]
        [Route("ListData")]
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10, string? search = null, string? statusCode = null)
        {
            // Lấy branchId của user đang đăng nhập
            long? branchId = null;

            // Thử lấy từ session trước
            var branchIdFromSession = HttpContext.Session.GetString("BranchId");
            if (!string.IsNullOrEmpty(branchIdFromSession) && long.TryParse(branchIdFromSession, out var sessionBranchId))
            {
                branchId = sessionBranchId;
                Console.WriteLine($"[FE TypeComponentController] Got branchId from session: {branchId}");
            }
            else
            {
                // Thử lấy từ JWT claim
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (long.TryParse(branchIdClaim, out var claimBranchId))
                {
                    branchId = claimBranchId;
                    Console.WriteLine($"[FE TypeComponentController] Got branchId from JWT claim: {branchId}");
                }
                else
                {
                    Console.WriteLine("[FE TypeComponentController] No branchId in session or JWT claim, trying Employee/me API");
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
                                            Console.WriteLine($"[FE TypeComponentController] Got branchId from Employee/me API: {branchId}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[FE TypeComponentController] Error getting employee info: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"[FE TypeComponentController] Final branchId: {branchId}, statusCode: {statusCode}");
            var data = await _service.GetAllAsync(page, pageSize, search, statusCode, branchId);
            return Json(data);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/vn.fpt.edu.views/TypeComponents/Create.cshtml");
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] object data)
        {
            try
            {
                var result = await _service.CreateAsync(data);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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

        [HttpGet]
        [Route("GetDetails/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return Json(data);
        }

        [HttpPost]
        [Route("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(long id, string statusCode)
        {
            try
            {
                var ok = await _service.ToggleStatusAsync(id, statusCode);
                return Json(new { success = ok });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] object data)
        {
            try
            {
                var result = await _service.UpdateAsync(id, data);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
