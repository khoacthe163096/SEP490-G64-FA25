using BE.vn.fpt.edu.DTOs.AutoOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.interfaces;
namespace BE.vn.fpt.edu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoOwnerController : ControllerBase
    {
        private readonly IAutoOwnerService _service;

        public AutoOwnerController(IAutoOwnerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] long? role = null)
        {
            try
            {
                // Normalize empty strings to null
                if (string.IsNullOrWhiteSpace(search)) search = null;
                if (string.IsNullOrWhiteSpace(status)) status = null;
                
                // Nếu có search, status hoặc role thì dùng filter
                if (search != null || status != null || role.HasValue)
                {
                    var result = await _service.GetWithFiltersAsync(page, pageSize, search, status, role);
                    return Ok(result);
                }
                
                // Nếu không có filter thì dùng method cũ nhưng vẫn trả về format đầy đủ
                var users = await _service.GetAllAsync(page, pageSize);
                var totalCount = await _service.GetTotalCountAsync(null, null, null);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                
                return Ok(new { 
                    success = true, 
                    data = users, 
                    page = page, 
                    pageSize = pageSize,
                    totalPages = totalPages,
                    currentPage = page,
                    totalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tải dữ liệu: " + ex.Message });
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound("Auto Owner not found.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// ✅ Cập nhật Status của AutoOwner (ACTIVE hoặc INACTIVE)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Branch Manager,Admin")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, request.StatusCode);
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

    public class UpdateStatusRequest
    {
        public string StatusCode { get; set; } = null!;
    }
}
