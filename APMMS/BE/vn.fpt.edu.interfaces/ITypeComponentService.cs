using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TypeComponent;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ITypeComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync();
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(long id, RequestDto dto);
        Task<bool> DeleteAsync(long id);
        Task<bool> ToggleStatusAsync(long id);
    }
}