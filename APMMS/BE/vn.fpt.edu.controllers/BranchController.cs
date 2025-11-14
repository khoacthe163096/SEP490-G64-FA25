using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.DTOs.Branch;
using BE.vn.fpt.edu.interfaces;
using System.Linq;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var branches = await _branchService.GetAllAsync();
                return Ok(new { success = true, data = branches });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var branch = await _branchService.GetByIdAsync(id);
                if (branch == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy chi nhánh" });
                }
                return Ok(new { success = true, data = branch });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BranchRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorsByField = ModelState
                        .Where(ms => ms.Value != null && ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => 
                                string.IsNullOrEmpty(e.ErrorMessage) 
                                    ? e.Exception?.Message ?? "Invalid value" 
                                    : e.ErrorMessage).ToArray()
                        );
                    
                    var errorsList = errorsByField
                        .SelectMany(kvp => kvp.Value.Select(e => e))
                        .ToList();
                    
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = errorsByField, errorsList = errorsList });
                }

                var branch = await _branchService.CreateAsync(dto);
                return Ok(new { success = true, data = branch, message = "Tạo chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] BranchRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorsByField = ModelState
                        .Where(ms => ms.Value != null && ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => 
                                string.IsNullOrEmpty(e.ErrorMessage) 
                                    ? e.Exception?.Message ?? "Invalid value" 
                                    : e.ErrorMessage).ToArray()
                        );
                    
                    var errorsList = errorsByField
                        .SelectMany(kvp => kvp.Value.Select(e => e))
                        .ToList();
                    
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = errorsByField, errorsList = errorsList });
                }

                var branch = await _branchService.UpdateAsync(id, dto);
                if (branch == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy chi nhánh" });
                }

                return Ok(new { success = true, data = branch, message = "Cập nhật chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _branchService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy chi nhánh" });
                }

                return Ok(new { success = true, message = "Xóa chi nhánh thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
