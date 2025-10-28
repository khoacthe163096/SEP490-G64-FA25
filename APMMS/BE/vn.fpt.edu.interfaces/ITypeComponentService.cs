using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TypeComponent;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ITypeComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync(long? branchId = null, string? statusCode = null);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(RequestDto dto);
        Task DisableEnableAsync(long id, string statusCode);
    }
}