using BE.vn.fpt.edu.DTOs.ServicePackage;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IServicePackageService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(RequestDto dto);
        Task DisableEnableAsync(long id, string statusCode);
    }
}


