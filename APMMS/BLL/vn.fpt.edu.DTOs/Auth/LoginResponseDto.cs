namespace BLL.vn.fpt.edu.DTOs.Auth
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }
    }
}


