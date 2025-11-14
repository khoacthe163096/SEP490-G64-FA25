using BE.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IServicePackageRepository
    {
        Task<IEnumerable<ServicePackage>> GetAllAsync();
        Task<ServicePackage?> GetByIdAsync(long id);
        Task AddAsync(ServicePackage entity);
        Task UpdateAsync(ServicePackage entity);
        Task DeleteAsync(ServicePackage entity);
        Task SaveChangesAsync();
    }
}
