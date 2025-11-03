using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE.vn.fpt.edu.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] long? role = null)
        {
            // Luôn dùng filter method để đảm bảo format response đồng nhất
            var result = await _employeeService.GetWithFiltersAsync(page, pageSize, search, status, role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> GetById(long id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound(new { success = false, message = "Employee not found" });
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestDto dto)
        {
            try
            {
                // Get RoleId from JWT claims
                var roleIdClaim = User.FindFirst("RoleId")?.Value;
                var roleId = long.TryParse(roleIdClaim, out var parsedRoleId) ? parsedRoleId : 0;
                
                // If logged in as Branch Manager (roleId = 2), auto-set branchId from JWT
                if (roleId == 2 && dto.BranchId == null || dto.BranchId == 0)
                {
                    var branchIdClaim = User.FindFirst("BranchId")?.Value;
                    if (long.TryParse(branchIdClaim, out var branchId))
                    {
                        dto.BranchId = branchId;
                    }
                }
                
                var result = await _employeeService.CreateAsync(dto);
                return Ok(new { success = true, data = result, message = "Employee created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message, stackTrace = ex.StackTrace, innerException = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] EmployeeRequestDto dto)
        {
            try
            {
                var result = await _employeeService.UpdateAsync(id, dto);
                if (result == null) return NotFound(new { success = false, message = "Employee not found" });
                return Ok(new { success = true, data = result, message = "Employee updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Employee deleted successfully (soft delete)." });
        }
        
        [HttpGet("filter")]
        [Authorize] // Cho phép tất cả user đã đăng nhập (bao gồm Consulter) truy cập để lấy danh sách technician
        public async Task<IActionResult> FilterEmployees([FromQuery] bool? isDelete, [FromQuery] long? roleId)
        {
            var employees = await _employeeService.FilterAsync(isDelete, roleId);
            return Ok(employees);
        }
        
        /// <summary>
        /// ✅ Cập nhật Status của Employee (ACTIVE hoặc INACTIVE)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] EmployeeUpdateStatusDto request)
        {
            try
            {
                var result = await _employeeService.UpdateStatusAsync(id, request.StatusCode);
                return Ok(new { success = true, data = result, message = "Status updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

    }
}
