using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.vn.fpt.edu.data;
using DAL.vn.fpt.edu.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, ApplicationDbContext db, IPasswordHasher<ApplicationUser> passwordHasher, IConfiguration configuration)
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
            var result = await _authService.LoginAsync(dto.Username, dto.Password, ct);
            if (!result.Success)
            {
                return Unauthorized(new LoginResponseDto { Success = false, Error = result.Error });
            }
            return Ok(new LoginResponseDto { Success = true, Token = result.Token });
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
    }
}


