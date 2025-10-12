using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================================
        // 🔐 LOGIN
        // ============================================
        /// <summary>
        /// Đăng nhập hệ thống và nhận JWT token
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 400)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 500)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return BadRequest(new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }

            var response = new AuthResponseWrapperDto<object>
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    result.Token,
                    result.UserId,
                    result.Username,
                    result.RoleName,
                    result.RoleId
                }
            };

            return Ok(response);
        }

        // ============================================
        // 🧾 REGISTER
        // ============================================
        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 400)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new AuthResponseWrapperDto<object>
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    result.UserId,
                    result.Username
                }
            });
        }

        // ============================================
        // 🚪 LOGOUT
        // ============================================
        /// <summary>
        /// Đăng xuất (thu hồi token)
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 400)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 500)]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = "Token not provided"
                });
            }

            var result = await _authService.LogoutAsync(token);

            return Ok(new AuthResponseWrapperDto<object>
            {
                Success = true,
                Message = result.Message
            });
        }

        // ============================================
        // 🔄 REFRESH TOKEN
        // ============================================
        /// <summary>
        /// Cấp mới JWT token khi token cũ còn hạn
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 400)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 500)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.Token);

            if (!result.Success)
            {
                return BadRequest(new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new AuthResponseWrapperDto<object>
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    result.Token,
                    result.UserId,
                    result.Username,
                    result.RoleName,
                    result.RoleId
                }
            });
        }

        // ============================================
        // 🧠 VALIDATE TOKEN
        // ============================================
        /// <summary>
        /// Kiểm tra token có hợp lệ không
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 500)]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenDto dto)
        {
            var isValid = await _authService.ValidateTokenAsync(dto.Token);

            return Ok(new AuthResponseWrapperDto<object>
            {
                Success = true,
                Message = isValid ? "Token is valid" : "Token is invalid",
                Data = new { isValid }
            });
        }
    }

    // ============================================
    // 📦 SMALL DTOs
    // ============================================
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
