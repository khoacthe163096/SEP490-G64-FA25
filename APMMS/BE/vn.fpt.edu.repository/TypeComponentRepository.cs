using BE.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class TypeComponentRepository : ITypeComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public TypeComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TypeComponent>> GetAllAsync()
        {
            return await _context.TypeComponents
                .Include(x => x.Branch)
                .Include(x => x.StatusCodeNavigation)
                .ToListAsync();
        }

        public async Task<TypeComponent?> GetByIdAsync(long id)
        {
            return await _context.TypeComponents
                .Include(x => x.Branch)
                .Include(x => x.StatusCodeNavigation)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TypeComponent> AddAsync(TypeComponent entity)
        {
            _context.TypeComponents.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TypeComponent> UpdateAsync(TypeComponent entity)
        {
            _context.TypeComponents.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _context.TypeComponents.FindAsync(id);
            if (existing == null)
                return false;

            _context.TypeComponents.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}