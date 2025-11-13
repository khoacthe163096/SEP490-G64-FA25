using System;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TotalReceipt;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ITotalReceiptService
    {
        Task<PagedResultDto<ResponseDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? statusCode = null, DateTime? fromDate = null, DateTime? toDate = null, long? branchId = null);
        Task<ResponseDto?> GetByIdAsync(long id);
        Task<ResponseDto?> GetByMaintenanceTicketIdAsync(long maintenanceTicketId);
        Task<ResponseDto> CreateAsync(RequestDto dto);
        Task<ResponseDto?> UpdateAsync(long id, RequestDto dto, long? currentUserId = null);
        Task<bool> DeleteAsync(long id);
    }
}


