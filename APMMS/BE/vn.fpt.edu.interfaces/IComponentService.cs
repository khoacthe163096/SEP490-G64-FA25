using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync(long? branchId = null, long? typeComponentId = null, string? statusCode = null, string? search = null);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(RequestDto dto);
        Task DisableEnableAsync(long id, string statusCode);
    }
}