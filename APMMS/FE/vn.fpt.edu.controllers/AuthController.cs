using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using FE.vn.fpt.edu.services;
using FE.vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View("~/vn.fpt.edu.views/Auth/Login.cshtml");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            try
            {
                Console.WriteLine($"Login attempt for user: {request.Username}");
                
                var result = await _authService.LoginAsync(request.Username, request.Password);
                Console.WriteLine($"AuthService result: Success={result?.Success}, Token={result?.Token?.Substring(0, Math.Min(50, result.Token?.Length ?? 0))}...");
                
                if (result?.Success == true)
                {
                    // Store token in session or cookie
                    HttpContext.Session.SetString("AuthToken", result.Token ?? "");
                    HttpContext.Session.SetString("Username", request.Username);
                    
                    // Use roleId from the response
                    var roleId = result.RoleId;
                    HttpContext.Session.SetString("RoleId", roleId.ToString());
                    
                    Console.WriteLine($"Login successful for user: {request.Username}, Role: {roleId}, UserId: {result.UserId}");
                    return Json(new { 
                        success = true, 
                        token = result.Token,
                        userId = result.UserId,
                        roleId = roleId,
                        redirectTo = result.RedirectTo ?? GetRedirectUrl(roleId)
                    });
                }
                
                Console.WriteLine($"Login failed for user: {request.Username}, Error: {result?.Error}");
                return Json(new { success = false, error = result?.Error ?? "Đăng nhập thất bại" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex.Message}");
                return Json(new { success = false, error = "Lỗi kết nối: " + ex.Message });
            }
        }

        private string GetRedirectUrl(int roleId)
        {
            return roleId switch
            {
                1 => "/Dashboard", // Admin
                2 => "/Dashboard", // Branch Manager
                3 => "/Dashboard", // Accountant
                4 => "/Dashboard", // Technician
                5 => "/Dashboard", // Warehouse Keeper
                6 => "/Dashboard", // Consulter
                7 => "/", // Auto Owner - stay on home page
                8 => "/", // Guest - stay on home page
                _ => "/"
            };
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            
            // Sign out cookie
            await HttpContext.SignOutAsync("CookieAuthentication");
            
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> LogoutPost()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Sign out cookie
            await HttpContext.SignOutAsync("CookieAuthentication");

            return Json(new { success = true, redirectTo = Url.Action("Login", "Auth") });
        }

        [HttpGet]
        [Route("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var username = HttpContext.Session.GetString("Username") ?? string.Empty;
            var roleIdString = HttpContext.Session.GetString("RoleId");
            int roleId = 0;
            if (!string.IsNullOrEmpty(roleIdString))
            {
                int.TryParse(roleIdString, out roleId);
            }

            var isLoggedIn = !string.IsNullOrEmpty(token);
            return Json(new { isLoggedIn, username, roleId });
        }
    }
}
