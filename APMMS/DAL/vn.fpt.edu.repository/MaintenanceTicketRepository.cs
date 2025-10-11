using DAL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace DAL.vn.fpt.edu.repository
{
    public class MaintenanceTicketRepository : IMaintenanceTicketRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public MaintenanceTicketRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<MaintenanceTicket> CreateAsync(MaintenanceTicket maintenanceTicket)
        {
            _context.MaintenanceTickets.Add(maintenanceTicket);
            await _context.SaveChangesAsync();
            return maintenanceTicket;
        }

        public async Task<MaintenanceTicket?> GetByIdAsync(long id)
        {
            return await _context.MaintenanceTickets
                .Include(mt => mt.Car)
                .Include(mt => mt.Consulter)
                .Include(mt => mt.Technician)
                .Include(mt => mt.Branch)
                .Include(mt => mt.ScheduleService)
                .FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<List<MaintenanceTicket>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.MaintenanceTickets
                .Include(mt => mt.Car)
                .Include(mt => mt.Consulter)
                .Include(mt => mt.Technician)
                .Include(mt => mt.Branch)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<MaintenanceTicket>> GetByCarIdAsync(long carId)
        {
            return await _context.MaintenanceTickets
                .Include(mt => mt.Car)
                .Include(mt => mt.Consulter)
                .Include(mt => mt.Technician)
                .Include(mt => mt.Branch)
                .Where(mt => mt.CarId == carId)
                .ToListAsync();
        }

        public async Task<List<MaintenanceTicket>> GetByStatusAsync(string statusCode)
        {
            return await _context.MaintenanceTickets
                .Include(mt => mt.Car)
                .Include(mt => mt.Consulter)
                .Include(mt => mt.Technician)
                .Include(mt => mt.Branch)
                .Where(mt => mt.StatusCode == statusCode)
                .ToListAsync();
        }

        public async Task<MaintenanceTicket> UpdateAsync(MaintenanceTicket maintenanceTicket)
        {
            _context.MaintenanceTickets.Update(maintenanceTicket);
            await _context.SaveChangesAsync();
            return maintenanceTicket;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var maintenanceTicket = await _context.MaintenanceTickets.FindAsync(id);
            if (maintenanceTicket == null)
                return false;

            _context.MaintenanceTickets.Remove(maintenanceTicket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.MaintenanceTickets.AnyAsync(mt => mt.Id == id);
        }
    }
}


