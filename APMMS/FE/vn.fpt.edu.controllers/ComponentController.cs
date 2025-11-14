using FE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("Components")]
    public class ComponentController : Controller
    {
        private readonly ComponentService _service;
        public ComponentController(ComponentService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, long? branchId, long? typeComponentId, string? statusCode)
        {
            var items = await _service.GetAllAsync(search, branchId, typeComponentId, statusCode);
            var types = await _service.GetTypeComponentsAsync();
            var vm = new ComponentIndexViewModel
            {
                Items = items,
                TypeComponents = types,
                Search = search,
                TypeComponentFilterId = typeComponentId,
                StatusCode = statusCode
            };
            return View("~/vn.fpt.edu.views/Components/Index.cshtml", vm);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.TypeComponents = await _service.GetTypeComponentsAsync();
            return View("~/vn.fpt.edu.views/Components/Create.cshtml", new ComponentViewModel());
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ComponentViewModel model)
        {
            var created = await _service.CreateAsync(model);
            return Json(new { success = created != null, item = created });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.TypeComponents = await _service.GetTypeComponentsAsync();
            var item = await _service.GetByIdAsync(id);
            return View("~/vn.fpt.edu.views/Components/Edit.cshtml", item);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(long id, ComponentViewModel model)
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
