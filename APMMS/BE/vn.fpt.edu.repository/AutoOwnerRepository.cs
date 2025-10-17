using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class AutoOwnerRepository : IAutoOwnerRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public AutoOwnerRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Users
                .Include(u => u.Cars)
                .Include(u => u.Role)
                .Include(u => u.Address)
                .Where(u => (u.IsDelete == false || u.IsDelete == null) &&
                            u.Role != null && u.Role.Name == "Auto Owner")
                .OrderByDescending(u => u.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
                .Include(u => u.Cars)
                .Include(u => u.Role)
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == id && (u.IsDelete == false || u.IsDelete == null));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && (u.IsDelete == false || u.IsDelete == null));
        }

        public async Task<List<User>> GetByCarIdAsync(long carId)
        {
            return await _context.Users
                .Include(u => u.Cars)
                .Where(u => (u.IsDelete == false || u.IsDelete == null) && u.Cars.Any(c => c.Id == carId))
                .ToListAsync();
        }

        public async Task<List<User>> GetByBranchIdAsync(long branchId)
        {
            return await _context.Users
                .Include(u => u.Cars)
                .Where(u => (u.IsDelete == false || u.IsDelete == null) && u.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsDelete = true;
            user.LastModifiedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
