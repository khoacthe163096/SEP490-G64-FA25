using BE.vn.fpt.edu.DTOs.Auth;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<LogoutResponseDto> LogoutAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task<LoginResponseDto> RefreshTokenAsync(string token);
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto dto);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto dto);


    }
}
