using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class VehicleCheckinRepository : IVehicleCheckinRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public VehicleCheckinRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleCheckin> CreateAsync(VehicleCheckin vehicleCheckin)
        {
            _context.VehicleCheckins.Add(vehicleCheckin);
            await _context.SaveChangesAsync();
            return vehicleCheckin;
        }

        public async Task<VehicleCheckin> UpdateAsync(VehicleCheckin vehicleCheckin)
        {
            _context.VehicleCheckins.Update(vehicleCheckin);
            await _context.SaveChangesAsync();
            return vehicleCheckin;
        }

        public async Task<VehicleCheckin?> GetByIdAsync(long id)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                .Include(vc => vc.MaintenanceRequest)
                .Include(vc => vc.VehicleCheckinImages)
                .FirstOrDefaultAsync(vc => vc.Id == id);
        }

        public async Task<List<VehicleCheckin>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                .Include(vc => vc.MaintenanceRequest)
                .Include(vc => vc.VehicleCheckinImages)
                .OrderByDescending(vc => vc.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<VehicleCheckin>> GetByCarIdAsync(long carId)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                .Include(vc => vc.MaintenanceRequest)
                .Include(vc => vc.VehicleCheckinImages)
                .Where(vc => vc.CarId == carId)
                .OrderByDescending(vc => vc.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VehicleCheckin>> GetByMaintenanceRequestIdAsync(long maintenanceRequestId)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                .Include(vc => vc.MaintenanceRequest)
                .Include(vc => vc.VehicleCheckinImages)
                .Where(vc => vc.MaintenanceRequestId == maintenanceRequestId)
                .OrderByDescending(vc => vc.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var vehicleCheckin = await _context.VehicleCheckins.FindAsync(id);
            if (vehicleCheckin == null)
                return false;

            _context.VehicleCheckins.Remove(vehicleCheckin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VehicleCheckin?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.User)
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.VehicleType)
                .Include(vc => vc.MaintenanceRequest)
                    .ThenInclude(mr => mr.User)
                .Include(vc => vc.VehicleCheckinImages)
                .FirstOrDefaultAsync(vc => vc.Id == id);
        }

        public async Task<List<VehicleCheckin>> GetAllWithDetailsAsync(int page = 1, int pageSize = 10)
        {
            return await _context.VehicleCheckins
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.User)
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.VehicleType)
                .Include(vc => vc.MaintenanceRequest)
                    .ThenInclude(mr => mr.User)
                .Include(vc => vc.VehicleCheckinImages)
                .OrderByDescending(vc => vc.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Car>> SearchCarsAsync(string searchTerm)
        {
            return await _context.Cars
                .Include(c => c.User)
                .Include(c => c.Branch)
                .Include(c => c.VehicleType)
                .Where(c => c.LicensePlate.Contains(searchTerm) || 
                           (c.VinNumber != null && c.VinNumber.Contains(searchTerm)))
                .ToListAsync();
        }
    }
}
