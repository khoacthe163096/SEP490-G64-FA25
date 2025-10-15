using AutoMapper;
using BE.vn.fpt.edu.DTOs.MaintenanceTicket;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaintenanceTicketController : ControllerBase
    {
        private readonly IMaintenanceTicketService _maintenanceTicketService;
        private readonly IMapper _mapper;

        public MaintenanceTicketController(IMaintenanceTicketService maintenanceTicketService, IMapper mapper)
        {
            _maintenanceTicketService = maintenanceTicketService;
            _mapper = mapper;
        }

        /// <summary>
        /// T?o Maintenance Ticket m?i
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceTicket([FromBody] RequestDto request)
        {
            try
            {
                var result = await _maintenanceTicketService.CreateMaintenanceTicketAsync(request);
                return Ok(new { success = true, data = result, message = "Maintenance ticket created successfully" });
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
        /// T?o Maintenance Ticket t? Vehicle Check-in
        /// </summary>
        [HttpPost("create-from-checkin")]
        public async Task<IActionResult> CreateFromVehicleCheckin([FromBody] CreateFromCheckinDto request)
        {
            try
            {
                var result = await _maintenanceTicketService.CreateFromVehicleCheckinAsync(request);
                return Ok(new { success = true, data = result, message = "Maintenance ticket created from vehicle check-in successfully" });
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
        /// C?p nh?t Maintenance Ticket
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceTicket(long id, [FromBody] RequestDto request)
        {
            try
            {
                var result = await _maintenanceTicketService.UpdateMaintenanceTicketAsync(id, request);
                return Ok(new { success = true, data = result, message = "Maintenance ticket updated successfully" });
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
        /// L?y Maintenance Ticket theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceTicketById(long id)
        {
            try
            {
                var result = await _maintenanceTicketService.GetMaintenanceTicketByIdAsync(id);
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
        /// L?y danh sách t?t c? Maintenance Tickets
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMaintenanceTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _maintenanceTicketService.GetAllMaintenanceTicketsAsync(page, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// L?y Maintenance Tickets theo Car ID
        /// </summary>
        [HttpGet("by-car/{carId}")]
        public async Task<IActionResult> GetMaintenanceTicketsByCarId(long carId)
        {
            try
            {
                var result = await _maintenanceTicketService.GetMaintenanceTicketsByCarIdAsync(carId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// L?y Maintenance Tickets theo Status
        /// </summary>
        [HttpGet("by-status/{statusCode}")]
        public async Task<IActionResult> GetMaintenanceTicketsByStatus(string statusCode)
        {
            try
            {
                var result = await _maintenanceTicketService.GetMaintenanceTicketsByStatusAsync(statusCode);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// C?p nh?t Status c?a Maintenance Ticket
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatusDto request)
        {
            try
            {
                var result = await _maintenanceTicketService.UpdateStatusAsync(id, request.StatusCode);
                return Ok(new { success = true, data = result, message = "Status updated successfully" });
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
        /// Xóa Maintenance Ticket
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenanceTicket(long id)
        {
            try
            {
                var result = await _maintenanceTicketService.DeleteMaintenanceTicketAsync(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Maintenance ticket deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Maintenance ticket not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}
