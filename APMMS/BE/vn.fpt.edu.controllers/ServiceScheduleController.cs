using BE.vn.fpt.edu.DTOs.ServiceSchedule;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class ServiceScheduleController : ControllerBase
    {
        private readonly IServiceScheduleService _serviceScheduleService;

        public ServiceScheduleController(IServiceScheduleService serviceScheduleService)
        {
            _serviceScheduleService = serviceScheduleService;
        }

        /// <summary>
        /// Đặt lịch công khai (khách hàng chưa có tài khoản)
        /// </summary>
        [HttpPost("public-booking")]
        public async Task<IActionResult> CreatePublicBooking([FromBody] PublicBookingDto request)
        {
            try
            {
                var result = await _serviceScheduleService.CreatePublicBookingAsync(request);
                return Ok(new { success = true, data = result, message = "Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống, vui lòng thử lại sau", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo lịch hẹn mới (khách hàng đặt lịch)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] RequestDto request)
        {
            try
            {
                var result = await _serviceScheduleService.CreateScheduleAsync(request);
                return Ok(new { success = true, data = result, message = "Schedule created successfully" });
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
        /// Lấy lịch hẹn theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(long id)
        {
            try
            {
                var result = await _serviceScheduleService.GetScheduleByIdAsync(id);
                return Ok(new { success = true, data = result });
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

        /// <summary>
        /// Lấy danh sách tất cả lịch hẹn (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSchedules([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _serviceScheduleService.GetAllSchedulesAsync(page, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo User ID (lịch hẹn của khách hàng)
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetSchedulesByUserId(long userId)
        {
            try
            {
                var result = await _serviceScheduleService.GetSchedulesByUserIdAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo Branch ID
        /// </summary>
        [HttpGet("by-branch/{branchId}")]
        public async Task<IActionResult> GetSchedulesByBranchId(long branchId)
        {
            try
            {
                var result = await _serviceScheduleService.GetSchedulesByBranchIdAsync(branchId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo Status
        /// </summary>
        [HttpGet("by-status/{statusCode}")]
        public async Task<IActionResult> GetSchedulesByStatus(string statusCode)
        {
            try
            {
                var result = await _serviceScheduleService.GetSchedulesByStatusAsync(statusCode);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetSchedulesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _serviceScheduleService.GetSchedulesByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật lịch hẹn
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(long id, [FromBody] UpdateScheduleDto request)
        {
            try
            {
                var result = await _serviceScheduleService.UpdateScheduleAsync(id, request);
                return Ok(new { success = true, data = result, message = "Schedule updated successfully" });
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
        /// Hủy lịch hẹn
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelSchedule(long id, [FromBody] CancelScheduleDto? request = null)
        {
            try
            {
                var result = await _serviceScheduleService.CancelScheduleAsync(id, request);
                return Ok(new { success = true, data = result, message = "Schedule cancelled successfully" });
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
        /// Xóa lịch hẹn
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(long id)
        {
            try
            {
                var result = await _serviceScheduleService.DeleteScheduleAsync(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Schedule deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Schedule not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}
