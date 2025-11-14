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
                .ToListAsync();
        }

        public async Task<ServicePackage?> GetByIdAsync(long id)
        {
            return await _context.ServicePackages
                .Include(x => x.Branch)
                .Include(x => x.StatusCodeNavigation)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ServicePackage entity)
        {
            await _context.ServicePackages.AddAsync(entity);
        }

        public async Task UpdateAsync(ServicePackage entity)
        {
            _context.ServicePackages.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ServicePackage entity)
        {
            _context.ServicePackages.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
