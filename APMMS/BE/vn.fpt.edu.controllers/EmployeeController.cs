using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.services;
using BE.vn.fpt.edu.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;

namespace BE.vn.fpt.edu.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly CarMaintenanceDbContext _dbContext;

        public EmployeeController(IEmployeeService employeeService, CloudinaryService cloudinaryService, CarMaintenanceDbContext dbContext)
        {
            _employeeService = employeeService;
            _cloudinaryService = cloudinaryService;
            _dbContext = dbContext;
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

        /// <summary>
        /// Upload avatar image to Cloudinary for employee (cho phép Admin/Branch Manager upload ảnh cho employee theo ID)
        /// </summary>
        [HttpPost("{id}/upload-avatar")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> UploadAvatar(long id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "Kích thước file không được vượt quá 5MB" });
                }

                // Kiểm tra employee có tồn tại không
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Employee not found" });
                }

                // Upload to Cloudinary (isAvatar = true để crop thành hình vuông)
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "user-avatars", isAvatar: true);

                // Cập nhật image URL vào database
                user.Image = imageUrl;
                user.LastModifiedDate = DateTime.Now;
                
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new { success = true, data = new { imageUrl = imageUrl }, message = "Upload avatar thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

    }
}
