using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.interfaces;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeComponentController : ControllerBase
    {
        private readonly ITypeComponentService _typeComponentService;

        public TypeComponentController(ITypeComponentService typeComponentService)
        {
            _typeComponentService = typeComponentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _typeComponentService.GetAllTypeComponentsAsync();
                return Ok(new { success = true, data = result });
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
                var result = await _typeComponentService.GetTypeComponentByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Type component not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTypeComponentDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
                }

                var result = await _typeComponentService.CreateTypeComponentAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTypeComponentDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
                }

                var result = await _typeComponentService.UpdateTypeComponentAsync(id, request);
                return Ok(new { success = true, data = result, message = "Type component updated successfully" });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _typeComponentService.DeleteTypeComponentAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Type component not found" });
                }

                return Ok(new { success = true, message = "Type component deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}