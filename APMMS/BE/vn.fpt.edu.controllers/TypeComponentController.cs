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
        public async Task<ActionResult<IEnumerable<ResponseDto>>> GetAll([FromQuery] long? branchId, [FromQuery] string? statusCode, [FromQuery] string? search)
        {
            var list = await _service.GetAllAsync(branchId, statusCode, search);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> Get(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] RequestDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto>> Update(long id, [FromBody] RequestDto dto)
        {
            dto.Id = id;
            var updated = await _service.UpdateAsync(dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatus(long id, [FromQuery] string statusCode)
        {
            await _service.DisableEnableAsync(id, statusCode);
            return NoContent();
        }
    }
}