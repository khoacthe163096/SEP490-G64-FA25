using Microsoft.AspNetCore.Mvc;
using BLL.vn.fpt.edu.DTOs.VehicleCheckin;
using BLL.vn.fpt.edu.interfaces;
using System.ComponentModel.DataAnnotations;

namespace vn.fpt.edu.controllers
{
    /// <summary>
    /// Controller quản lý Vehicle Check-in
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
        /// Tạo mới vehicle check-in
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVehicleCheckin([FromBody] VehicleCheckinRequestDto request)
        {
            try
            {
                var result = await _vehicleCheckinService.CreateVehicleCheckinAsync(request);
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
        /// Cập nhật vehicle check-in
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
        /// Lấy chi tiết vehicle check-in theo ID
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
        /// Lấy danh sách tất cả vehicle check-in (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleCheckins([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _vehicleCheckinService.GetAllVehicleCheckinsAsync(page, pageSize);
                return Ok(new { success = true, data = result, page = page, pageSize = pageSize });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách vehicle check-in theo Car ID
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
        /// Lấy danh sách vehicle check-in theo Maintenance Request ID
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
        /// Xóa vehicle check-in
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
        /// Link vehicle check-in với maintenance request
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
    }

    /// <summary>
    /// DTO cho việc link maintenance request
    /// </summary>
    public class LinkMaintenanceRequestDto
    {
        [Required(ErrorMessage = "Maintenance Request ID is required")]
        public long MaintenanceRequestId { get; set; }
    }
}