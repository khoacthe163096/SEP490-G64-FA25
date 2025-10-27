using BE.vn.fpt.edu.DTOs.Feedback;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync();
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<IEnumerable<ResponseDto>> GetByUserIdAsync(long userId);
        Task<IEnumerable<ResponseDto>> GetByTicketIdAsync(long ticketId);
        Task<ResponseDto> CreateAsync(RequestDto request);
        Task<ResponseDto?> UpdateAsync(long id, RequestDto request);
        Task<bool> DeleteAsync(long id);
    }
}
