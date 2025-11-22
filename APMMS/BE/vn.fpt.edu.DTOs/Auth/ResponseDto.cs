namespace BE.vn.fpt.edu.DTOs.Auth
{
    /// <summary>
    /// DTO cho Login Response
    /// </summary>
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public long? UserId { get; set; }
        public string? Username { get; set; }
        public string? RoleName { get; set; }
        public long? RoleId { get; set; }
        public long? BranchId { get; set; }
    }

    /// <summary>
    /// DTO cho Register Response
    /// </summary>
    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? UserId { get; set; }
        public string? Username { get; set; }
    }

    /// <summary>
    /// DTO cho Logout Response
    /// </summary>
    public class LogoutResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    public class ChangePasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
