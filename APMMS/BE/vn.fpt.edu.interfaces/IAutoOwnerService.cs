using BE.vn.fpt.edu.DTOs.AutoOwner;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IAutoOwnerService
    {
        Task<List<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto> UpdateAsync(long id, RequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
