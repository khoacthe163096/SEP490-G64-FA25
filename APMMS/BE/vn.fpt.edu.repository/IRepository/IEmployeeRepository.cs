using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IEmployeeRepository
    {
        IQueryable<User> GetAll();
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(long id);
        Task AddAsync(User employee);
        Task UpdateAsync(User employee);
        Task SoftDeleteAsync(User employee);
        Task SaveChangesAsync();
        Task<List<User>> GetWithFiltersAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, long? roleId = null);
        Task<int> GetTotalCountAsync(string? search = null, string? status = null, long? roleId = null);
    }
}
