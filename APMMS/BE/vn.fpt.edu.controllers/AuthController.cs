using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.vn.fpt.edu.models;
using DAL.vn.fpt.edu.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly CarMaintenanceDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, CarMaintenanceDbContext db, IPasswordHasher<ApplicationUser> passwordHasher, IConfiguration configuration)
        {
            _authService = authService;
            _db = db;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            Console.WriteLine($"Backend Login attempt for user: {dto.Username}");
            
            var result = await _authService.LoginAsync(dto.Username, dto.Password, ct);
            Console.WriteLine($"Backend AuthService result: Success={result.Success}, Token={result.Token?.Substring(0, Math.Min(50, result.Token?.Length ?? 0))}...");
            
            if (!result.Success)
            {
                Console.WriteLine($"Backend Login failed for user: {dto.Username}, Error: {result.Error}");
                return Unauthorized(new LoginResponseDto { Success = false, Error = result.Error });
            }
            
            // Get role ID from JWT token
            var roleId = GetRoleIdFromToken(result.Token) ?? 7; // Default to Auto Owner
            var redirectTo = GetRedirectUrl(roleId);
            
            Console.WriteLine($"Backend Login successful for user: {dto.Username}, Role: {roleId}, Redirect: {redirectTo}");
            
            return Ok(new LoginResponseDto { 
                Success = true, 
                Token = result.Token,
                RoleId = roleId,
                RedirectTo = redirectTo
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = User?.Identity?.Name ?? User?.FindFirst("sub")?.Value ?? string.Empty;
            await _authService.LogoutAsync(userId, ct);
            return NoContent();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(dto, ct);
            if (!result.Success)
            {
                if (string.Equals(result.Error, "Username already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("rehash-passwords")]
        [AllowAnonymous]
        public async Task<IActionResult> RehashPasswords([FromHeader(Name = "X-Rehash-Key")] string? key, CancellationToken ct)
        {
            var expected = _configuration["AdminMaintenance:RehashKey"];
            if (string.IsNullOrEmpty(expected) || key != expected)
            {
                return Unauthorized();
            }

            var users = await _db.Set<ApplicationUser>().ToListAsync(ct);
            int updated = 0;

            foreach (var u in users)
            {
                if (string.IsNullOrEmpty(u.PasswordHash)) continue;

                var isHashed = true;
                try
                {
                    _ = _passwordHasher.VerifyHashedPassword(u, u.PasswordHash, "test");
                }
                catch
                {
                    isHashed = false;
                }

                if (!isHashed)
                {
                    var plaintext = u.PasswordHash;
                    u.PasswordHash = _passwordHasher.HashPassword(u, plaintext);
                    updated++;
                }
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { updated });
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
                Console.WriteLine($"Backend AuthController: role_id claim found: {roleIdClaim?.Value}");
                if (roleIdClaim != null && int.TryParse(roleIdClaim.Value, out int roleId))
                {
                    Console.WriteLine($"Backend AuthController: Parsed role ID: {roleId}");
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
                Console.WriteLine($"Backend Error decoding JWT token: {ex.Message}");
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


