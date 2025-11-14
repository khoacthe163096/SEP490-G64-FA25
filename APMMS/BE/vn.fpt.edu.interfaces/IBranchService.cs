using BE.vn.fpt.edu.DTOs.Branch;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDto>> GetAllAsync();
        Task<BranchResponseDto?> GetByIdAsync(long id);
        Task<BranchResponseDto> CreateAsync(BranchRequestDto dto);
        Task<BranchResponseDto?> UpdateAsync(long id, BranchRequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}

