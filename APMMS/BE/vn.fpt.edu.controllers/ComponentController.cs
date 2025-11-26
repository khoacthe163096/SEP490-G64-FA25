using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentController : ControllerBase
    {
        private readonly IComponentService _service;
        public ComponentController(IComponentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? branchId = null,
            [FromQuery] long? typeComponentId = null,
            [FromQuery] string? statusCode = null,
            [FromQuery] string? search = null)
        {
            try
            {
                // Lấy userId từ JWT token
                long? userId = null;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                // LUÔN lấy BranchId từ JWT claim của user đang đăng nhập để đảm bảo chỉ hiển thị linh kiện của branch đó
                // Bỏ qua branchId từ query parameter để tránh user có thể xem linh kiện của branch khác
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (long.TryParse(branchIdClaim, out var claimBranchId))
                {
                    branchId = claimBranchId;
                }
                // Nếu không có branchId trong JWT, không trả về dữ liệu (hoặc có thể throw exception tùy yêu cầu)
                // Để đảm bảo an toàn, nếu không có branchId thì set branchId = -1 để không trả về gì
                if (!branchId.HasValue)
                {
                    branchId = -1; // Sẽ không có linh kiện nào có branchId = -1
                }

                var result = await _service.GetAllAsync(page, pageSize, branchId, typeComponentId, statusCode, search);
                var totalCount = await _service.GetTotalCountAsync(branchId, typeComponentId, statusCode, search);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return Ok(new
                {
                    success = true,
                    data = result,
                    page = page,
                    pageSize = pageSize,
                    totalPages = totalPages,
                    currentPage = page,
                    totalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { success = false, message = "Component not found" });
                return Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RequestDto dto)
        {
            try
            {
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (!long.TryParse(branchIdClaim, out var branchId))
                {
                    return BadRequest(new { success = false, message = "Không xác định được chi nhánh của người dùng" });
                }

                dto.BranchId = branchId;

                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, new { success = true, data = created, message = "Component created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            try
            {
                dto.Id = id;

                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (!long.TryParse(branchIdClaim, out var branchId))
                {
                    return BadRequest(new { success = false, message = "Không xác định được chi nhánh của người dùng" });
                }

                dto.BranchId = branchId;

                var updated = await _service.UpdateAsync(dto);
                if (updated == null) return NotFound(new { success = false, message = "Component not found" });
                return Ok(new { success = true, data = updated, message = "Component updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatus(long id, [FromQuery] string statusCode)
        {
            try
            {
                await _service.DisableEnableAsync(id, statusCode);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("batch-update-status")]
        public async Task<IActionResult> BatchUpdateStatus([FromBody] BatchUpdateStatusDto dto)
        {
            try
            {
                if (dto == null || dto.ComponentIds == null || dto.ComponentIds.Count == 0)
                {
                    return BadRequest(new { success = false, message = "Danh sách linh kiện không được rỗng" });
                }

                if (string.IsNullOrWhiteSpace(dto.StatusCode))
                {
                    return BadRequest(new { success = false, message = "Trạng thái không được rỗng" });
                }

                var updatedCount = await _service.BatchUpdateStatusAsync(dto.ComponentIds, dto.StatusCode);
                return Ok(new { success = true, message = $"Đã cập nhật trạng thái cho {updatedCount} linh kiện", updatedCount = updatedCount });
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

    public class BatchUpdateStatusDto
    {
        public List<long> ComponentIds { get; set; } = new List<long>();
        public string StatusCode { get; set; } = string.Empty;
    }
}