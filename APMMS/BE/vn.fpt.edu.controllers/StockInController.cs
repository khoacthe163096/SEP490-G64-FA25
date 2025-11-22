using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.DTOs.StockIn;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockInController : ControllerBase
    {
        private readonly IStockInService _service;

        public StockInController(IStockInService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? branchId = null,
            [FromQuery] string? statusCode = null,
            [FromQuery] string? search = null)
        {
            try
            {
                // Lấy BranchId từ JWT claim
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (long.TryParse(branchIdClaim, out var claimBranchId))
                {
                    branchId = claimBranchId;
                }

                var result = await _service.GetAllAsync(page, pageSize, branchId, statusCode, search);
                var totalCount = await _service.GetTotalCountAsync(branchId, statusCode, search);
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
                return Ok(new { success = true, data = item });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] StockInRequestDto dto)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var created = await _service.CreateFromRequestAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, new { success = true, data = created, message = "Phiếu nhập kho đã được tạo thành công" });
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] StockInRequestDto dto)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                dto.Id = id;
                var updated = await _service.UpdateAsync(dto, userId);
                return Ok(new { success = true, data = updated, message = "Phiếu nhập kho đã được cập nhật thành công" });
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

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> ChangeStatus(long id, [FromQuery] string statusCode)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var updated = await _service.ChangeStatusAsync(id, statusCode, userId);
                return Ok(new { success = true, data = updated, message = "Trạng thái đã được cập nhật thành công" });
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

        [HttpPost("{id}/approve")]
        public async Task<ActionResult> Approve(long id, [FromBody] StockInRequestDto dto)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                dto.Id = id;
                var updated = await _service.ApproveAsync(dto, userId);
                return Ok(new { success = true, data = updated, message = "Phiếu nhập kho đã được duyệt và cập nhật kho thành công" });
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

        [HttpPut("{id}/quantity-after-check")]
        public async Task<ActionResult> UpdateQuantityAfterCheck(long id, [FromBody] StockInRequestDto dto)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                dto.Id = id;
                var updated = await _service.UpdateQuantityAfterCheckAsync(dto, userId);
                return Ok(new { success = true, data = updated, message = "Số lượng sau kiểm tra đã được cập nhật thành công" });
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

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(long id)
        {
            try
            {
                // Lấy userId từ JWT token
                long userId = 0;
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var updated = await _service.CancelAsync(id, userId);
                return Ok(new { success = true, data = updated, message = "Phiếu nhập kho đã được hủy thành công" });
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

        [HttpGet("status/{statusCode}")]
        public async Task<ActionResult> GetByStatus(string statusCode)
        {
            try
            {
                var result = await _service.GetByStatusAsync(statusCode);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}

