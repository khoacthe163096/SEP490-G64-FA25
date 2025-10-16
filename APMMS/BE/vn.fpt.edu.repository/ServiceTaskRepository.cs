using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class ServiceTaskRepository : IServiceTaskRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public ServiceTaskRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTask> CreateAsync(ServiceTask serviceTask)
        {
            _context.ServiceTasks.Add(serviceTask);
            await _context.SaveChangesAsync();
            return serviceTask;
        }

        public async Task<ServiceTask?> GetByIdAsync(long id)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<ServiceTask?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Car)
                        .ThenInclude(c => c!.User)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Technician)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Branch)
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<List<ServiceTask>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetAllWithDetailsAsync(int page = 1, int pageSize = 10)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Car)
                        .ThenInclude(c => c!.User)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Technician)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Branch)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetByMaintenanceTicketIdAsync(long maintenanceTicketId)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Car)
                        .ThenInclude(c => c!.User)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Technician)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Branch)
                .Where(st => st.MaintenanceTicketId == maintenanceTicketId)
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetByStatusAsync(string statusCode)
        {
            return await _context.ServiceTasks
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Car)
                        .ThenInclude(c => c!.User)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Technician)
                .Include(st => st.MaintenanceTicket)
                    .ThenInclude(mt => mt!.Branch)
                .Where(st => st.StatusCode == statusCode)
                .ToListAsync();
        }

        public async Task<ServiceTask> UpdateAsync(ServiceTask serviceTask)
        {
            _context.ServiceTasks.Update(serviceTask);
            await _context.SaveChangesAsync();
            return serviceTask;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);
            if (serviceTask == null)
                return false;

            _context.ServiceTasks.Remove(serviceTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ServiceTasks.AnyAsync(st => st.Id == id);
        }
    }
}