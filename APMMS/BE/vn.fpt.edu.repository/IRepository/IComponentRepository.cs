using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Component>> GetAllAsync(bool onlyActive = false, long? typeComponentId = null, long? branchId = null);
        Task<Component> GetByIdAsync(long id);
        Task<Component> CreateAsync(Component entity);
        Task UpdateAsync(Component entity);
        Task SetActiveAsync(long id, bool isActive);
    }
}