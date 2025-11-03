using BE.models;
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

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .Include(c => c.StatusCodeNavigation)
                .ToListAsync();
        }

        public async Task<Component?> GetByIdAsync(long id)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .Include(c => c.StatusCodeNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Component> AddAsync(Component entity)
        {
            _context.Components.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Component> UpdateAsync(Component entity)
        {
            _context.Components.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _context.Components.FindAsync(id);
            if (existing == null) return false;

            _context.Components.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
