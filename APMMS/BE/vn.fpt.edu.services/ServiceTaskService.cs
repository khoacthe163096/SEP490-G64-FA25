using AutoMapper;
using BE.vn.fpt.edu.DTOs.ServiceTask;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class ServiceTaskService : IServiceTaskService
    {
        private readonly IServiceTaskRepository _serviceTaskRepository;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public ServiceTaskService(IServiceTaskRepository serviceTaskRepository, IMapper mapper, CarMaintenanceDbContext context)
        {
            _serviceTaskRepository = serviceTaskRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceTaskResponseDto> CreateServiceTaskAsync(ServiceTaskRequestDto request)
        {
            var serviceTask = _mapper.Map<ServiceTask>(request);
            
            // Tính LaborCost nếu có StandardLaborTime và Branch.LaborRate
            if (serviceTask.StandardLaborTime.HasValue && serviceTask.StandardLaborTime.Value > 0)
            {
                // Lấy MaintenanceTicket để lấy Branch
                var maintenanceTicket = await _context.MaintenanceTickets
                    .Include(mt => mt.Branch)
                    .FirstOrDefaultAsync(mt => mt.Id == request.MaintenanceTicketId);
                
                if (maintenanceTicket?.Branch?.LaborRate.HasValue == true && maintenanceTicket.Branch.LaborRate.Value > 0)
                {
                    serviceTask.LaborCost = serviceTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
                }
            }
            
            // Nếu có ServiceCategoryId nhưng chưa có StandardLaborTime, lấy từ ServiceCategory
            if (serviceTask.ServiceCategoryId.HasValue && !serviceTask.StandardLaborTime.HasValue)
            {
                var serviceCategory = await _context.ServiceCategories
                    .FirstOrDefaultAsync(sc => sc.Id == serviceTask.ServiceCategoryId.Value);
                
                if (serviceCategory?.StandardLaborTime.HasValue == true)
                {
                    serviceTask.StandardLaborTime = serviceCategory.StandardLaborTime;
                    
                    // Nếu chưa có ActualLaborTime, set bằng StandardLaborTime
                    if (!serviceTask.ActualLaborTime.HasValue)
                    {
                        serviceTask.ActualLaborTime = serviceCategory.StandardLaborTime;
                    }
                    
                    // Tính LaborCost dựa trên StandardLaborTime
                    var maintenanceTicket = await _context.MaintenanceTickets
                        .Include(mt => mt.Branch)
                        .FirstOrDefaultAsync(mt => mt.Id == request.MaintenanceTicketId);
                    
                    if (maintenanceTicket?.Branch?.LaborRate.HasValue == true && maintenanceTicket.Branch.LaborRate.Value > 0)
                    {
                        serviceTask.LaborCost = serviceTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
                    }
                }
            }
            
            var createdTask = await _serviceTaskRepository.CreateAsync(serviceTask);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(serviceTask.MaintenanceTicketId ?? 0);
            
            return await GetServiceTaskByIdAsync(createdTask.Id);
        }

        public async Task<ServiceTaskResponseDto> UpdateServiceTaskAsync(ServiceTaskUpdateDto request)
        {
            var existingTask = await _serviceTaskRepository.GetByIdAsync(request.Id);
            if (existingTask == null)
                throw new ArgumentException("Service task not found");

            _mapper.Map(request, existingTask);
            
            // Tính lại LaborCost dựa trên StandardLaborTime
            if (existingTask.StandardLaborTime.HasValue && existingTask.StandardLaborTime.Value > 0)
            {
                var maintenanceTicket = await _context.MaintenanceTickets
                    .Include(mt => mt.Branch)
                    .FirstOrDefaultAsync(mt => mt.Id == existingTask.MaintenanceTicketId);
                
                if (maintenanceTicket?.Branch?.LaborRate.HasValue == true && maintenanceTicket.Branch.LaborRate.Value > 0)
                {
                    existingTask.LaborCost = existingTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
                }
            }
            else
            {
                // Nếu không có StandardLaborTime, set LaborCost = null
                existingTask.LaborCost = null;
            }
            
            var updatedTask = await _serviceTaskRepository.UpdateAsync(existingTask);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(existingTask.MaintenanceTicketId ?? 0);
            
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
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);
            if (serviceTask == null)
                return false;
            
            var maintenanceTicketId = serviceTask.MaintenanceTicketId ?? 0;
            var result = await _serviceTaskRepository.DeleteAsync(id);
            
            if (result)
            {
                // Cập nhật TotalEstimatedCost của MaintenanceTicket
                await UpdateMaintenanceTicketTotalCost(maintenanceTicketId);
            }
            
            return result;
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

        public async Task<ServiceTaskResponseDto> UpdateLaborTimeAsync(long id, decimal actualLaborTime)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);
            if (serviceTask == null)
                throw new ArgumentException("Service task not found");

            serviceTask.ActualLaborTime = actualLaborTime;
            
            // Tính lại LaborCost dựa trên StandardLaborTime (không phải ActualLaborTime)
            var maintenanceTicket = await _context.MaintenanceTickets
                .Include(mt => mt.Branch)
                .FirstOrDefaultAsync(mt => mt.Id == serviceTask.MaintenanceTicketId);
            
            if (serviceTask.StandardLaborTime.HasValue && serviceTask.StandardLaborTime.Value > 0 
                && maintenanceTicket?.Branch?.LaborRate.HasValue == true && maintenanceTicket.Branch.LaborRate.Value > 0)
            {
                serviceTask.LaborCost = serviceTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
            }
            else
            {
                // Nếu không có StandardLaborTime, set LaborCost = null
                serviceTask.LaborCost = null;
            }
            
            var updatedTask = await _serviceTaskRepository.UpdateAsync(serviceTask);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(serviceTask.MaintenanceTicketId ?? 0);
            
            return await GetServiceTaskByIdAsync(updatedTask.Id);
        }

        /// <summary>
        /// Cập nhật TotalEstimatedCost của MaintenanceTicket = ComponentTotal + LaborCostTotal
        /// </summary>
        private async Task UpdateMaintenanceTicketTotalCost(long maintenanceTicketId)
        {
            var maintenanceTicket = await _context.MaintenanceTickets
                .Include(mt => mt.TicketComponents)
                .Include(mt => mt.ServiceTasks)
                .FirstOrDefaultAsync(mt => mt.Id == maintenanceTicketId);
            
            if (maintenanceTicket == null)
                return;
            
            // Tính tổng phụ tùng
            var componentTotal = maintenanceTicket.TicketComponents
                .Sum(tc => tc.Quantity * (tc.UnitPrice ?? 0));
            
            // Tính tổng phí nhân công
            var laborCostTotal = maintenanceTicket.ServiceTasks
                .Sum(st => st.LaborCost ?? 0);
            
            // Cập nhật TotalEstimatedCost
            maintenanceTicket.TotalEstimatedCost = componentTotal + laborCostTotal;
            await _context.SaveChangesAsync();
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

                // Labor cost fields
                ServiceCategoryId = serviceTask.ServiceCategoryId,
                StandardLaborTime = serviceTask.StandardLaborTime,
                ActualLaborTime = serviceTask.ActualLaborTime,
                LaborCost = serviceTask.LaborCost,

                // Navigation properties
                MaintenanceTicketDescription = serviceTask.MaintenanceTicket?.Description,
                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,
                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 
                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()
                    : null,
                TechnicianName = serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                    : null,
                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name,
                ServiceCategoryName = serviceTask.ServiceCategory?.Name
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

                // Labor cost fields
                ServiceCategoryId = serviceTask.ServiceCategoryId,
                StandardLaborTime = serviceTask.StandardLaborTime,
                ActualLaborTime = serviceTask.ActualLaborTime,
                LaborCost = serviceTask.LaborCost,

                // Basic info
                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,
                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 
                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()
                    : null,
                TechnicianName = serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                    : null,
                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name,
                ServiceCategoryName = serviceTask.ServiceCategory?.Name
            };
        }
    }
}


