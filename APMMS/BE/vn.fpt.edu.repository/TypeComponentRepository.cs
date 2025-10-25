using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
namespace BE.vn.fpt.edu.repository
{
    public class TypeComponentRepository : ITypeComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;
        public TypeComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TypeComponent>> GetAllAsync(bool onlyActive = false)
        {
            IQueryable<TypeComponent> q = _context.TypeComponents.AsNoTracking();
            if (onlyActive) q = q.Where(x => x.IsActive);
            return await q.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<TypeComponent> GetByIdAsync(long id)
            => await _context.TypeComponents.FindAsync(id);

        public async Task<TypeComponent> CreateAsync(TypeComponent entity)
        {
            _context.TypeComponents.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TypeComponent entity)
        {
            _context.TypeComponents.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SetActiveAsync(long id, bool isActive)
        {
            var e = await _context.TypeComponents.FindAsync(id);
            if (e == null) return;
            e.IsActive = isActive;
            _context.TypeComponents.Update(e);
            await _context.SaveChangesAsync();
        }
    }
}