using BE.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IServicePackageRepository
    {
        Task<IEnumerable<ServicePackage>> GetAllAsync();
        Task<ServicePackage?> GetByIdAsync(long id);
        Task<ServicePackage> AddAsync(ServicePackage entity, List<long>? componentIds);
        Task<ServicePackage> UpdateAsync(ServicePackage entity, List<long>? componentIds);
        Task<bool> DeleteAsync(long id);
    }
}