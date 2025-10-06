using BLL.vn.fpt.edu.DTOs.Auth;

namespace BLL.vn.fpt.edu.interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, string? Error)> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
        Task LogoutAsync(string userId, CancellationToken cancellationToken = default);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    }
}


