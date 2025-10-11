using DAL.vn.fpt.edu.models;

namespace DAL.vn.fpt.edu.interfaces
{
    public interface IMaintenanceTicketRepository
    {
        Task<MaintenanceTicket> CreateAsync(MaintenanceTicket maintenanceTicket);
        Task<MaintenanceTicket?> GetByIdAsync(long id);
        Task<List<MaintenanceTicket>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<List<MaintenanceTicket>> GetByCarIdAsync(long carId);
        Task<List<MaintenanceTicket>> GetByStatusAsync(string statusCode);
        Task<MaintenanceTicket> UpdateAsync(MaintenanceTicket maintenanceTicket);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}


