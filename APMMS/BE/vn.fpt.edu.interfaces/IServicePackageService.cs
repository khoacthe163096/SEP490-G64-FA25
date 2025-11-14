using BE.vn.fpt.edu.DTOs.ServicePackage;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IServicePackageService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync();
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<bool> UpdateAsync(long id, RequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
