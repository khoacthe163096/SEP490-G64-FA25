using DAL.vn.fpt.edu.entities;
using DAL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.vn.fpt.edu.repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CarMaintenanceDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UserRepository(CarMaintenanceDbContext db, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string? UserId, string? RoleName, long? RoleId)> VerifyCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var user = await _db.Set<ApplicationUser>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return (false, null, null, null);
            }

            PasswordVerificationResult verify = PasswordVerificationResult.Failed;
            try
            {
                verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            }
            catch
            {
                verify = PasswordVerificationResult.Failed;
            }

            if (verify == PasswordVerificationResult.Failed)
            {
                return (false, null, null, null);
            }

            string? roleName = null;
            if (user.RoleId.HasValue)
            {
                roleName = await _db.Set<ApplicationRole>()
                    .AsNoTracking()
                    .Where(r => r.Id == user.RoleId.Value)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return (true, user.Id.ToString(), roleName, user.RoleId);
        }

        public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return _db.Set<ApplicationUser>().AnyAsync(u => u.UserName == username, cancellationToken);
        }

        public async Task<(bool Success, long UserId)> CreateUserAsync(string username, string hashedPassword, string? email, string phone, long? roleId, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                PasswordHash = hashedPassword,
                Email = email,
                PhoneNumber = phone,
                RoleId = roleId
            };
            _db.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            return (true, user.Id);
        }

        public Task<long?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return _db.Set<ApplicationRole>()
                .Where(r => r.Name == roleName)
                .Select(r => (long?)r.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}


