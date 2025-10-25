using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;
using System;
using vn.fpt.edu.models;
using static BE.vn.fpt.edu.models.CarMaintenanceDbContext;


public class UserService : IUserService
{
    private readonly CarMaintenanceDbContext _context;

    public UserService(CarMaintenanceDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveResetTokenAsync(long userId, string token)
    {
        var reset = new CarMaintenanceDbContext.PasswordReset
        {
            UserId = userId,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1)
        };
        _context.PasswordResets.Add(reset);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var reset = await _context.PasswordResets
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token && r.Expiration > DateTime.UtcNow);

        if (reset == null)
            return false;

        reset.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _context.PasswordResets.Remove(reset);
        await _context.SaveChangesAsync();
        return true;
    }
}
