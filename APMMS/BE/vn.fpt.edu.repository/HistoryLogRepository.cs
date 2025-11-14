using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.repository
{
    public class HistoryLogRepository : IHistoryLogRepository
    {
        private readonly CarMaintenanceDbContext _context;

        public HistoryLogRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<HistoryLog> CreateAsync(HistoryLog historyLog)
        {
            await _context.HistoryLogs.AddAsync(historyLog);
            await _context.SaveChangesAsync();
            return historyLog;
        }

        public async Task<List<HistoryLog>> GetByMaintenanceTicketIdAsync(long maintenanceTicketId)
        {
            return await _context.HistoryLogs
                .Include(h => h.User)
                .Where(h => h.MaintenanceTicketId == maintenanceTicketId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}


