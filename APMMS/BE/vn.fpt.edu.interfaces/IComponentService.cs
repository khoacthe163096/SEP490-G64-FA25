using BE.vn.fpt.edu.DTOs.Component;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IComponentService
    {
        Task<(IEnumerable<ComponentResponseDto> components, PaginationDto pagination)> GetAllComponentsAsync(int page = 1, int pageSize = 10, long? typeComponentId = null, long? branchId = null);
        Task<ComponentResponseDto?> GetComponentByIdAsync(long id);
        Task<(IEnumerable<ComponentResponseDto> components, PaginationDto pagination)> GetComponentsByTypeComponentIdAsync(long typeComponentId, int page = 1, int pageSize = 10);
        Task<ComponentResponseDto> CreateComponentAsync(CreateComponentDto request);
        Task<ComponentResponseDto> UpdateComponentAsync(long id, UpdateComponentDto request);
        Task<bool> DeleteComponentAsync(long id);
        Task<ComponentResponseDto> UpdateStockAsync(long id, UpdateStockDto request);
    }
}