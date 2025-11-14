using BE.vn.fpt.edu.models;
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

        public async Task<ServicePackage> AddAsync(ServicePackage entity)
        {
            var added = await _context.ServicePackages.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            var item = await _context.ServicePackages.FindAsync(id);
            if (item == null) return;
            item.StatusCode = statusCode;
            _context.ServicePackages.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ServicePackages.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ServicePackage>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null)
        {
            var q = _context.ServicePackages
                .Include(sp => sp.Components)
                .Include(sp => sp.ServicePackageCategories)
                    .ThenInclude(spc => spc.ServiceCategory)
                .AsQueryable();

            if (branchId.HasValue) q = q.Where(x => x.BranchId == branchId.Value);
            if (!string.IsNullOrEmpty(statusCode)) q = q.Where(x => x.StatusCode == statusCode);
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x => (x.Name != null && x.Name.ToLower().Contains(s)) || (x.Code != null && x.Code.ToLower().Contains(s)));
            }

            return await q.ToListAsync();
        }

        public async Task<ServicePackage?> GetByIdAsync(long id)
        {
            return await _context.ServicePackages
                .Include(sp => sp.Components)
                .Include(sp => sp.ServicePackageCategories)
                    .ThenInclude(spc => spc.ServiceCategory)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<ServicePackage> UpdateAsync(ServicePackage entity)
        {
            _context.ServicePackages.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}


