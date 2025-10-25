using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ITypeComponentRepository
    {
        Task<IEnumerable<TypeComponent>> GetAllAsync(bool onlyActive = false);
        Task<TypeComponent> GetByIdAsync(long id);
        Task<TypeComponent> CreateAsync(TypeComponent entity);
        Task UpdateAsync(TypeComponent entity);
        Task SetActiveAsync(long id, bool isActive);
    }
}