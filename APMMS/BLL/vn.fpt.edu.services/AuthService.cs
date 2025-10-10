using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;

namespace BLL.vn.fpt.edu.services
{
    public class AuthService : IAuthService
    {
        private readonly CarMaintenanceDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(CarMaintenanceDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsDelete != true);

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
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                var user = new User
                {
                    Username = registerDto.Username,
                    Password = HashPassword(registerDto.Password),
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Phone = registerDto.Phone,
                    RoleId = registerDto.RoleId ?? 2, // Default role ID
                    StatusCode = "ACTIVE",
                    IsDelete = false,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    UserId = user.Id,
                    Username = user.Username
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<LogoutResponseDto> LogoutAsync(string token)
        {
            // In a real application, you would add the token to a blacklist
            // For now, we'll just return success
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
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == long.Parse(userId) && u.IsDelete != true);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

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
            // Hỗ trợ cả plain text và hashed password
            var hashedInput = HashPassword(password);
            
            // Thử so sánh với hashed password trước
            if (hashedInput == storedPassword)
                return true;
                
            // Nếu không khớp, thử so sánh với plain text
            if (password == storedPassword)
                return true;
                
            return false;
        }
    }
}
