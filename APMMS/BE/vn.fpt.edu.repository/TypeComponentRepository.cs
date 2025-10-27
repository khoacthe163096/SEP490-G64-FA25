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

        public async Task<IEnumerable<TypeComponent>> GetAllAsync()
        {
            return await _context.TypeComponents
                .Include(tc => tc.Branch)
                .Include(tc => tc.StatusCodeNavigation)
                .OrderBy(tc => tc.Name)
                .ToListAsync();
        }

        public async Task<TypeComponent?> GetByIdAsync(long id)
        {
            return await _context.TypeComponents
                .Include(tc => tc.Branch)
                .Include(tc => tc.StatusCodeNavigation)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task AddAsync(TypeComponent entity)
        {
            await _context.TypeComponents.AddAsync(entity);
        }

        public void Update(TypeComponent entity)
        {
            _context.TypeComponents.Update(entity);
        }

        public void Delete(TypeComponent entity)
        {
            _context.TypeComponents.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}