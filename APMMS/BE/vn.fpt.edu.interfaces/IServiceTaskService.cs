using BE.vn.fpt.edu.DTOs.ServiceTask;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IServiceTaskService
    {
        Task<ServiceTaskResponseDto> CreateServiceTaskAsync(ServiceTaskRequestDto request);
        Task<ServiceTaskResponseDto> UpdateServiceTaskAsync(ServiceTaskUpdateDto request);
        Task<ServiceTaskResponseDto> GetServiceTaskByIdAsync(long id);
        Task<List<ServiceTaskListResponseDto>> GetAllServiceTasksAsync(int page = 1, int pageSize = 10);
        Task<List<ServiceTaskListResponseDto>> GetServiceTasksByMaintenanceTicketIdAsync(long maintenanceTicketId);
        Task<List<ServiceTaskListResponseDto>> GetServiceTasksByStatusAsync(string statusCode);
        Task<List<ServiceTaskListResponseDto>> GetServiceTasksByTechnicianIdAsync(long technicianId);
        Task<bool> DeleteServiceTaskAsync(long id);
        Task<ServiceTaskResponseDto> UpdateStatusAsync(long id, string statusCode);
        Task<ServiceTaskResponseDto> UpdateLaborTimeAsync(long id, decimal actualLaborTime);
    }
}


