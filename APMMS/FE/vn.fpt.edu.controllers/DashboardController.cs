using Microsoft.AspNetCore.Mvc;

namespace FE.vn.fpt.edu.controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // ✅ Kiểm tra quyền truy cập - chỉ cho phép nhân viên (role 1-6)
            var roleIdStr = HttpContext.Session.GetString("RoleId");
            if (string.IsNullOrEmpty(roleIdStr))
            {
                // Chưa đăng nhập, redirect về login
                return RedirectToAction("Login", "Auth");
            }

            if (!int.TryParse(roleIdStr, out var roleId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ Chặn AutoOwner (role 7) và Guest (role 8) - không được vào Dashboard
            if (roleId == 7 || roleId == 8)
            {
                // Redirect về Profile hoặc Home
                return RedirectToAction("Index", "Profile");
            }

            // ✅ Chỉ cho phép nhân viên (role 1-6)
            if (roleId < 1 || roleId > 6)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        public IActionResult Profile()
        {
            ViewBag.ApiBaseUrl = "https://localhost:7173/api";
            return View();
        }
    }
}
