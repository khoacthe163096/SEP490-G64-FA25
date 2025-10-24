using BE.vn.fpt.edu.models;
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

        public IQueryable<TypeComponent> GetAll()
        {
            return _context.TypeComponents
                .Include(tc => tc.Components)
                .AsQueryable();
        }

        public async Task<IEnumerable<TypeComponent>> GetAllAsync()
        {
            return await _context.TypeComponents
                .Include(tc => tc.Components)
                .ToListAsync();
        }

        public async Task<TypeComponent?> GetByIdAsync(long id)
        {
            return await _context.TypeComponents
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<TypeComponent?> GetByIdWithComponentsAsync(long id)
        {
            return await _context.TypeComponents
                .Include(tc => tc.Components)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task AddAsync(TypeComponent typeComponent)
        {
            await _context.TypeComponents.AddAsync(typeComponent);
        }

        public async Task UpdateAsync(TypeComponent typeComponent)
        {
            _context.TypeComponents.Update(typeComponent);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(TypeComponent typeComponent)
        {
            _context.TypeComponents.Remove(typeComponent);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}