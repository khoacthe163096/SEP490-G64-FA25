using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly CarMaintenanceDbContext _context;

        public BranchController(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var branches = await _context.Branches.ToListAsync();
                return Ok(new { success = true, data = branches });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
