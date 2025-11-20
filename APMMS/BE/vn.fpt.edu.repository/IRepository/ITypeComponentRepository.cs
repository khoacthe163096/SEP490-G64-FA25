using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ITypeComponentRepository
    {
        Task<IEnumerable<TypeComponent>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null);
        Task<TypeComponent?> GetByIdAsync(long id);
        Task<TypeComponent> AddAsync(TypeComponent entity);
        Task<TypeComponent> UpdateAsync(TypeComponent entity);
        Task DisableEnableAsync(long id, string statusCode);
        Task<bool> ExistsAsync(long id);
    }
}