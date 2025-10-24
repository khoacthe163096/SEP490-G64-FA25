using BE.vn.fpt.edu.DTOs.TypeComponent;

namespace BE.vn.fpt.edu.interfaces
{
    public interface ITypeComponentService
    {
        Task<IEnumerable<TypeComponentResponseDto>> GetAllTypeComponentsAsync();
        Task<TypeComponentDetailResponseDto?> GetTypeComponentByIdAsync(long id);
        Task<TypeComponentResponseDto> CreateTypeComponentAsync(CreateTypeComponentDto request);
        Task<TypeComponentResponseDto> UpdateTypeComponentAsync(long id, UpdateTypeComponentDto request);
        Task<bool> DeleteTypeComponentAsync(long id);
    }
}