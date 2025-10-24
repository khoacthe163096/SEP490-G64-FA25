using Microsoft.AspNetCore.Mvc;
using FE.vn.fpt.edu.services;
using FE.vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("Components")]
    public class ComponentController : Controller
    {
        private readonly ComponentService _componentService;

        public ComponentController(ComponentService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, long? typeComponentId = null, string? status = null)
        {
            try
            {
                var model = await _componentService.GetComponentsAsync(page, pageSize, search, typeComponentId, status);
                return View("~/vn.fpt.edu.views/Components/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/vn.fpt.edu.views/Components/Index.cshtml", new ComponentListViewModel());
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var model = new CreateComponentViewModel();
            return View("~/vn.fpt.edu.views/Components/Create.cshtml", model);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateComponentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/vn.fpt.edu.views/Components/Create.cshtml", model);
            }

            try
            {
                var success = await _componentService.CreateComponentAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo linh kiện thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo linh kiện");
                    return View("~/vn.fpt.edu.views/Components/Create.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("~/vn.fpt.edu.views/Components/Create.cshtml", model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var component = await _componentService.GetComponentByIdAsync(id);
                if (component == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy linh kiện";
                    return RedirectToAction("Index");
                }

                var model = new UpdateComponentViewModel
                {
                    Name = component.Name,
                    Code = component.Code,
                    UnitPrice = component.UnitPrice,
                    QuantityStock = component.QuantityStock,
                    TypeComponentId = component.TypeComponentId ?? 0,
                    BranchId = component.BranchId ?? 0,
                    ImageUrl = component.ImageUrl
                };

                ViewBag.ComponentId = id;
                return View("~/vn.fpt.edu.views/Components/Edit.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id, UpdateComponentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ComponentId = id;
                return View("~/vn.fpt.edu.views/Components/Edit.cshtml", model);
            }

            try
            {
                var success = await _componentService.UpdateComponentAsync(id, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật linh kiện thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật linh kiện");
                    ViewBag.ComponentId = id;
                    return View("~/vn.fpt.edu.views/Components/Edit.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.ComponentId = id;
                return View("~/vn.fpt.edu.views/Components/Edit.cshtml", model);
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var component = await _componentService.GetComponentByIdAsync(id);
                if (component == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy linh kiện";
                    return RedirectToAction("Index");
                }

                ViewBag.ComponentId = id;
                return View("~/vn.fpt.edu.views/Components/Details.cshtml", component);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _componentService.DeleteComponentAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Xóa linh kiện thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa linh kiện";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("UpdateStock/{id}")]
        public async Task<IActionResult> UpdateStock(long id, int quantityStock)
        {
            try
            {
                var success = await _componentService.UpdateStockAsync(id, quantityStock);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật số lượng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật số lượng";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Route("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(long id, string status)
        {
            try
            {
                var success = await _componentService.UpdateStatusAsync(id, status);
                if (success)
                {
                    var statusText = status == "ACTIVE" ? "kích hoạt" : "ngưng cung cấp";
                    TempData["SuccessMessage"] = $"Cập nhật trạng thái thành công! Đã {statusText} linh kiện.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
