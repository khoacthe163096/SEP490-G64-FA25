using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IMaintenanceTicketRepository
    {
        Task<MaintenanceTicket> CreateAsync(MaintenanceTicket maintenanceTicket);
        Task<MaintenanceTicket?> GetByIdAsync(long id);
        Task<MaintenanceTicket?> GetByIdWithBranchAsync(long id);
        Task<MaintenanceTicket?> GetByIdWithCostDetailsAsync(long id);
        Task<List<MaintenanceTicket>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<List<MaintenanceTicket>> GetByCarIdAsync(long carId);
        Task<List<MaintenanceTicket>> GetByStatusAsync(string statusCode);
        Task<MaintenanceTicket> UpdateAsync(MaintenanceTicket maintenanceTicket);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<bool> CodeExistsAsync(string code);
    }
}


