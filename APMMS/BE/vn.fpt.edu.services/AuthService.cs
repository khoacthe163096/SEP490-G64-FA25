using BE.vn.fpt.edu.DTOs.Auth;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BE.vn.fpt.edu.services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthService(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role?.Name ?? "User", user.RoleId ?? 0);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                RoleName = user.Role?.Name,
                RoleId = user.RoleId
            };
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
                return new RegisterResponseDto { Success = false, Message = "Username already exists" };

            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                return new RegisterResponseDto { Success = false, Message = "Email already exists" };

            var user = new User
            {
                Username = registerDto.Username,
                Password = HashPassword(registerDto.Password),
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                RoleId = registerDto.RoleId ?? 2,
                StatusCode = "ACTIVE",
                IsDelete = false,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new RegisterResponseDto
            {
                Success = true,
                Message = "Registration successful",
                UserId = user.Id,
                Username = user.Username
            };
        }

        public async Task<LogoutResponseDto> LogoutAsync(string token)
        {
            // optional: token blacklist
            return new LogoutResponseDto
            {
                Success = true,
                Message = "Logout successful"
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var principal = _jwtService.ValidateToken(token);
            return principal != null;
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string token)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal == null)
                return new LoginResponseDto { Success = false, Message = "Invalid token" };

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new LoginResponseDto { Success = false, Message = "Invalid token" };

            var user = await _userRepository.GetByIdAsync(long.Parse(userId));
            if (user == null)
                return new LoginResponseDto { Success = false, Message = "User not found" };

            var newToken = _jwtService.GenerateToken(user.Id, user.Username, user.Role?.Name ?? "User", user.RoleId ?? 0);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newToken,
                UserId = user.Id,
                Username = user.Username,
                RoleName = user.Role?.Name,
                RoleId = user.RoleId
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == storedPassword || password == storedPassword;
        }
    }
}
