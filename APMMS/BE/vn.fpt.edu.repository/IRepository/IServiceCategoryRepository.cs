using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IServiceCategoryRepository
    {
        Task<ServiceCategory?> GetByIdAsync(long id);
        Task<List<ServiceCategory>> GetAllAsync();
    }
}

