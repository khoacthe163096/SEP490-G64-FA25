using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync();
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(long id, RequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
