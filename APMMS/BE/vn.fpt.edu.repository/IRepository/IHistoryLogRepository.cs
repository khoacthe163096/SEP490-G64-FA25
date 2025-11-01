using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IHistoryLogRepository
    {
        Task<HistoryLog> CreateAsync(HistoryLog historyLog);
    }
}


