using FE.vn.fpt.edu.services;
using FE.vn.fpt.edu.viewmodels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace FE.vn.fpt.edu.controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public IActionResult TestAdmin()
        {
            // Test admin login without database
            var testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwicm9sZSI6IkFkbWluIiwicm9sZV9pZCI6IjEiLCJleHAiOjk5OTk5OTk5OTl9.test";
            
            HttpContext.Session.SetString("AuthToken", testToken);
            HttpContext.Session.SetString("Username", "admin");
            HttpContext.Session.SetString("RoleId", "1");
            
            return Json(new { 
                success = true, 
                token = testToken,
                roleId = 1,
                redirectTo = "/Dashboard"
            });
        }

        [HttpGet]
        public async Task<IActionResult> TestBackendConnection()
        {
            try
            {
                Console.WriteLine("Testing Backend connection...");
                var result = await _authService.LoginAsync("test", "test");
                Console.WriteLine($"Backend test result: {result?.Success}");
                return Json(new { 
                    success = true, 
                    message = "Backend connection successful",
                    result = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Backend connection test failed: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Backend connection failed",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult CheckSession()
        {
            var username = HttpContext.Session.GetString("Username");
            var roleId = HttpContext.Session.GetString("RoleId");
            var token = HttpContext.Session.GetString("AuthToken");
            
            Console.WriteLine($"Session check - Username: '{username}', RoleId: '{roleId}', Token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");
            
            return Json(new {
                success = true,
                username = username,
                roleId = roleId,
                hasToken = !string.IsNullOrEmpty(token),
                sessionId = HttpContext.Session.Id
            });
        }

        [HttpPost]
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
                    
                    Console.WriteLine($"Login successful for user: {request.Username}, Role: {roleId}");
                    return Json(new { 
                        success = true, 
                        token = result.Token,
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

        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                
                if (result?.Success == true)
                {
                    return Json(new { success = true, message = "Đăng ký thành công" });
                }
                
                return Json(new { success = false, error = result?.Error ?? "Đăng ký thất bại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Lỗi kết nối: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var username = HttpContext.Session.GetString("Username");
            var token = HttpContext.Session.GetString("AuthToken");
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                return Json(new { isLoggedIn = false });
            }
            
            return Json(new { 
                isLoggedIn = true, 
                username = username,
                token = token 
            });
        }

        private int? GetRoleIdFromToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return null;
                
            try
            {
                // Decode JWT token to get role_id claim
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                
                var roleIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "role_id");
                Console.WriteLine($"AuthController: role_id claim found: {roleIdClaim?.Value}");
                if (roleIdClaim != null && int.TryParse(roleIdClaim.Value, out int roleId))
                {
                    Console.WriteLine($"AuthController: Parsed role ID: {roleId}");
                    return roleId;
                }
                
                // Fallback: try to get role from role claim
                var roleClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "role");
                if (roleClaim != null)
                {
                    return GetRoleIdFromRoleName(roleClaim.Value);
                }
                
                return 7; // Default to Auto Owner
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding JWT token: {ex.Message}");
                return 7; // Default to Auto Owner
            }
        }
        
        private static int GetRoleIdFromRoleName(string roleName)
        {
            return roleName.ToLower() switch
            {
                "admin" => 1,
                "branch manager" => 2,
                "accountant" => 3,
                "technician" => 4,
                "warehouse keeper" => 5,
                "consulter" => 6,
                "auto owner" => 7,
                "guest" => 8,
                _ => 7 // Default to Auto Owner
            };
        }

        private string GetRedirectUrl(int roleId)
        {
            // Admin (1), Branch Manager (2), Accountant (3), Technician (4), Warehouse Keeper (5), Consulter (6) -> Dashboard
            if (roleId >= 1 && roleId <= 6)
            {
                return "/Dashboard";
            }
            
            // Auto Owner (7), Guest (8) -> Home
            return "/";
        }
    }
}
