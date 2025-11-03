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

        public async Task<Component> AddAsync(Component entity)
        {
            var added = await _context.Components.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            var item = await _context.Components.FindAsync(id);
            if (item == null) return;
            item.StatusCode = statusCode;
            _context.Components.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Components.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Component>> GetAllAsync(long? branchId = null, long? typeComponentId = null, string? statusCode = null, string? search = null)
        {
            var q = _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .AsQueryable();

            if (branchId.HasValue) q = q.Where(x => x.BranchId == branchId.Value);
            if (typeComponentId.HasValue) q = q.Where(x => x.TypeComponentId == typeComponentId.Value);
            if (!string.IsNullOrEmpty(statusCode)) q = q.Where(x => x.StatusCode == statusCode);
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x => (x.Name != null && x.Name.ToLower().Contains(s)) || (x.Code != null && x.Code.ToLower().Contains(s)));
            }

            return await q.ToListAsync();
        }

        public async Task<Component?> GetByIdAsync(long id)
        {
            return await _context.Components
                .Include(c => c.TypeComponent)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Component> UpdateAsync(Component entity)
        {
            _context.Components.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}