using FE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("ServicePackages")]
    public class ServicePackageController : Controller
    {
        private readonly ServicePackageService _service;

        public ServicePackageController(ServicePackageService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, string? statusCode)
        {
            var items = await _service.GetAllAsync(search, statusCode);
            var vm = new ServicePackageIndexViewModel
            {
                Items = items,
                Search = search,
                StatusCode = statusCode
            };
            return View("~/vn.fpt.edu.views/ServicePackages/Index.cshtml", vm);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/vn.fpt.edu.views/ServicePackages/Create.cshtml", new ServicePackageViewModel());
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ServicePackageViewModel model)
        {
            var created = await _service.CreateAsync(model);
            return Json(new { success = created != null, item = created });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _service.GetByIdAsync(id);
            return View("~/vn.fpt.edu.views/ServicePackages/Edit.cshtml", item);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(long id, ServicePackageViewModel model)
        {
            var success = await _service.UpdateAsync(id, model);
            return Json(new { success });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var item = await _service.GetByIdAsync(id);
            return View("~/vn.fpt.edu.views/ServicePackages/Details.cshtml", item);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);
            return Json(new { success });
        }
    }
}
