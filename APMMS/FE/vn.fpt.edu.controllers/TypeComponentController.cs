using FE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("TypeComponents")]
    public class TypeComponentController : Controller
    {
        private readonly TypeComponentService _service;
        public TypeComponentController(TypeComponentService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, long? branchId, string? statusCode)
        {
            var items = await _service.GetAllAsync(search, branchId, statusCode);
            var vm = new TypeComponentIndexViewModel
            {
                Items = items,
                Search = search,
                BranchId = branchId,
                StatusCode = statusCode
            };
            return View("~/vn.fpt.edu.views/TypeComponents/Index.cshtml", vm);
        }

        [HttpGet("Create")]
        public IActionResult Create() => View("~/vn.fpt.edu.views/TypeComponents/Create.cshtml");

        [HttpPost("Create")]
        public async Task<IActionResult> Create(TypeComponentViewModel model)
        {
            var created = await _service.CreateAsync(model);
            return Json(new { success = created != null, item = created });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _service.GetByIdAsync(id);
            return View("~/vn.fpt.edu.views/TypeComponents/Edit.cshtml", item);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(long id, TypeComponentViewModel model)
        {
            var success = await _service.UpdateAsync(id, model);
            return Json(new { success });
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);
            return Json(new { success });
        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(long id, string statusCode)
        {
            var success = await _service.ToggleStatusAsync(id, statusCode);
            return Json(new { success });
        }
    }
}
