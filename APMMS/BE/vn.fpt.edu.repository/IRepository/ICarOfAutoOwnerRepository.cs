using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface ICarOfAutoOwnerRepository
    {
        Task<List<Car>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Car?> GetByIdAsync(long id);
        Task<List<Car>> GetByUserIdAsync(long userId);
        Task<Car> CreateAsync(Car car);
        Task<Car> UpdateAsync(Car car);
        Task<bool> DeleteAsync(long id);
    }
}
