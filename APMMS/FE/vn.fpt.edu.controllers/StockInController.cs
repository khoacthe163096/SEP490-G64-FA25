using FE.vn.fpt.edu.services;
using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("StockIns")]
    public class StockInController : Controller
    {
        private readonly StockInService _service;

        public StockInController(StockInService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("~/vn.fpt.edu.views/StockIns/Index.cshtml");
        }

        [HttpGet]
        [Route("ListData")]
        public async Task<IActionResult> ListData(int page = 1, int pageSize = 10, string? search = null, string? statusCode = null)
        {
            // Lấy branchId của user đang đăng nhập
            long? branchId = null;

            var branchIdFromSession = HttpContext.Session.GetString("BranchId");
            if (!string.IsNullOrEmpty(branchIdFromSession) && long.TryParse(branchIdFromSession, out var sessionBranchId))
            {
                branchId = sessionBranchId;
            }
            else
            {
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (long.TryParse(branchIdClaim, out var claimBranchId))
                {
                    branchId = claimBranchId;
                }
            }

            var result = await _service.GetAllAsync(page, pageSize, search, statusCode, branchId);
            return Json(result);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/vn.fpt.edu.views/StockIns/Create.cshtml");
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            ViewBag.StockIn = result;
            return View("~/vn.fpt.edu.views/StockIns/Details.cshtml");
        }

        [HttpGet]
        [Route("{id}/Data")]
        public async Task<IActionResult> GetData(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Json(result);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] object data)
        {
            var result = await _service.CreateAsync(data);
            return Json(result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] object data)
        {
            var result = await _service.UpdateAsync(id, data);
            return Json(result);
        }

        [HttpPost]
        [Route("{id}/Approve")]
        public async Task<IActionResult> Approve(long id, [FromBody] object data)
        {
            var result = await _service.ApproveAsync(id, data);
            return Json(result);
        }

        [HttpPut]
        [Route("{id}/QuantityAfterCheck")]
        public async Task<IActionResult> UpdateQuantityAfterCheck(long id, [FromBody] object data)
        {
            var result = await _service.UpdateQuantityAfterCheckAsync(id, data);
            return Json(result);
        }

        [HttpPost]
        [Route("{id}/Cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            var result = await _service.CancelAsync(id);
            return Json(result);
        }

        [HttpPatch]
        [Route("{id}/Status")]
        public async Task<IActionResult> ChangeStatus(long id, [FromQuery] string statusCode)
        {
            var result = await _service.ChangeStatusAsync(id, statusCode);
            return Json(result);
        }
    }
}

