using BE.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Component>> GetAllAsync();
        Task<Component?> GetByIdAsync(long id);
        Task<Component> AddAsync(Component entity);
        Task<Component> UpdateAsync(Component entity);
        Task<bool> DeleteAsync(long id);
    }
}
