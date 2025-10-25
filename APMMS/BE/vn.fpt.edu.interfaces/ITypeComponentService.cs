using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TypeComponent;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ITypeComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync(bool onlyActive = false);
        Task<ResponseDto> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task UpdateAsync(long id, RequestDto dto);
        Task SetActiveAsync(long id, bool isActive);
    }
}