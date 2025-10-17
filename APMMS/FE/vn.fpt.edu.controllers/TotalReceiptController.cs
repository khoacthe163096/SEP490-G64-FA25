using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    [Route("TotalReceipts")]
    public class TotalReceiptController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
			return View("~/vn.fpt.edu.views/TotalReceipts/Index.cshtml");
        }
    }
}


