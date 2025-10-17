using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IVehicleCheckinRepository
    {
        Task<VehicleCheckin> CreateAsync(VehicleCheckin vehicleCheckin);
        Task<VehicleCheckin> UpdateAsync(VehicleCheckin vehicleCheckin);
        Task<VehicleCheckin?> GetByIdAsync(long id);
        Task<List<VehicleCheckin>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<List<VehicleCheckin>> GetByCarIdAsync(long carId);
        Task<List<VehicleCheckin>> GetByMaintenanceRequestIdAsync(long maintenanceRequestId);
        Task<bool> DeleteAsync(long id);
        Task<VehicleCheckin?> GetByIdWithDetailsAsync(long id);
        Task<List<VehicleCheckin>> GetAllWithDetailsAsync(int page = 1, int pageSize = 10);
        Task<List<Car>> SearchCarsAsync(string searchTerm);
    }
}
