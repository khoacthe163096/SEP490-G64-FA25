using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.repository
{
    public class ComponentRepository : IComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;
        public ComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Component>> GetAllAsync(bool onlyActive = false, long? typeComponentId = null, long? branchId = null)
        {
            IQueryable<Component> q = _context.Components
                .Include(c => c.TypeComponent)
                .AsNoTracking();

            if (onlyActive) q = q.Where(c => c.IsActive);
            if (typeComponentId.HasValue) q = q.Where(c => c.TypeComponentId == typeComponentId.Value);
            if (branchId.HasValue) q = q.Where(c => c.BranchId == branchId.Value);

            return await q.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Component> GetByIdAsync(long id)
            => await _context.Components.Include(c => c.TypeComponent).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Component> CreateAsync(Component entity)
        {
            _context.Components.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Component entity)
        {
            _context.Components.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SetActiveAsync(long id, bool isActive)
        {
            var e = await _context.Components.FindAsync(id);
            if (e == null) return;
            e.IsActive = isActive;
            _context.Components.Update(e);
            await _context.SaveChangesAsync();
        }
    }
}