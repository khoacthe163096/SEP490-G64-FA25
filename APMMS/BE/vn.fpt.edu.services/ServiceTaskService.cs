using AutoMapper;
using BE.vn.fpt.edu.DTOs.ServiceTask;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.services
{
    public class ServiceTaskService : IServiceTaskService
    {
        private readonly IServiceTaskRepository _serviceTaskRepository;
        private readonly IMapper _mapper;

        public ServiceTaskService(IServiceTaskRepository serviceTaskRepository, IMapper mapper)
        {
            _serviceTaskRepository = serviceTaskRepository;
            _mapper = mapper;
        }

        public async Task<ServiceTaskResponseDto> CreateServiceTaskAsync(ServiceTaskRequestDto request)
        {
            var serviceTask = _mapper.Map<ServiceTask>(request);
            var createdTask = await _serviceTaskRepository.CreateAsync(serviceTask);
            return await GetServiceTaskByIdAsync(createdTask.Id);
        }

        public async Task<ServiceTaskResponseDto> UpdateServiceTaskAsync(ServiceTaskUpdateDto request)
        {
            var existingTask = await _serviceTaskRepository.GetByIdAsync(request.Id);
            if (existingTask == null)
                throw new ArgumentException("Service task not found");

            _mapper.Map(request, existingTask);
            var updatedTask = await _serviceTaskRepository.UpdateAsync(existingTask);
            return await GetServiceTaskByIdAsync(updatedTask.Id);
        }

        public async Task<ServiceTaskResponseDto> GetServiceTaskByIdAsync(long id)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdWithDetailsAsync(id);
            if (serviceTask == null)
                throw new ArgumentException("Service task not found");

            return MapToResponseDTO(serviceTask);
        }

        public async Task<List<ServiceTaskListResponseDto>> GetAllServiceTasksAsync(int page = 1, int pageSize = 10)
        {
            var serviceTasks = await _serviceTaskRepository.GetAllWithDetailsAsync(page, pageSize);
            return serviceTasks.Select(MapToListResponseDTO).ToList();
        }

        public async Task<List<ServiceTaskListResponseDto>> GetServiceTasksByMaintenanceTicketIdAsync(long maintenanceTicketId)
        {
            var serviceTasks = await _serviceTaskRepository.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
            return serviceTasks.Select(MapToListResponseDTO).ToList();
        }

        public async Task<List<ServiceTaskListResponseDto>> GetServiceTasksByStatusAsync(string statusCode)
        {
            var serviceTasks = await _serviceTaskRepository.GetByStatusAsync(statusCode);
            return serviceTasks.Select(MapToListResponseDTO).ToList();
        }

        public async Task<List<ServiceTaskListResponseDto>> GetServiceTasksByTechnicianIdAsync(long technicianId)
        {
            var serviceTasks = await _serviceTaskRepository.GetByTechnicianIdAsync(technicianId);
            return serviceTasks.Select(MapToListResponseDTO).ToList();
        }

        public async Task<bool> DeleteServiceTaskAsync(long id)
        {
            return await _serviceTaskRepository.DeleteAsync(id);
        }

        public async Task<ServiceTaskResponseDto> UpdateStatusAsync(long id, string statusCode)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);
            if (serviceTask == null)
                throw new ArgumentException("Service task not found");

            serviceTask.StatusCode = statusCode;
            var updatedTask = await _serviceTaskRepository.UpdateAsync(serviceTask);
            return await GetServiceTaskByIdAsync(updatedTask.Id);
        }

        private ServiceTaskResponseDto MapToResponseDTO(ServiceTask serviceTask)
        {
            return new ServiceTaskResponseDto
            {
                Id = serviceTask.Id,
                MaintenanceTicketId = serviceTask.MaintenanceTicketId ?? 0,
                TaskName = serviceTask.TaskName ?? string.Empty,
                Description = serviceTask.Description,
                StatusCode = serviceTask.StatusCode,
                Note = serviceTask.Note,

                // Navigation properties
                MaintenanceTicketDescription = serviceTask.MaintenanceTicket?.Description,
                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,
                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 
                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()
                    : null,
                TechnicianName = serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                    : null,
                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name
            };
        }

        private ServiceTaskListResponseDto MapToListResponseDTO(ServiceTask serviceTask)
        {
            return new ServiceTaskListResponseDto
            {
                Id = serviceTask.Id,
                MaintenanceTicketId = serviceTask.MaintenanceTicketId ?? 0,
                TaskName = serviceTask.TaskName ?? string.Empty,
                Description = serviceTask.Description,
                StatusCode = serviceTask.StatusCode,
                Note = serviceTask.Note,

                // Basic info
                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,
                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 
                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()
                    : null,
                TechnicianName = serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                    : null,
                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name
            };
        }
    }
}


