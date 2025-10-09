using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.vn.fpt.edu.services
{
    public class AuthService : IAuthService
    {
        private readonly Func<string, string, CancellationToken, Task<(bool Success, string? UserId, string? RoleName, long? RoleId)>> _verifyCredentialsAsync;
        private readonly Func<string, CancellationToken, Task> _logoutAsync;
        private readonly Func<string, string?, CancellationToken, Task<string>> _generateJwtAsync;
        private readonly Func<string, string?, int?, CancellationToken, Task<string>> _generateJwtWithRoleIdAsync;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<DAL.vn.fpt.edu.entities.ApplicationUser> _passwordHasher;

        public AuthService(
            Func<string, string, CancellationToken, Task<(bool Success, string? UserId, string? RoleName, long? RoleId)>> verifyCredentialsAsync,
            Func<string, CancellationToken, Task> logoutAsync,
            Func<string, string?, CancellationToken, Task<string>> generateJwtAsync,
            Func<string, string?, int?, CancellationToken, Task<string>> generateJwtWithRoleIdAsync,
            IUserRepository userRepository,
            IPasswordHasher<DAL.vn.fpt.edu.entities.ApplicationUser> passwordHasher)
        {
            _verifyCredentialsAsync = verifyCredentialsAsync;
            _logoutAsync = logoutAsync;
            _generateJwtAsync = generateJwtAsync;
            _generateJwtWithRoleIdAsync = generateJwtWithRoleIdAsync;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string? Token, string? Error)> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var verification = await _verifyCredentialsAsync(username, password, cancellationToken);
            if (!verification.Success || string.IsNullOrEmpty(verification.UserId))
            {
                return (false, null, "Invalid username or password");
            }

            // Use role ID from database instead of parsing role name
            var roleId = (int?)(verification.RoleId ?? 7); // Default to Auto Owner if no role
            Console.WriteLine($"AuthService: User {username}, RoleId from DB: {verification.RoleId}, RoleName: {verification.RoleName}, Final RoleId: {roleId}");
            var token = await _generateJwtWithRoleIdAsync(verification.UserId, verification.RoleName, roleId, cancellationToken);
            return (true, token, null);
        }

        public Task LogoutAsync(string userId, CancellationToken cancellationToken = default)
        {
            return _logoutAsync(userId, cancellationToken);
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var username = (request.Username ?? string.Empty).Trim();
            var phone = (request.Phone ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(phone))
            {
                return new RegisterResponseDto { Success = false, Error = "Username, password and phone are required" };
            }

            if (await _userRepository.UsernameExistsAsync(username, cancellationToken))
            {
                return new RegisterResponseDto { Success = false, Error = "Username already exists" };
            }

            long? roleId = 7; // Auto Owner
            var hashed = _passwordHasher.HashPassword(new DAL.vn.fpt.edu.entities.ApplicationUser { UserName = username }, request.Password);

            var (success, userId) = await _userRepository.CreateUserAsync(username, hashed, request.Email, phone, roleId, cancellationToken);
            if (!success)
            {
                return new RegisterResponseDto { Success = false, Error = "Failed to create user" };
            }

            return new RegisterResponseDto { Success = true, UserId = userId, Username = username };
        }
    }
}


