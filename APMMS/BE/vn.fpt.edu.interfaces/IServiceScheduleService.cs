using BE.vn.fpt.edu.DTOs.ServiceSchedule;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IServiceScheduleService
    {
        Task<ResponseDto> CreateScheduleAsync(RequestDto request);
        Task<ResponseDto> GetScheduleByIdAsync(long id);
        Task<List<ListResponseDto>> GetAllSchedulesAsync(int page = 1, int pageSize = 10);
        Task<List<ListResponseDto>> GetSchedulesByUserIdAsync(long userId);
        Task<List<ListResponseDto>> GetSchedulesByBranchIdAsync(long branchId);
        Task<List<ListResponseDto>> GetSchedulesByStatusAsync(string statusCode, long? branchId = null);
        Task<List<ListResponseDto>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, long? branchId = null);
        Task<ResponseDto> UpdateScheduleAsync(long id, UpdateScheduleDto request);
        Task<ResponseDto> CancelScheduleAsync(long id, CancelScheduleDto? request = null);
        Task<ResponseDto> CompleteScheduleAsync(long id);
        Task<bool> DeleteScheduleAsync(long id);
        Task<ResponseDto> CreatePublicBookingAsync(PublicBookingDto request);
        Task<ResponseDto> AcceptScheduleAsync(long id, AcceptScheduleDto request);
        Task<NoteResponseDto> AddNoteAsync(long scheduleId, AddNoteDto request);
        Task<List<NoteResponseDto>> GetNotesAsync(long scheduleId);
    }
}
