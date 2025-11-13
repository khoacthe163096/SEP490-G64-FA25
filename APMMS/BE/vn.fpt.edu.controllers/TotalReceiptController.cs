using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TotalReceipt;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TotalReceiptController : ControllerBase
    {
        private readonly ITotalReceiptService _service;

        public TotalReceiptController(ITotalReceiptService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
                                                  [FromQuery] string? statusCode = null, [FromQuery] DateTime? fromDate = null,
                                                  [FromQuery] DateTime? toDate = null, [FromQuery] long? branchId = null)
        {
            try
            {
                var result = await _service.GetPagedAsync(page, pageSize, search, statusCode, fromDate, toDate, branchId);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("by-ticket/{maintenanceTicketId}")]
        public async Task<IActionResult> GetByMaintenanceTicket(long maintenanceTicketId)
        {
            try
            {
                var result = await _service.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(new { success = true, data = created, message = "Invoice created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            try
            {
                // ✅ Lấy current user từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                long? currentUserId = null;
                if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var userId))
                {
                    currentUserId = userId;
                }

                var updated = await _service.UpdateAsync(id, dto, currentUserId);
                if (updated == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new { success = true, data = updated, message = "Invoice updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new { success = true, message = "Invoice deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}
