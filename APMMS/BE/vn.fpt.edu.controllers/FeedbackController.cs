using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
