using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Component>> GetAllAsync(long? branchId = null, long? typeComponentId = null, string? statusCode = null, string? search = null);
        Task<Component?> GetByIdAsync(long id);
        Task<Component> AddAsync(Component entity);
        Task<Component> UpdateAsync(Component entity);
        Task DisableEnableAsync(long id, string statusCode);
        Task<bool> ExistsAsync(long id);
    }
}