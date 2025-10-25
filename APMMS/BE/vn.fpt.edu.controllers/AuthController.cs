using BE.vn.fpt.edu.DTOs.Auth;
using BE.vn.fpt.edu.interfaces;
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


    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }
    
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly EmailService _emailService;

        public AuthController(IUserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        // B1: Quên mật khẩu (gửi email xác nhận)
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { success = false, message = "Email không tồn tại." });

            // Tạo mã reset
            var token = Guid.NewGuid().ToString();

            // Lưu token vào DB
            await _userService.SaveResetTokenAsync(user.Id, token);

            // Tạo link reset (ví dụ localhost, bạn thay domain thật)
            var resetLink = $"https://localhost:5173/reset-password?token={token}";

            // Gửi mail
            await _emailService.SendEmailAsync(model.Email, "Đặt lại mật khẩu",
                $"Nhấn vào liên kết sau để đặt lại mật khẩu: {resetLink}");

            return Ok(new { success = true, message = "Đã gửi link đặt lại mật khẩu qua email." });
        }

        // B2: Reset mật khẩu
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _userService.ResetPasswordAsync(model.Token, model.NewPassword);
            if (!result)
                return BadRequest(new { success = false, message = "Token không hợp lệ hoặc đã hết hạn." });

            return Ok(new { success = true, message = "Đặt lại mật khẩu thành công." });
        }
    }

}
