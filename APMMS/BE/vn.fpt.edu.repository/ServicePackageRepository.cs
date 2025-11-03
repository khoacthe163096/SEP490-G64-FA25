using BE.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class ServicePackageRepository : IServicePackageRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public ServicePackageRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicePackage>> GetAllAsync()
        {
            return await _context.ServicePackages
                .Include(x => x.Branch)
                .Include(x => x.StatusCodeNavigation)
                .Include(x => x.Components)
                .ToListAsync();
        }

        public async Task<ServicePackage?> GetByIdAsync(long id)
        {
            return await _context.ServicePackages
                .Include(x => x.Branch)
                .Include(x => x.StatusCodeNavigation)
                .Include(x => x.Components)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServicePackage> AddAsync(ServicePackage entity, List<long>? componentIds)
        {
            if (componentIds != null && componentIds.Any())
            {
                var components = await _context.Components
                    .Where(c => componentIds.Contains(c.Id))
                    .ToListAsync();
                entity.Components = components;
            }

            _context.ServicePackages.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ServicePackage> UpdateAsync(ServicePackage entity, List<long>? componentIds)
        {
            var existing = await _context.ServicePackages
                .Include(sp => sp.Components)
                .FirstOrDefaultAsync(sp => sp.Id == entity.Id);

            if (existing == null) throw new Exception("ServicePackage not found.");

            // Update base info
            _context.Entry(existing).CurrentValues.SetValues(entity);

            // Update components relationship
            existing.Components.Clear();

            if (componentIds != null && componentIds.Any())
            {
                var components = await _context.Components
                    .Where(c => componentIds.Contains(c.Id))
                    .ToListAsync();
                foreach (var c in components)
                    existing.Components.Add(c);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.ServicePackages.FindAsync(id);
            if (entity == null)
                return false;

            _context.ServicePackages.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}