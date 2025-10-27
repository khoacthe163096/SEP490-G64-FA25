using BE.vn.fpt.edu.DTOs;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.interfaces;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("{id:long}/toggle")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var ok = await _service.ToggleStatusAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}