using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ITypeComponentRepository
    {
        Task<IEnumerable<TypeComponent>> GetAllAsync();
        Task<TypeComponent?> GetByIdAsync(long id);
        Task AddAsync(TypeComponent entity);
        void Update(TypeComponent entity);
        void Delete(TypeComponent entity);
        Task SaveChangesAsync();
    }
}