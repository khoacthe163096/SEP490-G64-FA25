using BE.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ITypeComponentRepository
    {
        Task<IEnumerable<TypeComponent>> GetAllAsync();
        Task<TypeComponent?> GetByIdAsync(long id);
        Task<TypeComponent> AddAsync(TypeComponent entity);
        Task<TypeComponent> UpdateAsync(TypeComponent entity);
        Task<bool> DeleteAsync(long id);
    }
}