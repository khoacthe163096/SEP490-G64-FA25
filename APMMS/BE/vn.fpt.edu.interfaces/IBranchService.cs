using BE.vn.fpt.edu.DTOs.Branch;
using BE.vn.fpt.edu.DTOs.Employee;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDto>> GetAllAsync();
        Task<BranchResponseDto?> GetByIdAsync(long id);
        Task<BranchResponseDto> CreateAsync(BranchRequestDto dto);
        Task<BranchResponseDto?> UpdateAsync(long id, BranchRequestDto dto);
        Task<bool> DeleteAsync(long id);
        Task<EmployeeResponseDto?> GetDirectorAsync(long branchId);
        Task<bool> ChangeDirectorAsync(long branchId, long newDirectorId);
    }
}

