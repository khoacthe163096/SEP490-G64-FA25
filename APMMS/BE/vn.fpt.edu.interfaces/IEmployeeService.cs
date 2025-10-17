using BE.vn.fpt.edu.DTOs.Employee;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();
        Task<EmployeeResponseDto?> GetByIdAsync(long id);
        Task<EmployeeResponseDto> CreateAsync(EmployeeRequestDto dto);
        Task<EmployeeResponseDto?> UpdateAsync(long id, EmployeeRequestDto dto);
        Task<bool> DeleteAsync(long id);
    }
}



