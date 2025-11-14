using BE.vn.fpt.edu.DTOs.CarOfAutoOwner;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarOfAutoOwnerController : ControllerBase
    {
        private readonly ICarOfAutoOwnerService _service;

        public CarOfAutoOwnerController(ICarOfAutoOwnerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound("Car not found.");
            return Ok(result);
        }

        [HttpGet("user/{userId:long}")]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe đã từng được bảo dưỡng của user
        /// </summary>
        [HttpGet("user/{userId:long}/serviced")]
        public async Task<IActionResult> GetServicedCarsByUserId(long userId)
        {
            var result = await _service.GetServicedCarsByUserIdAsync(userId);
            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Request data is required." });
                }

                if (!dto.UserId.HasValue || dto.UserId.Value <= 0)
                {
                    return BadRequest(new { message = "UserId is required." });
                }

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Handle database constraint violations
                var errorMessage = "An error occurred while saving to the database.";
                
                if (ex.InnerException != null)
                {
                    var innerMessage = ex.InnerException.Message;
                    if (innerMessage.Contains("UNIQUE KEY constraint") || innerMessage.Contains("duplicate key"))
                    {
                        errorMessage = "Biển số xe đã tồn tại trong hệ thống. Vui lòng sử dụng biển số khác.";
                    }
                    else if (innerMessage.Contains("FOREIGN KEY constraint"))
                    {
                        if (innerMessage.Contains("user_id"))
                        {
                            errorMessage = "Người dùng không tồn tại trong hệ thống.";
                        }
                        else if (innerMessage.Contains("vehicle_type_id"))
                        {
                            errorMessage = "Loại phương tiện không tồn tại trong hệ thống.";
                        }
                        else if (innerMessage.Contains("branch_id"))
                        {
                            errorMessage = "Chi nhánh không tồn tại trong hệ thống.";
                        }
                        else
                        {
                            errorMessage = "Dữ liệu tham chiếu không hợp lệ.";
                        }
                    }
                    else
                    {
                        errorMessage = innerMessage;
                    }
                }
                
                return StatusCode(500, new { message = errorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the car.", error = ex.Message });
            }
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
                return NotFound("Car not found or already deleted.");
            return NoContent();
        }
    }
}
