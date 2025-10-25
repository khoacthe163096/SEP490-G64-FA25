using BE.vn.fpt.edu.DTOs;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeComponentController : ControllerBase
    {
        private readonly ITypeComponentService _service;
        public TypeComponentController(ITypeComponentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var result = await _service.GetAllAsync(onlyActive);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TypeComponentRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] TypeComponentRequestDto dto)
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