using BE.vn.fpt.edu.models;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task SaveResetTokenAsync(int userId, string token);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
}
