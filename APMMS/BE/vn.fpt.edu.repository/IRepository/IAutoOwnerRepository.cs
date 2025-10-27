using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IAutoOwnerRepository
    {
        Task<List<User>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByCarIdAsync(long carId);
        Task<List<User>> GetByBranchIdAsync(long branchId);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(long id);
    }
}
