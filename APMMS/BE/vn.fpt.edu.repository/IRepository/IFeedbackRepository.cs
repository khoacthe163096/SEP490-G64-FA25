using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(long id);
        Task<IEnumerable<Feedback>> GetByUserIdAsync(long userId);
        Task<IEnumerable<Feedback>> GetByTicketIdAsync(long ticketId);
        Task<Feedback> CreateAsync(Feedback feedback);
        Task<Feedback?> UpdateAsync(Feedback feedback);
        Task<bool> DeleteAsync(long id);
    }
}
