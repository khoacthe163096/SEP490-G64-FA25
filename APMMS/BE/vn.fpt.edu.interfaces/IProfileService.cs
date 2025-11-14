using BE.vn.fpt.edu.DTOs.Employee;
using Microsoft.AspNetCore.Http;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IProfileService
    {
        Task<EmployeeResponseDto?> GetMyProfileAsync(long userId);
        Task<EmployeeResponseDto> UpdateMyProfileAsync(long userId, EmployeeProfileUpdateDto dto);
        Task<string> UploadAvatarAsync(long userId, IFormFile file);
    }
}


