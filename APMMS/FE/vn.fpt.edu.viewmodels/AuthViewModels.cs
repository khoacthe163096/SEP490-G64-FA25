using System.ComponentModel.DataAnnotations;

namespace FE.vn.fpt.edu.viewmodels
{
    // Request Models
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // Response Models
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public int RoleId { get; set; }
        public string? RedirectTo { get; set; }
        public string? Error { get; set; }
    }

    public class RegisterResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
