using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IServicePackageRepository
    {
        Task<IEnumerable<ServicePackage>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null);
        Task<ServicePackage?> GetByIdAsync(long id);
        Task<ServicePackage> AddAsync(ServicePackage entity);
        Task<ServicePackage> UpdateAsync(ServicePackage entity);
        Task DisableEnableAsync(long id, string statusCode);
        Task<bool> ExistsAsync(long id);
    }
}


