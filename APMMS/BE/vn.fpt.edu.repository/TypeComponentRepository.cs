using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository
{
    public interface ITypeComponentRepository
    {
        Task<IEnumerable<TypeComponent>> GetAllAsync(bool onlyActive = false);
        Task<TypeComponent> GetByIdAsync(long id);
        Task<TypeComponent> CreateAsync(TypeComponent entity);
        Task UpdateAsync(TypeComponent entity);
        Task SetActiveAsync(long id, bool isActive);
    }

    public class TypeComponentRepository : ITypeComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;
        public TypeComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TypeComponent>> GetAllAsync(bool onlyActive = false)
        {
            IQueryable<TypeComponent> query = _context.TypeComponents.AsNoTracking();
            if (onlyActive)
                query = query.Where(x => x.IsActive);
            return await query.OrderBy(x => x.Name).ToListAsync();
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
            var entity = await _context.TypeComponents.FindAsync(id);
            if (entity == null) return;
            entity.IsActive = isActive;
            _context.TypeComponents.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}