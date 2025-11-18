using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.DTOs.VehicleCheckin;
using BE.vn.fpt.edu.interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace vn.fpt.edu.controllers
{
    /// <summary>
    /// Controller qu?n l� Vehicle Check-in
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleCheckinController : ControllerBase
    {
        private readonly IVehicleCheckinService _vehicleCheckinService;

        public VehicleCheckinController(IVehicleCheckinService vehicleCheckinService)
        {
            _vehicleCheckinService = vehicleCheckinService;
        }

        /// <summary>
        /// T?o m?i vehicle check-in
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVehicleCheckin([FromBody] VehicleCheckinRequestDto request)
        {
            try
            {
                // Get current user ID from JWT claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                long? createdByUserId = long.TryParse(userIdClaim, out var parsedUserId) ? parsedUserId : null;
                
                var result = await _vehicleCheckinService.CreateVehicleCheckinAsync(request, createdByUserId);
                return Ok(new { success = true, data = result, message = "Vehicle check-in created successfully" });
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
        /// C?p nh?t vehicle check-in
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicleCheckin(long id, [FromBody] UpdateDto request)
        {
            try
            {
                if (id != request.Id)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var result = await _vehicleCheckinService.UpdateVehicleCheckinAsync(request);
                return Ok(new { success = true, data = result, message = "Vehicle check-in updated successfully" });
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
        /// L?y chi ti?t vehicle check-in theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleCheckinById(long id)
        {
            try
            {
                var result = await _vehicleCheckinService.GetVehicleCheckinByIdAsync(id);
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
        /// L?y danh s�ch t?t c? vehicle check-in (c� ph�n trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleCheckins(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? statusCode = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var result = await _vehicleCheckinService.GetAllVehicleCheckinsAsync(page, pageSize, searchTerm, statusCode, fromDate, toDate);
                var totalCount = await _vehicleCheckinService.GetTotalCountAsync(searchTerm, statusCode, fromDate, toDate);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                
                return Ok(new { 
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

        /// <summary>
        /// L?y danh s�ch vehicle check-in theo Car ID
        /// </summary>
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetVehicleCheckinsByCarId(long carId)
        {
            try
            {
                var result = await _vehicleCheckinService.GetVehicleCheckinsByCarIdAsync(carId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// L?y danh s�ch vehicle check-in theo Maintenance Request ID
        /// </summary>
        [HttpGet("maintenance-request/{maintenanceRequestId}")]
        public async Task<IActionResult> GetVehicleCheckinsByMaintenanceRequestId(long maintenanceRequestId)
        {
            try
            {
                var result = await _vehicleCheckinService.GetVehicleCheckinsByMaintenanceRequestIdAsync(maintenanceRequestId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// X�a vehicle check-in
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicleCheckin(long id)
        {
            try
            {
                var result = await _vehicleCheckinService.DeleteVehicleCheckinAsync(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Vehicle check-in deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Vehicle check-in not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm xe theo biển số hoặc số khung
        /// </summary>
        [HttpGet("search-vehicle")]
        public async Task<IActionResult> SearchVehicle([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { success = false, message = "Search term is required" });

                var result = await _vehicleCheckinService.SearchVehicleAsync(searchTerm);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Link vehicle check-in v?i maintenance request
        /// </summary>
        [HttpPut("{id}/link-maintenance")]
        public async Task<IActionResult> LinkMaintenanceRequest(long id, [FromBody] LinkMaintenanceRequestDto request)
        {
            try
            {
                var result = await _vehicleCheckinService.LinkMaintenanceRequestAsync(id, request.MaintenanceRequestId);
                return Ok(new { success = true, data = result, message = "Vehicle check-in linked to maintenance request successfully" });
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
        /// Cập nhật trạng thái VehicleCheckin (chỉ PENDING hoặc CONFIRMED)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatusDto request)
        {
            try
            {
                // Validate status
                if (request.StatusCode != "PENDING" && request.StatusCode != "CONFIRMED")
                {
                    return BadRequest(new { success = false, message = "Trạng thái chỉ được phép là PENDING hoặc CONFIRMED" });
                }

                var result = await _vehicleCheckinService.UpdateStatusAsync(id, request.StatusCode);
                return Ok(new { success = true, data = result, message = "Cập nhật trạng thái thành công" });
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
    }

    /// <summary>
    /// DTO cho vi?c link maintenance request
    /// </summary>
    public class LinkMaintenanceRequestDto
    {
        [Required(ErrorMessage = "Maintenance Request ID is required")]
        public long MaintenanceRequestId { get; set; }
    }

    /// <summary>
    /// DTO cho việc cập nhật trạng thái
    /// </summary>
    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "Status code is required")]
        public string StatusCode { get; set; } = string.Empty;
    }
}
