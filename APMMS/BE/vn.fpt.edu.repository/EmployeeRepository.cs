using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public EmployeeRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .Where(u => u.RoleId >= 3 && u.RoleId <= 6 && (u.IsDelete == null || u.IsDelete == false))
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == id && u.RoleId >= 3 && u.RoleId <= 6);
        }

        public async Task AddAsync(User employee)
        {
            await _context.Users.AddAsync(employee);
        }

        public async Task UpdateAsync(User employee)
        {
            _context.Users.Update(employee);
            await Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(User employee)
        {
            employee.IsDelete = true;
            _context.Users.Update(employee);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

