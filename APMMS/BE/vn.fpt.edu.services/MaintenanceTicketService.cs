using AutoMapper;
using BE.vn.fpt.edu.DTOs.MaintenanceTicket;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class MaintenanceTicketService : IMaintenanceTicketService
    {
        private readonly IMaintenanceTicketRepository _maintenanceTicketRepository;
        private readonly IVehicleCheckinRepository _vehicleCheckinRepository;
        private readonly IHistoryLogRepository _historyLogRepository;
        private readonly IServicePackageService _servicePackageService;
        private readonly ITicketComponentService _ticketComponentService;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public MaintenanceTicketService(
            IMaintenanceTicketRepository maintenanceTicketRepository,
            IVehicleCheckinRepository vehicleCheckinRepository,
            IHistoryLogRepository historyLogRepository,
            IServicePackageService servicePackageService,
            ITicketComponentService ticketComponentService,
            IMapper mapper,
            CarMaintenanceDbContext context)
        {
            _maintenanceTicketRepository = maintenanceTicketRepository;
            _vehicleCheckinRepository = vehicleCheckinRepository;
            _historyLogRepository = historyLogRepository;
            _servicePackageService = servicePackageService;
            _ticketComponentService = ticketComponentService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResponseDto> CreateMaintenanceTicketAsync(RequestDto request)
        {
            var maintenanceTicket = _mapper.Map<MaintenanceTicket>(request);
            // Tự sinh code 7 ký tự ngẫu nhiên
            maintenanceTicket.Code = await GenerateUniqueCodeAsync();
            maintenanceTicket.CreatedAt = DateTime.UtcNow;
            var createdTicket = await _maintenanceTicketRepository.CreateAsync(maintenanceTicket);
            return _mapper.Map<ResponseDto>(createdTicket);
        }

        public async Task<ResponseDto> CreateFromVehicleCheckinAsync(CreateFromCheckinDto request)
        {
            // Lấy thông tin Vehicle Check-in
            var vehicleCheckin = await _vehicleCheckinRepository.GetByIdAsync(request.VehicleCheckinId);
            if (vehicleCheckin == null)
                throw new ArgumentException("Vehicle check-in not found");

            // Kiểm tra xem VehicleCheckin đã có MaintenanceTicket chưa
            if (vehicleCheckin.MaintenanceRequestId.HasValue && vehicleCheckin.MaintenanceRequestId.Value > 0)
                throw new ArgumentException("Vehicle check-in already has a maintenance ticket");

            // ⚠️ QUAN TRỌNG: Không cho phép gán kỹ thuật viên khi tạo phiếu
            // Phiếu phải được tạo ở trạng thái PENDING và chưa có kỹ thuật viên
            // Kỹ thuật viên chỉ được gán sau khi Consulter thực hiện hành động "Assign Technician"
            
            // Tạo Maintenance Ticket từ thông tin Check-in
            var maintenanceTicket = new MaintenanceTicket
            {
                VehicleCheckinId = request.VehicleCheckinId,
                CarId = vehicleCheckin.CarId,
                ConsulterId = request.ConsulterId,
                TechnicianId = null, // ✅ Luôn null - không gán kỹ thuật viên khi tạo
                BranchId = request.BranchId,
                ScheduleServiceId = request.ScheduleServiceId,
                StatusCode = "PENDING", // ✅ Luôn PENDING khi tạo từ check-in
                PriorityLevel = "NORMAL",
                Description = request.Description,
                Code = await GenerateUniqueCodeAsync() // Tự sinh code 7 ký tự ngẫu nhiên
            };

            maintenanceTicket.CreatedAt = DateTime.UtcNow;
            var createdTicket = await _maintenanceTicketRepository.CreateAsync(maintenanceTicket);

            // ✅ KHÔNG lưu kỹ thuật viên vào bảng MaintenanceTicketTechnician
            // Kỹ thuật viên chỉ được gán thông qua endpoint AddTechniciansAsync

            // ✅ Tạo history log để ghi nhận việc tạo phiếu
            var consulterName = createdTicket.Consulter != null 
                ? ($"{createdTicket.Consulter.FirstName} {createdTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            await CreateHistoryLogAsync(
                userId: request.ConsulterId,
                action: "CREATE_TICKET",
                maintenanceTicketId: createdTicket.Id,
                oldData: null,
                newData: $"Phiếu bảo dưỡng được tạo bởi {consulterName} lúc {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}"
            );

            // Lấy thông tin đầy đủ để trả về
            var fullTicket = await _maintenanceTicketRepository.GetByIdAsync(createdTicket.Id);
            
            // Map thủ công thay vì dùng AutoMapper và điền đầy đủ thông tin để FE hiển thị ngay
            var response = new ResponseDto
            {
                Id = fullTicket.Id,
                CarId = fullTicket.CarId,
                ConsulterId = fullTicket.ConsulterId,
                TechnicianId = fullTicket.TechnicianId,
                BranchId = fullTicket.BranchId,
                StatusCode = fullTicket.StatusCode,
                ScheduleServiceId = fullTicket.ScheduleServiceId,
                Code = fullTicket.Code,
                TotalEstimatedCost = fullTicket.TotalEstimatedCost,
                CreatedDate = fullTicket.CreatedAt,
                StartTime = fullTicket.StartTime,
                EndTime = fullTicket.EndTime,
                PriorityLevel = fullTicket.PriorityLevel,
                Description = fullTicket.Description,
                // Tên hiển thị
                CarName = fullTicket.Car != null ? fullTicket.Car.CarName : null,
                ConsulterName = fullTicket.Consulter != null ? ($"{fullTicket.Consulter.FirstName} {fullTicket.Consulter.LastName}").Trim() : null,
                TechnicianName = fullTicket.Technician != null ? ($"{fullTicket.Technician.FirstName} {fullTicket.Technician.LastName}").Trim() : null,
                BranchName = fullTicket.Branch != null ? fullTicket.Branch.Name : null,
                ScheduleServiceName = fullTicket.ScheduleService != null ? fullTicket.ScheduleService.ScheduledDate.ToString("dd/MM/yyyy") : null,
                // Khách hàng và xe
                CustomerName = fullTicket.Car != null && fullTicket.Car.User != null ? ($"{fullTicket.Car.User.FirstName} {fullTicket.Car.User.LastName}").Trim() : null,
                CustomerPhone = fullTicket.Car != null && fullTicket.Car.User != null ? fullTicket.Car.User.Phone : null,
                CustomerAddress = fullTicket.Car != null && fullTicket.Car.User != null && !string.IsNullOrWhiteSpace(fullTicket.Car.User.Address)
                    ? fullTicket.Car.User.Address
                    : null,
                LicensePlate = fullTicket.Car != null ? fullTicket.Car.LicensePlate : null,
                CarModel = fullTicket.Car != null ? fullTicket.Car.CarModel : null,
                // Danh sách tất cả kỹ thuật viên
                Technicians = fullTicket.MaintenanceTicketTechnicians != null && fullTicket.MaintenanceTicketTechnicians.Count > 0
                    ? fullTicket.MaintenanceTicketTechnicians.Select(mtt => new TechnicianInfoDto
                    {
                        TechnicianId = mtt.TechnicianId,
                        TechnicianName = mtt.Technician != null ? ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim() : null,
                        RoleInTicket = mtt.RoleInTicket,
                        AssignedDate = mtt.AssignedDate
                    }).ToList()
                    : new List<TechnicianInfoDto>()
            };

            // Thêm thông tin từ Vehicle Check-in
            response.VehicleCheckinId = vehicleCheckin.Id;
            response.Mileage = vehicleCheckin.Mileage;
            response.CheckinNotes = vehicleCheckin.Notes;
            response.CheckinImages = vehicleCheckin.VehicleCheckinImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>();

            return response;
        }

        public async Task<ResponseDto> UpdateMaintenanceTicketAsync(long id, RequestDto request)
        {
            var existingTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (existingTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // Lưu thông tin cũ để so sánh
            var oldDescription = existingTicket.Description;
            var oldPriority = existingTicket.PriorityLevel;

            _mapper.Map(request, existingTicket);
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(existingTicket);

            // ✅ Tạo history log để ghi nhận việc cập nhật thông tin
            var updaterName = updatedTicket.Consulter != null 
                ? ($"{updatedTicket.Consulter.FirstName} {updatedTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            var changes = new List<string>();
            if (oldDescription != updatedTicket.Description) changes.Add("mô tả");
            if (oldPriority != updatedTicket.PriorityLevel) changes.Add($"mức ưu tiên: {oldPriority} → {updatedTicket.PriorityLevel}");
            
            if (changes.Count > 0)
            {
                await CreateHistoryLogAsync(
                    userId: request.ConsulterId,
                    action: "UPDATE_INFO",
                    maintenanceTicketId: id,
                    oldData: $"Description: {oldDescription}, Priority: {oldPriority}",
                    newData: $"Description: {updatedTicket.Description}, Priority: {updatedTicket.PriorityLevel}"
                );
            }

            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        public async Task<ResponseDto> GetMaintenanceTicketByIdAsync(long id)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            var response = _mapper.Map<ResponseDto>(maintenanceTicket);
            
            // Thêm danh sách kỹ thuật viên
            if (maintenanceTicket.MaintenanceTicketTechnicians != null && maintenanceTicket.MaintenanceTicketTechnicians.Count > 0)
            {
                response.Technicians = maintenanceTicket.MaintenanceTicketTechnicians.Select(mtt => new TechnicianInfoDto
                {
                    TechnicianId = mtt.TechnicianId,
                    TechnicianName = mtt.Technician != null ? ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim() : null,
                    RoleInTicket = mtt.RoleInTicket,
                    AssignedDate = mtt.AssignedDate
                }).ToList();
            }
            
            return response;
        }

        public async Task<List<ListResponseDto>> GetAllMaintenanceTicketsAsync(int page = 1, int pageSize = 10)
        {
            var maintenanceTickets = await _maintenanceTicketRepository.GetAllAsync(page, pageSize);
            return _mapper.Map<List<ListResponseDto>>(maintenanceTickets);
        }

        public async Task<List<ListResponseDto>> GetMaintenanceTicketsByCarIdAsync(long carId)
        {
            var maintenanceTickets = await _maintenanceTicketRepository.GetByCarIdAsync(carId);
            return _mapper.Map<List<ListResponseDto>>(maintenanceTickets);
        }

        public async Task<List<ListResponseDto>> GetMaintenanceTicketsByStatusAsync(string statusCode)
        {
            var maintenanceTickets = await _maintenanceTicketRepository.GetByStatusAsync(statusCode);
            return _mapper.Map<List<ListResponseDto>>(maintenanceTickets);
        }

        public async Task<bool> DeleteMaintenanceTicketAsync(long id)
        {
            return await _maintenanceTicketRepository.DeleteAsync(id);
        }

        public async Task<ResponseDto> UpdateStatusAsync(long id, string statusCode)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            var oldStatus = maintenanceTicket.StatusCode;
            maintenanceTicket.StatusCode = statusCode;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
            
            // ✅ Tạo history log để ghi nhận việc cập nhật trạng thái
            var userName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            var statusMap = new Dictionary<string, string>
            {
                { "PENDING", "Chờ xử lý" },
                { "ASSIGNED", "Đã gán" },
                { "IN_PROGRESS", "Đang thực hiện" },
                { "COMPLETED", "Hoàn thành" },
                { "CANCELLED", "Đã hủy" }
            };
            var oldStatusName = statusMap.ContainsKey(oldStatus) ? statusMap[oldStatus] : oldStatus;
            var newStatusName = statusMap.ContainsKey(statusCode) ? statusMap[statusCode] : statusCode;
            
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.ConsulterId,
                action: "UPDATE_STATUS",
                maintenanceTicketId: id,
                oldData: $"Status: {oldStatus}",
                newData: $"Cập nhật trạng thái: '{oldStatusName}' → '{newStatusName}' bởi {userName}"
            );
            
            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        public async Task<ResponseDto> AssignTechnicianAsync(long id, long technicianId)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            maintenanceTicket.TechnicianId = technicianId;
            maintenanceTicket.StatusCode = "ASSIGNED";
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        public async Task<ResponseDto> AddTechniciansAsync(long id, List<long> technicianIds, long? primaryId)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Chỉ cho phép gán/cập nhật kỹ thuật viên khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS
            if (maintenanceTicket.StatusCode != "PENDING" && maintenanceTicket.StatusCode != "ASSIGNED" && maintenanceTicket.StatusCode != "IN_PROGRESS")
                throw new ArgumentException($"Không thể gán/cập nhật kỹ thuật viên cho phiếu có trạng thái {maintenanceTicket.StatusCode}. Chỉ có thể gán khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS.");

            // Cho phép danh sách rỗng (để xóa tất cả)
            // Không validate bắt buộc phải có ít nhất 1 kỹ thuật viên
            
            // Lưu trạng thái cũ để ghi log
            var oldStatus = maintenanceTicket.StatusCode;
            
            // Lấy danh sách kỹ thuật viên hiện tại
            var existingTechnicians = maintenanceTicket.MaintenanceTicketTechnicians?.ToList() ?? new List<MaintenanceTicketTechnician>();
            var existingIds = existingTechnicians.Select(x => x.TechnicianId).ToHashSet();
            
            // Xóa những người không còn trong danh sách mới
            var toRemove = existingTechnicians.Where(e => !technicianIds.Contains(e.TechnicianId)).ToList();
            if (toRemove.Count > 0)
            {
                _context.MaintenanceTicketTechnicians.RemoveRange(toRemove);
            }

            // Thêm các kỹ thuật viên mới (chưa có trong danh sách)
            foreach (var techId in technicianIds.Distinct())
            {
                if (!existingIds.Contains(techId))
                {
                    _context.MaintenanceTicketTechnicians.Add(new MaintenanceTicketTechnician
                    {
                        MaintenanceTicketId = id,
                        TechnicianId = techId,
                        AssignedDate = DateTime.UtcNow,
                        RoleInTicket = (primaryId.HasValue && primaryId.Value == techId) ? "PRIMARY" : "ASSISTANT"
                    });
                }
            }
            
            // Cập nhật role cho các bản ghi đã tồn tại nếu có primaryId
            if (primaryId.HasValue)
            {
                foreach (var mtt in _context.MaintenanceTicketTechnicians.Where(x => x.MaintenanceTicketId == id))
                {
                    mtt.RoleInTicket = (mtt.TechnicianId == primaryId.Value) ? "PRIMARY" : "ASSISTANT";
                }
                // cập nhật TechnicianId chính trên ticket
                maintenanceTicket.TechnicianId = primaryId.Value;
            }
            else if (technicianIds.Count > 0)
            {
                // Nếu không có primaryId nhưng có kỹ thuật viên, chọn người đầu tiên làm chính
                maintenanceTicket.TechnicianId = technicianIds[0];
                foreach (var mtt in _context.MaintenanceTicketTechnicians.Where(x => x.MaintenanceTicketId == id))
                {
                    mtt.RoleInTicket = (mtt.TechnicianId == technicianIds[0]) ? "PRIMARY" : "ASSISTANT";
                }
            }
            else
            {
                // Nếu không còn kỹ thuật viên nào, xóa technician chính
                maintenanceTicket.TechnicianId = null;
            }

            // ✅ Chuyển trạng thái: 
            // - Nếu có kỹ thuật viên và đang PENDING → IN_PROGRESS
            // - Nếu không còn kỹ thuật viên và đang IN_PROGRESS/ASSIGNED → PENDING
            if (technicianIds.Count > 0)
            {
                if (maintenanceTicket.StatusCode == "PENDING")
                {
                    maintenanceTicket.StatusCode = "IN_PROGRESS";
                }
            }
            else
            {
                // Không còn kỹ thuật viên, chuyển về PENDING
                if (maintenanceTicket.StatusCode == "IN_PROGRESS" || maintenanceTicket.StatusCode == "ASSIGNED")
                {
                    maintenanceTicket.StatusCode = "PENDING";
                }
            }
            _context.MaintenanceTickets.Update(maintenanceTicket);
            await _context.SaveChangesAsync();

            // ✅ Tạo history log để ghi nhận việc gán kỹ thuật viên
            var refreshed = await _maintenanceTicketRepository.GetByIdAsync(id);
            var technicianDetails = refreshed?.MaintenanceTicketTechnicians?.Select(mtt => 
            {
                var techName = mtt.Technician != null 
                    ? ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim() 
                    : $"ID: {mtt.TechnicianId}";
                var role = mtt.RoleInTicket == "PRIMARY" ? "Chính" : "Phụ";
                return $"{techName} ({role})";
            }).ToList() ?? new List<string>();
            
            var consulterName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            
            foreach (var techDetail in technicianDetails)
            {
                await CreateHistoryLogAsync(
                    userId: maintenanceTicket.ConsulterId,
                    action: "ASSIGN_TECHNICIAN",
                    maintenanceTicketId: id,
                    oldData: $"Status: {oldStatus}",
                    newData: $"Gán kỹ thuật viên {techDetail} bởi {consulterName}"
                );
            }

            // Trả về chi tiết mới nhất
            var response = _mapper.Map<ResponseDto>(refreshed);
            if (refreshed?.MaintenanceTicketTechnicians != null)
            {
                response.Technicians = refreshed.MaintenanceTicketTechnicians.Select(mtt => new TechnicianInfoDto
                {
                    TechnicianId = mtt.TechnicianId,
                    TechnicianName = mtt.Technician != null ? ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim() : null,
                    RoleInTicket = mtt.RoleInTicket,
                    AssignedDate = mtt.AssignedDate
                }).ToList();
            }
            return response;
        }

        public async Task<ResponseDto> RemoveTechniciansAsync(long id)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Chỉ cho phép xóa kỹ thuật viên khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS
            if (maintenanceTicket.StatusCode != "PENDING" && maintenanceTicket.StatusCode != "ASSIGNED" && maintenanceTicket.StatusCode != "IN_PROGRESS")
                throw new ArgumentException($"Không thể xóa kỹ thuật viên cho phiếu có trạng thái {maintenanceTicket.StatusCode}. Chỉ có thể xóa khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS.");

            // Lưu thông tin cũ để ghi log (phải lấy trước khi xóa)
            var oldStatus = maintenanceTicket.StatusCode;
            var oldTechnicianIds = maintenanceTicket.MaintenanceTicketTechnicians?.Select(x => x.TechnicianId).ToList() ?? new List<long>();
            var oldPrimaryId = maintenanceTicket.TechnicianId;
            
            // Lấy tên kỹ thuật viên đã bị xóa (trước khi xóa)
            var removedTechnicians = new List<string>();
            if (maintenanceTicket.MaintenanceTicketTechnicians != null && maintenanceTicket.MaintenanceTicketTechnicians.Count > 0)
            {
                foreach (var mtt in maintenanceTicket.MaintenanceTicketTechnicians)
                {
                    if (mtt.Technician != null)
                    {
                        var techName = ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim();
                        removedTechnicians.Add(techName);
                    }
                }
            }

            // Xóa tất cả kỹ thuật viên khỏi bảng trung gian
            if (maintenanceTicket.MaintenanceTicketTechnicians != null && maintenanceTicket.MaintenanceTicketTechnicians.Count > 0)
            {
                _context.MaintenanceTicketTechnicians.RemoveRange(maintenanceTicket.MaintenanceTicketTechnicians);
            }

            // Xóa technician chính
            maintenanceTicket.TechnicianId = null;

            // ✅ Chuyển trạng thái về PENDING nếu đang ở IN_PROGRESS hoặc ASSIGNED
            if (maintenanceTicket.StatusCode == "IN_PROGRESS" || maintenanceTicket.StatusCode == "ASSIGNED")
            {
                maintenanceTicket.StatusCode = "PENDING";
            }

            _context.MaintenanceTickets.Update(maintenanceTicket);
            await _context.SaveChangesAsync();

            // ✅ Tạo history log để ghi nhận việc xóa kỹ thuật viên
            
            var consulterName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            
            foreach (var techName in removedTechnicians)
            {
                await CreateHistoryLogAsync(
                    userId: maintenanceTicket.ConsulterId,
                    action: "REMOVE_TECHNICIAN",
                    maintenanceTicketId: id,
                    oldData: $"Status: {oldStatus}",
                    newData: $"Gỡ kỹ thuật viên {techName} khỏi phiếu bởi {consulterName}"
                );
            }

            // Trả về chi tiết mới nhất
            var refreshed = await _maintenanceTicketRepository.GetByIdAsync(id);
            var response = _mapper.Map<ResponseDto>(refreshed);
            if (refreshed?.MaintenanceTicketTechnicians != null)
            {
                response.Technicians = refreshed.MaintenanceTicketTechnicians.Select(mtt => new TechnicianInfoDto
                {
                    TechnicianId = mtt.TechnicianId,
                    TechnicianName = mtt.Technician != null ? ($"{mtt.Technician.FirstName} {mtt.Technician.LastName}").Trim() : null,
                    RoleInTicket = mtt.RoleInTicket,
                    AssignedDate = mtt.AssignedDate
                }).ToList();
            }
            return response;
        }

        public async Task<ResponseDto> StartMaintenanceAsync(long id)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            var oldStatus = maintenanceTicket.StatusCode;
            maintenanceTicket.StatusCode = "IN_PROGRESS";
            maintenanceTicket.StartTime = DateTime.UtcNow;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
            
            // ✅ Tạo history log để ghi nhận việc cập nhật trạng thái
            var userName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            var statusMap = new Dictionary<string, string>
            {
                { "PENDING", "Chờ xử lý" },
                { "ASSIGNED", "Đã gán" },
                { "IN_PROGRESS", "Đang thực hiện" },
                { "COMPLETED", "Hoàn thành" },
                { "CANCELLED", "Đã hủy" }
            };
            var oldStatusName = statusMap.ContainsKey(oldStatus) ? statusMap[oldStatus] : oldStatus;
            var newStatusName = statusMap.ContainsKey("IN_PROGRESS") ? statusMap["IN_PROGRESS"] : "IN_PROGRESS";
            
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.ConsulterId,
                action: "UPDATE_STATUS",
                maintenanceTicketId: id,
                oldData: $"Status: {oldStatus}",
                newData: $"Cập nhật trạng thái: '{oldStatusName}' → '{newStatusName}' bởi {userName}"
            );
            
            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        public async Task<ResponseDto> CompleteMaintenanceAsync(long id)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Chỉ cho phép hoàn thành khi phiếu đang ở trạng thái IN_PROGRESS
            if (maintenanceTicket.StatusCode != "IN_PROGRESS")
                throw new ArgumentException($"Không thể hoàn thành phiếu ở trạng thái {maintenanceTicket.StatusCode}. Chỉ có thể hoàn thành khi phiếu ở trạng thái IN_PROGRESS.");

            // Lưu trạng thái cũ để ghi log
            var oldStatus = maintenanceTicket.StatusCode;

            maintenanceTicket.StatusCode = "COMPLETED";
            maintenanceTicket.EndTime = DateTime.UtcNow;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);

            // ✅ Tạo history log để ghi nhận việc hoàn thành bảo dưỡng
            var userName = maintenanceTicket.Technician != null 
                ? ($"{maintenanceTicket.Technician.FirstName} {maintenanceTicket.Technician.LastName}").Trim() 
                : (maintenanceTicket.Consulter != null 
                    ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                    : "Unknown");
            
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.TechnicianId ?? maintenanceTicket.ConsulterId,
                action: "COMPLETE_MAINTENANCE",
                maintenanceTicketId: id,
                oldData: $"Status: {oldStatus}",
                newData: $"Phiếu hoàn thành bởi {userName}"
            );

            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        public async Task<ResponseDto> CancelMaintenanceTicketAsync(long id)
        {
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Không cho phép hủy phiếu đã hoàn thành hoặc đã hủy
            if (maintenanceTicket.StatusCode == "COMPLETED")
                throw new ArgumentException("Không thể hủy phiếu đã hoàn thành.");

            if (maintenanceTicket.StatusCode == "CANCELLED")
                throw new ArgumentException("Phiếu này đã được hủy trước đó.");

            // Lưu trạng thái cũ để ghi log
            var oldStatus = maintenanceTicket.StatusCode;

            maintenanceTicket.StatusCode = "CANCELLED";
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);

            // ✅ Tạo history log để ghi nhận việc hủy phiếu
            var userName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.ConsulterId,
                action: "CANCEL_MAINTENANCE",
                maintenanceTicketId: id,
                oldData: $"Status: {oldStatus}",
                newData: $"Phiếu bị hủy bởi {userName}"
            );

            return _mapper.Map<ResponseDto>(updatedTicket);
        }

        /// <summary>
        /// Tạo mã phiếu bảo dưỡng 7 ký tự ngẫu nhiên (số + chữ cái), đảm bảo không trùng
        /// </summary>
        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int codeLength = 7;
            var random = new Random();
            string code;
            
            // Tạo lại code cho đến khi không trùng
            do
            {
                code = new string(Enumerable.Repeat(chars, codeLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (await _maintenanceTicketRepository.CodeExistsAsync(code));
            
            return code;
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

        /// <summary>
        /// Lấy lịch sử hoạt động của Maintenance Ticket
        /// </summary>
        public async Task<List<BE.vn.fpt.edu.DTOs.HistoryLog.ResponseDto>> GetHistoryLogsAsync(long id)
        {
            var historyLogs = await _historyLogRepository.GetByMaintenanceTicketIdAsync(id);
            return historyLogs.Select(h => new BE.vn.fpt.edu.DTOs.HistoryLog.ResponseDto
            {
                Id = h.Id,
                UserId = h.UserId,
                UserName = h.User != null ? ($"{h.User.FirstName} {h.User.LastName}").Trim() : null,
                Action = h.Action,
                NewData = h.NewData,
                CreatedAt = h.CreatedAt
            }).ToList();
        }

        /// <summary>
        /// Áp dụng Service Package vào Maintenance Ticket - tự động thêm các Components từ package
        /// </summary>
        public async Task<ResponseDto> ApplyServicePackageAsync(long id, long servicePackageId)
        {
            // Validate MaintenanceTicket exists
            var maintenanceTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            if (maintenanceTicket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Chỉ cho phép áp dụng Service Package khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS
            if (maintenanceTicket.StatusCode != "PENDING" && maintenanceTicket.StatusCode != "ASSIGNED" && maintenanceTicket.StatusCode != "IN_PROGRESS")
                throw new ArgumentException($"Không thể áp dụng gói dịch vụ cho phiếu có trạng thái {maintenanceTicket.StatusCode}. Chỉ có thể áp dụng khi phiếu ở trạng thái PENDING, ASSIGNED hoặc IN_PROGRESS.");

            // Validate ServicePackage exists
            var servicePackage = await _servicePackageService.GetByIdAsync(servicePackageId);
            if (servicePackage == null)
                throw new ArgumentException("Service package not found");

            // Validate branch match
            if (servicePackage.BranchId != maintenanceTicket.BranchId)
                throw new ArgumentException("Service package không thuộc chi nhánh của phiếu bảo dưỡng");

            // Get existing components in ticket
            var existingComponents = await _ticketComponentService.GetByMaintenanceTicketIdAsync(id);
            var existingComponentIds = existingComponents.Select(c => c.ComponentId).ToHashSet();

            // Get components from ServicePackage
            if (servicePackage.Components == null || !servicePackage.Components.Any())
                throw new ArgumentException("Service package không có phụ tùng nào");

            // Add components from package to ticket
            var addedComponents = new List<string>();
            var skippedComponents = new List<string>();
            
            foreach (var packageComponent in servicePackage.Components)
            {
                // Skip if component already exists in ticket
                if (existingComponentIds.Contains(packageComponent.Id))
                {
                    skippedComponents.Add(packageComponent.Name ?? $"Component ID: {packageComponent.Id}");
                    continue;
                }

                try
                {
                    // Add component to ticket (quantity = 1, use component's unit price)
                    var ticketComponentDto = new BE.vn.fpt.edu.DTOs.TicketComponent.RequestDto
                    {
                        MaintenanceTicketId = id,
                        ComponentId = packageComponent.Id,
                        Quantity = 1, // Default quantity = 1
                        UnitPrice = packageComponent.UnitPrice
                    };

                    await _ticketComponentService.CreateAsync(ticketComponentDto);
                    addedComponents.Add(packageComponent.Name ?? $"Component ID: {packageComponent.Id}");
                }
                catch (Exception ex)
                {
                    // Log error but continue with other components
                    Console.WriteLine($"Error adding component {packageComponent.Id}: {ex.Message}");
                }
            }

            // Create history log
            var consulterName = maintenanceTicket.Consulter != null 
                ? ($"{maintenanceTicket.Consulter.FirstName} {maintenanceTicket.Consulter.LastName}").Trim() 
                : "Unknown";
            
            var logMessage = $"Áp dụng gói dịch vụ '{servicePackage.Name}' bởi {consulterName}. ";
            if (addedComponents.Any())
                logMessage += $"Đã thêm {addedComponents.Count} phụ tùng: {string.Join(", ", addedComponents)}. ";
            if (skippedComponents.Any())
                logMessage += $"Đã bỏ qua {skippedComponents.Count} phụ tùng (đã tồn tại): {string.Join(", ", skippedComponents)}.";

            await CreateHistoryLogAsync(
                userId: maintenanceTicket.ConsulterId,
                action: "APPLY_SERVICE_PACKAGE",
                maintenanceTicketId: id,
                oldData: null,
                newData: logMessage
            );

            // Update TotalEstimatedCost if ServicePackage has price
            if (servicePackage.Price.HasValue && servicePackage.Price.Value > 0)
            {
                var componentTotal = await _ticketComponentService.CalculateTotalCostAsync(id);
                // Update TotalEstimatedCost = component total + service package price (labor cost)
                maintenanceTicket.TotalEstimatedCost = componentTotal + servicePackage.Price.Value;
                await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
            }

            // Return updated ticket
            var updatedTicket = await _maintenanceTicketRepository.GetByIdAsync(id);
            return _mapper.Map<ResponseDto>(updatedTicket);
        }
    }
}


