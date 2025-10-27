using BE.vn.fpt.edu.DTOs.AutoOwner;

using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.interfaces;
namespace BE.vn.fpt.edu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoOwnerController : ControllerBase
    {
        private readonly IAutoOwnerService _service;

        public AutoOwnerController(IAutoOwnerService service)
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
                return NotFound("Auto Owner not found.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
                return NotFound("Auto Owner not found or already deleted.");
            return NoContent();
        }
    }
}
