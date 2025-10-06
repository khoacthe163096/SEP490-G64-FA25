namespace BLL.vn.fpt.edu.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public long? UserId { get; set; }
        public string? Username { get; set; }
    }
}


