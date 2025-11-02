using FE.vn.fpt.edu.adapters;
using FE.vn.fpt.edu.viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class TypeComponentController : Controller
    {
        private readonly ApiAdapter _api;
        private const string BaseEndpoint = "TypeComponent";

        public TypeComponentController(ApiAdapter api)
        {
            _api = api;
        }

        // GET: /TypeComponent
        public async Task<IActionResult> Index(string? search, long? branchId, string? statusCode)
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
            if (branchId.HasValue) query.Add($"branchId={branchId.Value}");
            if (!string.IsNullOrWhiteSpace(statusCode)) query.Add($"statusCode={Uri.EscapeDataString(statusCode)}");

            var endpoint = BaseEndpoint;
            if (query.Any()) endpoint += "?" + string.Join("&", query);

            var items = await _api.GetAsync<List<TypeComponentViewModel>>(endpoint) ?? new List<TypeComponentViewModel>();

            var vm = new TypeComponentIndexViewModel
            {
                Items = items,
                Search = search,
                BranchId = branchId,
                StatusCode = statusCode
            };

            return View(vm);
        }

        // GET: /TypeComponent/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _api.GetAsync<TypeComponentViewModel>($"{BaseEndpoint}/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /TypeComponent/Create
        public IActionResult Create()
        {
            return PartialView("_CreateEdit", new TypeComponentViewModel());
        }

        // POST: /TypeComponent/Create
        [HttpPost]
        public async Task<IActionResult> Create(TypeComponentViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _api.PostAsync<TypeComponentViewModel>(BaseEndpoint, new
            {
                Name = model.Name,
                Description = model.Description,
                BranchId = model.BranchId,
                StatusCode = model.StatusCode
            });

            if (created == null)
                return Json(new { success = false, message = "Create failed" });

            return Json(new { success = true, item = created });
        }

        // GET: /TypeComponent/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _api.GetAsync<TypeComponentViewModel>($"{BaseEndpoint}/{id}");
            if (item == null) return NotFound();
            return PartialView("_CreateEdit", item);
        }

        // POST: /TypeComponent/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(long id, TypeComponentViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _api.PutAsync($"{BaseEndpoint}/{id}", new
            {
                Id = id,
                Name = model.Name,
                Description = model.Description,
                BranchId = model.BranchId,
                StatusCode = model.StatusCode
            });

            if (!ok)
                return Json(new { success = false, message = "Update failed" });

            return Json(new { success = true });
        }

        // POST: /TypeComponent/ToggleStatus
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(long id, string statusCode)
        {
            // BE endpoint: PUT /api/TypeComponent/{id}/status?statusCode=...
            var ok = await _api.PutAsync($"{BaseEndpoint}/{id}/status?statusCode={statusCode}", new { });
            return Json(new { success = ok });
        }
    }
}
