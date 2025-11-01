using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;
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
                .Include(u => u.Address)
                    .ThenInclude(a => a.Ward)
                .Include(u => u.Address)
                    .ThenInclude(a => a.Province)
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
        public IQueryable<User> GetAll()
        {
            return _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .Where(u => u.RoleId >= 3 && u.RoleId <= 6);
        }

        public async Task<List<User>> GetWithFiltersAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, long? roleId = null)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .Where(u => u.RoleId >= 3 && u.RoleId <= 6 && (u.IsDelete == false || u.IsDelete == null));

            // Filter theo role
            if (roleId.HasValue && roleId.Value >= 3 && roleId.Value <= 6)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchLower)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchLower)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                    (u.Phone != null && u.Phone.Contains(search)) ||
                    (u.Username != null && u.Username.ToLower().Contains(searchLower))
                );
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.ToUpper() == "ACTIVE")
                {
                    query = query.Where(u => (u.StatusCode == "ACTIVE" || u.StatusCode == null) && (u.IsDelete == false || u.IsDelete == null));
                }
                else if (status.ToUpper() == "INACTIVE")
                {
                    query = query.Where(u => u.StatusCode == "INACTIVE" || u.IsDelete == true);
                }
            }

            return await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? search = null, string? status = null, long? roleId = null)
        {
            var query = _context.Users
                .Where(u => u.RoleId >= 3 && u.RoleId <= 6 && (u.IsDelete == false || u.IsDelete == null));

            // Filter theo role
            if (roleId.HasValue && roleId.Value >= 3 && roleId.Value <= 6)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchLower)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchLower)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                    (u.Phone != null && u.Phone.Contains(search)) ||
                    (u.Username != null && u.Username.ToLower().Contains(searchLower))
                );
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.ToUpper() == "ACTIVE")
                {
                    query = query.Where(u => (u.StatusCode == "ACTIVE" || u.StatusCode == null) && (u.IsDelete == false || u.IsDelete == null));
                }
                else if (status.ToUpper() == "INACTIVE")
                {
                    query = query.Where(u => u.StatusCode == "INACTIVE" || u.IsDelete == true);
                }
            }

            return await query.CountAsync();
        }


    }
}

