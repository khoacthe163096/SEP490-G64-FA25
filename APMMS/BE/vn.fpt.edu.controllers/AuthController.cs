using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<LoginResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return BadRequest(new AuthResponseWrapperDto<LoginResponseDto>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = null
                    });
                }

                return Ok(new AuthResponseWrapperDto<LoginResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<RegisterResponseDto>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(new AuthResponseWrapperDto<RegisterResponseDto>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = null
                    });
                }

                return Ok(new AuthResponseWrapperDto<RegisterResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<LogoutResponseDto>), 200)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new AuthResponseWrapperDto<LogoutResponseDto>
                    {
                        Success = false,
                        Message = "Token not provided",
                        Data = null
                    });
                }

                var result = await _authService.LogoutAsync(token);

                return Ok(new AuthResponseWrapperDto<LogoutResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<LoginResponseDto>), 200)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto.Token);

                if (!result.Success)
                {
                    return BadRequest(new AuthResponseWrapperDto<LoginResponseDto>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = null
                    });
                }

                return Ok(new AuthResponseWrapperDto<LoginResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Validate token
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(AuthResponseWrapperDto<object>), 200)]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenDto validateTokenDto)
        {
            try
            {
                var isValid = await _authService.ValidateTokenAsync(validateTokenDto.Token);

                return Ok(new AuthResponseWrapperDto<object>
                {
                    Success = true,
                    Message = isValid ? "Token is valid" : "Token is invalid",
                    Data = new { isValid }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseWrapperDto<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
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
}
