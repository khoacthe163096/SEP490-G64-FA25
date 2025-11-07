using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class ServiceScheduleRepository : IServiceScheduleRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public ServiceScheduleRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleService> CreateAsync(ScheduleService scheduleService)
        {
            _context.ScheduleServices.Add(scheduleService);
            await _context.SaveChangesAsync();
            return scheduleService;
        }

        public async Task<ScheduleService?> GetByIdAsync(long id)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<ScheduleService>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .OrderByDescending(s => s.ScheduledDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ScheduleService>> GetByUserIdAsync(long userId)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();
        }

        public async Task<List<ScheduleService>> GetByBranchIdAsync(long branchId)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .Where(s => s.BranchId == branchId)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();
        }

        public async Task<List<ScheduleService>> GetByStatusAsync(string statusCode)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .Where(s => s.StatusCode == statusCode)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();
        }

        public async Task<List<ScheduleService>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleServices
                .Include(s => s.User)
                .Include(s => s.Car)
                    .ThenInclude(c => c.User)
                .Include(s => s.Branch)
                .Include(s => s.StatusCodeNavigation)
                .Where(s => s.ScheduledDate >= startDate && s.ScheduledDate <= endDate)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();
        }

        public async Task<ScheduleService> UpdateAsync(ScheduleService scheduleService)
        {
            _context.ScheduleServices.Update(scheduleService);
            await _context.SaveChangesAsync();
            return scheduleService;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var schedule = await _context.ScheduleServices.FindAsync(id);
            if (schedule == null)
                return false;

            _context.ScheduleServices.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ScheduleServices.AnyAsync(s => s.Id == id);
        }
    }
}
