using BE.vn.fpt.edu.DTOs.CarOfAutoOwner;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ICarOfAutoOwnerService
    {
        Task<List<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<List<ResponseDto>> GetByUserIdAsync(long userId);
        Task<List<ResponseDto>> GetServicedCarsByUserIdAsync(long userId);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto> UpdateAsync(long id, RequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
