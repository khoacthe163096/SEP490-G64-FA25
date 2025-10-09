namespace FE.vn.fpt.edu.models
{
    public class LoginRequestModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }
        public int RoleId { get; set; }
        public string? RedirectTo { get; set; }
    }

    public class RegisterRequestModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class RegisterResponseModel
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Error { get; set; }
    }
}
