using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentController : ControllerBase
    {
        private readonly IComponentService _service;
        public ComponentController(IComponentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false, [FromQuery] long? typeComponentId = null, [FromQuery] long? branchId = null)
        {
            var list = await _service.GetAllAsync(onlyActive, typeComponentId, branchId);
            return Ok(list);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> SetStatus(long id, [FromQuery] bool isActive)
        {
            await _service.SetActiveAsync(id, isActive);
            return NoContent();
        }
    }
}