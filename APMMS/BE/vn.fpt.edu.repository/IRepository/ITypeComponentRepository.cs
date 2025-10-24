using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ITypeComponentRepository
    {
        IQueryable<TypeComponent> GetAll();
        Task<IEnumerable<TypeComponent>> GetAllAsync();
        Task<TypeComponent?> GetByIdAsync(long id);
        Task<TypeComponent?> GetByIdWithComponentsAsync(long id);
        Task AddAsync(TypeComponent typeComponent);
        Task UpdateAsync(TypeComponent typeComponent);
        Task DeleteAsync(TypeComponent typeComponent);
        Task SaveChangesAsync();
    }
}