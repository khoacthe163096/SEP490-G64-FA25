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
        private readonly IMaintenanceTicketRepository _maintenanceTicketRepository;
        private readonly IServiceCategoryRepository _serviceCategoryRepository;
        private readonly IHistoryLogRepository _historyLogRepository;
        private readonly CarMaintenanceDbContext _context;
        private readonly IMapper _mapper;



        public ServiceTaskService(
            IServiceTaskRepository serviceTaskRepository, 
            IMaintenanceTicketRepository maintenanceTicketRepository,
            IServiceCategoryRepository serviceCategoryRepository,
            IHistoryLogRepository historyLogRepository,
            CarMaintenanceDbContext context,
            IMapper mapper)

        {

            _serviceTaskRepository = serviceTaskRepository;
            _maintenanceTicketRepository = maintenanceTicketRepository;
            _serviceCategoryRepository = serviceCategoryRepository;
            _historyLogRepository = historyLogRepository;
            _context = context;
            _mapper = mapper;

        }



        public async Task<ServiceTaskResponseDto> CreateServiceTaskAsync(ServiceTaskRequestDto request, long? userId = null)
        {
            var serviceTask = _mapper.Map<ServiceTask>(request);           
            // Tính LaborCost nếu có StandardLaborTime và Branch.LaborRate

            if (serviceTask.StandardLaborTime.HasValue && serviceTask.StandardLaborTime.Value > 0)
            {
                // Lấy MaintenanceTicket để lấy Branch
                var maintenanceTicket = await _maintenanceTicketRepository.GetByIdWithBranchAsync(request.MaintenanceTicketId);
                if (maintenanceTicket?.Branch?.LaborRate.HasValue != true || maintenanceTicket.Branch!.LaborRate!.Value <= 0)
                    throw new ArgumentException("Branch.LaborRate phải được cấu hình khi sử dụng StandardLaborTime");
               serviceTask.LaborCost = serviceTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;

            }           
            // Nếu có ServiceCategoryId nhưng chưa có StandardLaborTime, lấy từ ServiceCategory
            if (serviceTask.ServiceCategoryId.HasValue && !serviceTask.StandardLaborTime.HasValue)
            {
                var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(serviceTask.ServiceCategoryId.Value);
                if (serviceCategory?.StandardLaborTime.HasValue == true)
                {
                    serviceTask.StandardLaborTime = serviceCategory.StandardLaborTime;
                    // Nếu chưa có ActualLaborTime, set bằng StandardLaborTime
                    if (!serviceTask.ActualLaborTime.HasValue)
                    {
                        serviceTask.ActualLaborTime = serviceCategory.StandardLaborTime;
                    }                   
                    // Tính LaborCost dựa trên StandardLaborTime
                    var maintenanceTicket = await _maintenanceTicketRepository.GetByIdWithBranchAsync(request.MaintenanceTicketId);
                    if (maintenanceTicket?.Branch?.LaborRate.HasValue != true || maintenanceTicket.Branch!.LaborRate!.Value <= 0)
                        throw new ArgumentException("Branch.LaborRate phải được cấu hình khi sử dụng StandardLaborTime");
                    serviceTask.LaborCost = serviceTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
                }

            }

            

            var createdTask = await _serviceTaskRepository.CreateAsync(serviceTask);

            

            // Cập nhật TotalEstimatedCost của MaintenanceTicket

            await UpdateMaintenanceTicketTotalCost(serviceTask.MaintenanceTicketId ?? 0);

            // ✅ Tạo history log để ghi nhận việc thêm công việc
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var userName = user != null 
                    ? ($"{user.FirstName} {user.LastName}").Trim() 
                    : "Unknown";
                
                var taskDetails = $"Thêm công việc '{createdTask.TaskName ?? "N/A"}'";
                if (createdTask.StandardLaborTime.HasValue)
                {
                    taskDetails += $" - Thời gian chuẩn: {createdTask.StandardLaborTime.Value} giờ";
                }
                if (createdTask.LaborCost.HasValue)
                {
                    taskDetails += $" - Phí nhân công: {createdTask.LaborCost.Value:N0} ₫";
                }
                
                await CreateHistoryLogAsync(
                    userId: userId,
                    action: "ADD_SERVICE_TASK",
                    maintenanceTicketId: serviceTask.MaintenanceTicketId,
                    newData: $"{taskDetails} bởi {userName}"
                );
            }

            

            return await GetServiceTaskByIdAsync(createdTask.Id);

        }



        public async Task<ServiceTaskResponseDto> UpdateServiceTaskAsync(ServiceTaskUpdateDto request)

        {

            var existingTask = await _serviceTaskRepository.GetByIdAsync(request.Id);

            if (existingTask == null)

                throw new ArgumentException("Service task not found");

            // ✅ VALIDATION: Không cho phép sửa công việc đã hoàn thành
            if (existingTask.StatusCode == "DONE" || existingTask.StatusCode == "COMPLETED")
            {
                throw new ArgumentException("Không thể sửa công việc đã hoàn thành");
            }

            _mapper.Map(request, existingTask);

            

            // Tính lại LaborCost dựa trên StandardLaborTime

            if (existingTask.StandardLaborTime.HasValue && existingTask.StandardLaborTime.Value > 0)

            {

                var maintenanceTicket = await _maintenanceTicketRepository.GetByIdWithBranchAsync(existingTask.MaintenanceTicketId ?? 0);
                if (maintenanceTicket?.Branch?.LaborRate.HasValue != true || maintenanceTicket.Branch!.LaborRate!.Value <= 0)
                    throw new ArgumentException("Branch.LaborRate phải được cấu hình khi sử dụng StandardLaborTime");
                existingTask.LaborCost = existingTask.StandardLaborTime.Value * maintenanceTicket.Branch.LaborRate.Value;
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

            // ✅ VALIDATION: Không cho phép xóa công việc đã hoàn thành
            if (serviceTask.StatusCode == "DONE" || serviceTask.StatusCode == "COMPLETED")
            {
                throw new ArgumentException("Không thể xóa công việc đã hoàn thành");
            }          

            var maintenanceTicketId = serviceTask.MaintenanceTicketId ?? 0;

            var result = await _serviceTaskRepository.DeleteAsync(id);

            

            if (result)

            {

                // Cập nhật TotalEstimatedCost của MaintenanceTicket

                await UpdateMaintenanceTicketTotalCost(maintenanceTicketId);

            }

            

            return result;

        }



        public async Task<ServiceTaskResponseDto> UpdateStatusAsync(long id, string statusCode, long? userId = null, string? completionNote = null)

        {

            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);

            if (serviceTask == null)

                throw new ArgumentException("Service task not found");

            var oldStatus = serviceTask.StatusCode;

            // ✅ Tự động set StartTime khi chuyển sang IN_PROGRESS
            if (statusCode == "IN_PROGRESS" && oldStatus != "IN_PROGRESS" && !serviceTask.StartTime.HasValue)
            {
                serviceTask.StartTime = DateTime.UtcNow;
            }

            // ✅ Tự động set EndTime khi chuyển sang DONE hoặc COMPLETED
            if ((statusCode == "DONE" || statusCode == "COMPLETED") && oldStatus != statusCode && !serviceTask.EndTime.HasValue)
            {
                serviceTask.EndTime = DateTime.UtcNow;
            }

            // ✅ Set CompletionNote nếu có
            if (!string.IsNullOrWhiteSpace(completionNote))
            {
                serviceTask.CompletionNote = completionNote;
            }

            serviceTask.StatusCode = statusCode;

            var updatedTask = await _serviceTaskRepository.UpdateAsync(serviceTask);

            // ✅ Tạo history log để ghi nhận việc cập nhật trạng thái công việc
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var userName = user != null 
                    ? ($"{user.FirstName} {user.LastName}").Trim() 
                    : "Unknown";
                
                string action = "";
                string statusText = "";
                
                if (statusCode == "IN_PROGRESS" && oldStatus != "IN_PROGRESS")
                {
                    action = "START_SERVICE_TASK";
                    statusText = "Bắt đầu công việc";
                }
                else if ((statusCode == "DONE" || statusCode == "COMPLETED") && oldStatus != statusCode)
                {
                    action = "COMPLETE_SERVICE_TASK";
                    statusText = "Hoàn thành công việc";
                }
                else
                {
                    action = "UPDATE_SERVICE_TASK_STATUS";
                    statusText = "Cập nhật trạng thái công việc";
                }
                
                var taskDetails = $"{statusText} '{serviceTask.TaskName ?? "N/A"}'";
                if (oldStatus != statusCode)
                {
                    taskDetails += $" (Từ {oldStatus} → {statusCode})";
                }
                
                await CreateHistoryLogAsync(
                    userId: userId,
                    action: action,
                    maintenanceTicketId: serviceTask.MaintenanceTicketId,
                    oldData: $"Trạng thái cũ: {oldStatus}",
                    newData: $"{taskDetails} bởi {userName}"
                );
            }

            return await GetServiceTaskByIdAsync(updatedTask.Id);

        }



        public async Task<ServiceTaskResponseDto> UpdateLaborTimeAsync(long id, decimal actualLaborTime)

        {

            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);

            if (serviceTask == null)

                throw new ArgumentException("Service task not found");



            serviceTask.ActualLaborTime = actualLaborTime;

            

            // Tính lại LaborCost dựa trên StandardLaborTime (không phải ActualLaborTime)

            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdWithBranchAsync(serviceTask.MaintenanceTicketId ?? 0);

            

            if (serviceTask.StandardLaborTime.HasValue && serviceTask.StandardLaborTime.Value > 0)

            {

                if (maintenanceTicket?.Branch?.LaborRate.HasValue != true || maintenanceTicket.Branch!.LaborRate!.Value <= 0)

                    throw new ArgumentException("Branch.LaborRate phải được cấu hình khi sử dụng StandardLaborTime");

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
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdWithCostDetailsAsync(maintenanceTicketId);       
            if (maintenanceTicket == null)

                return;

            // Tính tổng phụ tùng

            var componentTotal = maintenanceTicket.TicketComponents

                .Sum(tc => (tc.ActualQuantity ?? tc.Quantity) * (tc.UnitPrice ?? 0));
        
            // Tính tổng phí nhân công

            var laborCostTotal = maintenanceTicket.ServiceTasks

                .Sum(st => st.LaborCost ?? 0);     

            // Cập nhật TotalEstimatedCost

            maintenanceTicket.TotalEstimatedCost = componentTotal + laborCostTotal;

            await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);

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

                // New fields
                TechnicianId = serviceTask.TechnicianId,
                StartTime = serviceTask.StartTime,
                EndTime = serviceTask.EndTime,
                DisplayOrder = serviceTask.DisplayOrder,
                CompletionNote = serviceTask.CompletionNote,

                // Navigation properties
                MaintenanceTicketCode = serviceTask.MaintenanceTicket?.Code,
                MaintenanceTicketDescription = serviceTask.MaintenanceTicket?.Description,

                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,

                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 

                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()

                    : null,

                TechnicianName = serviceTask.Technician != null
                    ? $"{serviceTask.Technician.FirstName} {serviceTask.Technician.LastName}".Trim()
                    : (serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                        : null),

                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name,

                ServiceCategoryName = serviceTask.ServiceCategory?.Name,
                
                // ✅ Map danh sách nhiều kỹ thuật viên
                Technicians = serviceTask.ServiceTaskTechnicians?.Select(stt => new ServiceTaskTechnicianDto
                {
                    TechnicianId = stt.TechnicianId,
                    TechnicianName = stt.Technician != null 
                        ? $"{stt.Technician.FirstName} {stt.Technician.LastName}".Trim() 
                        : null,
                    RoleInTask = stt.RoleInTask,
                    AssignedDate = stt.AssignedDate
                }).ToList()

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

                // New fields
                TechnicianId = serviceTask.TechnicianId,
                StartTime = serviceTask.StartTime,
                EndTime = serviceTask.EndTime,
                DisplayOrder = serviceTask.DisplayOrder,
                CompletionNote = serviceTask.CompletionNote,

                // Basic info
                MaintenanceTicketCode = serviceTask.MaintenanceTicket?.Code,
                CarName = serviceTask.MaintenanceTicket?.Car?.CarName,

                CustomerName = serviceTask.MaintenanceTicket?.Car?.User != null 

                    ? $"{serviceTask.MaintenanceTicket.Car.User.FirstName} {serviceTask.MaintenanceTicket.Car.User.LastName}".Trim()

                    : null,

                TechnicianName = serviceTask.Technician != null
                    ? $"{serviceTask.Technician.FirstName} {serviceTask.Technician.LastName}".Trim()
                    : (serviceTask.MaintenanceTicket?.Technician != null
                    ? $"{serviceTask.MaintenanceTicket.Technician.FirstName} {serviceTask.MaintenanceTicket.Technician.LastName}".Trim()
                        : null),

                BranchName = serviceTask.MaintenanceTicket?.Branch?.Name,

                ServiceCategoryName = serviceTask.ServiceCategory?.Name,
                
                // ✅ Map danh sách nhiều kỹ thuật viên
                Technicians = serviceTask.ServiceTaskTechnicians?.Select(stt => new ServiceTaskTechnicianDto
                {
                    TechnicianId = stt.TechnicianId,
                    TechnicianName = stt.Technician != null 
                        ? $"{stt.Technician.FirstName} {stt.Technician.LastName}".Trim() 
                        : null,
                    RoleInTask = stt.RoleInTask,
                    AssignedDate = stt.AssignedDate
                }).ToList()

            };

        }

        /// <summary>
        /// Gán kỹ thuật viên cho ServiceTask
        /// </summary>
        public async Task<ServiceTaskResponseDto> AssignTechniciansAsync(long id, ServiceTaskAssignTechniciansDto request, long? userId = null)
        {
            var serviceTask = await _serviceTaskRepository.GetByIdAsync(id);
            if (serviceTask == null)
                throw new ArgumentException("Service task not found");

            // Xóa tất cả technicians hiện tại
            var existingTechnicians = _context.ServiceTaskTechnicians
                .Where(stt => stt.ServiceTaskId == id)
                .ToList();
            _context.ServiceTaskTechnicians.RemoveRange(existingTechnicians);

            // Thêm technicians mới
            if (request.TechnicianIds != null && request.TechnicianIds.Count > 0)
            {
                var newTechnicians = request.TechnicianIds.Select(techId => new ServiceTaskTechnician
                {
                    ServiceTaskId = id,
                    TechnicianId = techId,
                    AssignedDate = DateTime.UtcNow,
                    RoleInTask = (request.PrimaryTechnicianId.HasValue && techId == request.PrimaryTechnicianId.Value) 
                        ? "PRIMARY" 
                        : "ASSISTANT"
                }).ToList();

                _context.ServiceTaskTechnicians.AddRange(newTechnicians);
            }

            await _context.SaveChangesAsync();

            // ✅ Tạo history log
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var userName = user != null 
                    ? ($"{user.FirstName} {user.LastName}").Trim() 
                    : "Unknown";
                
                var techNames = request.TechnicianIds?.Select(techId => 
                {
                    var tech = _context.Users.Find(techId);
                    return tech != null ? $"{tech.FirstName} {tech.LastName}".Trim() : $"#{techId}";
                }).ToList() ?? new List<string>();
                
                var techList = string.Join(", ", techNames);
                await CreateHistoryLogAsync(
                    userId: userId,
                    action: "ASSIGN_SERVICE_TASK_TECHNICIANS",
                    maintenanceTicketId: serviceTask.MaintenanceTicketId,
                    newData: $"Gán kỹ thuật viên cho công việc '{serviceTask.TaskName ?? "N/A"}': {techList} bởi {userName}"
                );
            }

            return await GetServiceTaskByIdAsync(id);
        }

        /// <summary>
        /// Helper method để tạo history log
        /// </summary>
        private async Task CreateHistoryLogAsync(long? userId, string action, long? maintenanceTicketId = null, string? oldData = null, string? newData = null)
        {
            var historyLog = new HistoryLog
            {
                UserId = userId,
                MaintenanceTicketId = maintenanceTicketId,
                Action = action,
                OldData = oldData,
                NewData = newData,
                CreatedAt = DateTime.UtcNow
            };

            await _historyLogRepository.CreateAsync(historyLog);
        }

    }

}






      