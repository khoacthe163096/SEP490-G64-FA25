using BE.vn.fpt.edu.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByIdWithIncludesAsync(long id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByResetTokenAsync(string resetToken);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task UpdateAsync(User user);
    }
}
