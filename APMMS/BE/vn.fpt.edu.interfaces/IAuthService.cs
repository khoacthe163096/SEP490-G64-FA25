using BE.vn.fpt.edu.DTOs.Auth;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LogoutResponseDto> LogoutAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task<LoginResponseDto> RefreshTokenAsync(string token);
    }
}
