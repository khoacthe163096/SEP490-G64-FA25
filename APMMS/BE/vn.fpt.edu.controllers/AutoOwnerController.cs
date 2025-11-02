using BE.vn.fpt.edu.DTOs.AutoOwner;

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
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound("Auto Owner not found or already deleted.");
            return NoContent();
        }
    }
}
