using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetById(long id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound(new { success = false, message = "Employee not found" });
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestDto dto)
        {
            var result = await _employeeService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] EmployeeRequestDto dto)
        {
            var result = await _employeeService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Employee deleted successfully (soft delete)." });
        }
        [HttpGet("filter")]
        public async Task<IActionResult> FilterEmployees([FromQuery] bool? isDelete, [FromQuery] long? roleId)
        {
            var employees = await _employeeService.FilterAsync(isDelete, roleId);
            return Ok(employees);
        }
   

    }
}
