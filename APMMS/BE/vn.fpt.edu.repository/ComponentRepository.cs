using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class ComponentRepository : IComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public ComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public IQueryable<Component> GetAll()
        {
            return _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .AsQueryable();
        }

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .ToListAsync();
        }

        public async Task<IEnumerable<Component>> GetAllWithDetailsAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Component?> GetByIdAsync(long id)
        {
            return await _context.Components
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Component?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Component>> GetByTypeComponentIdAsync(long typeComponentId, int page = 1, int pageSize = 10)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .Where(c => c.TypeComponentId == typeComponentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Component>> GetByBranchIdAsync(long branchId, int page = 1, int pageSize = 10)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .Where(c => c.BranchId == branchId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Components.CountAsync();
        }

        public async Task<int> GetCountByTypeComponentIdAsync(long typeComponentId)
        {
            return await _context.Components
                .Where(c => c.TypeComponentId == typeComponentId)
                .CountAsync();
        }

        public async Task<int> GetCountByBranchIdAsync(long branchId)
        {
            return await _context.Components
                .Where(c => c.BranchId == branchId)
                .CountAsync();
        }

        public async Task AddAsync(Component component)
        {
            await _context.Components.AddAsync(component);
        }

        public async Task UpdateAsync(Component component)
        {
            _context.Components.Update(component);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Component component)
        {
            _context.Components.Remove(component);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}