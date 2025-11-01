using AutoMapper;
using BE.vn.fpt.edu.DTOs.ServiceTask;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceTaskController : ControllerBase
    {
        private readonly IServiceTaskService _serviceTaskService;
        private readonly IMapper _mapper;

        public ServiceTaskController(IServiceTaskService serviceTaskService, IMapper mapper)
        {
            _serviceTaskService = serviceTaskService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo ServiceTask mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateServiceTask([FromBody] ServiceTaskRequestDto request)
        {
            try
            {
                var result = await _serviceTaskService.CreateServiceTaskAsync(request);
                return Ok(new { success = true, data = result, message = "Service task created successfully" });
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
        /// Cập nhật ServiceTask
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceTask(long id, [FromBody] ServiceTaskUpdateDto request)
        {
            try
            {
                if (id != request.Id)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var result = await _serviceTaskService.UpdateServiceTaskAsync(request);
                return Ok(new { success = true, data = result, message = "Service task updated successfully" });
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
        /// Lấy ServiceTask theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceTaskById(long id)
        {
            try
            {
                var result = await _serviceTaskService.GetServiceTaskByIdAsync(id);
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
        /// Lấy danh sách tất cả ServiceTasks
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllServiceTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _serviceTaskService.GetAllServiceTasksAsync(page, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy ServiceTasks theo MaintenanceTicket ID
        /// </summary>
        [HttpGet("by-ticket/{maintenanceTicketId}")]
        public async Task<IActionResult> GetServiceTasksByMaintenanceTicketId(long maintenanceTicketId)
        {
            try
            {
                var result = await _serviceTaskService.GetServiceTasksByMaintenanceTicketIdAsync(maintenanceTicketId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy ServiceTasks theo Status
        /// </summary>
        [HttpGet("by-status/{statusCode}")]
        public async Task<IActionResult> GetServiceTasksByStatus(string statusCode)
        {
            try
            {
                var result = await _serviceTaskService.GetServiceTasksByStatusAsync(statusCode);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật Status của ServiceTask
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] ServiceTaskUpdateStatusDto request)
        {
            try
            {
                var result = await _serviceTaskService.UpdateStatusAsync(id, request.StatusCode);
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
        /// Lấy ServiceTasks theo Technician ID
        /// </summary>
        [HttpGet("by-technician/{technicianId}")]
        public async Task<IActionResult> GetServiceTasksByTechnicianId(long technicianId)
        {
            try
            {
                var result = await _serviceTaskService.GetServiceTasksByTechnicianIdAsync(technicianId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa ServiceTask
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceTask(long id)
        {
            try
            {
                var result = await _serviceTaskService.DeleteServiceTaskAsync(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Service task deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Service task not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}
