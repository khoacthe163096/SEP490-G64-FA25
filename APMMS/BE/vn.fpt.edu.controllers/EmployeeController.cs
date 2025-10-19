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
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
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
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null) return Unauthorized();

            var userId = long.Parse(userIdClaim.Value);
            var profile = await _employeeService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null) return Unauthorized();

            var userId = long.Parse(userIdClaim.Value);
            var updated = await _employeeService.UpdateProfileAsync(userId, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }


    }
}
