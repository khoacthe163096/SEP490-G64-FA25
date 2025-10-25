using BE.vn.fpt.edu.models;

namespace vn.fpt.edu.models
{
    public class PasswordReset
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }

        public User User { get; set; } // navigation property
    }
}

