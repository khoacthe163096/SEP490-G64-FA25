using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.DTOs.Component;
using BE.vn.fpt.edu.interfaces;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentController : ControllerBase
    {
        private readonly IComponentService _componentService;

        public ComponentController(IComponentService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? typeComponentId = null, [FromQuery] long? branchId = null)
        {
            try
            {
                var (components, pagination) = await _componentService.GetAllComponentsAsync(page, pageSize, typeComponentId, branchId);
                return Ok(new { success = true, data = components, pagination = pagination });
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
                var result = await _componentService.GetComponentByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Component not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("type/{typeComponentId}")]
        public async Task<IActionResult> GetByTypeComponent(long typeComponentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (components, pagination) = await _componentService.GetComponentsByTypeComponentIdAsync(typeComponentId, page, pageSize);
                return Ok(new { success = true, data = components, pagination = pagination });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComponentDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
                }

                var result = await _componentService.CreateComponentAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateComponentDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
                }

                var result = await _componentService.UpdateComponentAsync(id, request);
                return Ok(new { success = true, data = result, message = "Component updated successfully" });
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
                var result = await _componentService.DeleteComponentAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Component not found" });
                }

                return Ok(new { success = true, message = "Component deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(long id, [FromBody] UpdateStockDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
                }

                var result = await _componentService.UpdateStockAsync(id, request);
                return Ok(new { success = true, data = result, message = "Stock updated successfully" });
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
}