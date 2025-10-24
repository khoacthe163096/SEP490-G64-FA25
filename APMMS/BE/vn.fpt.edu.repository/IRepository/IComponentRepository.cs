using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IComponentRepository
    {
        IQueryable<Component> GetAll();
        Task<IEnumerable<Component>> GetAllAsync();
        Task<IEnumerable<Component>> GetAllWithDetailsAsync(int page = 1, int pageSize = 10);
        Task<Component?> GetByIdAsync(long id);
        Task<Component?> GetByIdWithDetailsAsync(long id);
        Task<IEnumerable<Component>> GetByTypeComponentIdAsync(long typeComponentId, int page = 1, int pageSize = 10);
        Task<IEnumerable<Component>> GetByBranchIdAsync(long branchId, int page = 1, int pageSize = 10);
        Task<int> GetTotalCountAsync();
        Task<int> GetCountByTypeComponentIdAsync(long typeComponentId);
        Task<int> GetCountByBranchIdAsync(long branchId);
        Task AddAsync(Component component);
        Task UpdateAsync(Component component);
        Task DeleteAsync(Component component);
        Task SaveChangesAsync();
    }
}