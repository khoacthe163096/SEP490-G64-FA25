using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class CarOfAutoOwnerRepository : ICarOfAutoOwnerRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public CarOfAutoOwnerRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Cars
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(long id)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetByUserIdAsync(long userId)
        {
            return await _context.Cars
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Car>> GetServicedCarsByUserIdAsync(long userId)
        {
            // Lấy các xe đã từng có schedule (đã bảo dưỡng) - loại trừ CANCELLED
            var servicedCarIds = await _context.ScheduleServices
                .Where(s => s.UserId == userId && 
                           s.CarId != null &&
                           s.StatusCode != "CANCELLED")
                .Select(s => s.CarId.Value)
                .Distinct()
                .ToListAsync();

            if (servicedCarIds.Count == 0)
                return new List<Car>();

            return await _context.Cars
                .Where(c => servicedCarIds.Contains(c.Id))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<Car> CreateAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Car> UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _context.Cars.FindAsync(id);
            if (existing == null)
                return false;

            _context.Cars.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
