using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IComponentService
    {
        Task<IEnumerable<ResponseDto>> GetAllAsync(bool onlyActive = false, long? typeComponentId = null, long? branchId = null);
        Task<ResponseDto> GetByIdAsync(long id);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task UpdateAsync(long id, RequestDto dto);
        Task SetActiveAsync(long id, bool isActive);
    }
}