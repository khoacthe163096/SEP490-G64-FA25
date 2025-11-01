using AutoMapper;
using BE.vn.fpt.edu.DTOs.MaintenanceTicket;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using System.Linq;

namespace BE.vn.fpt.edu.services
{
    public class MaintenanceTicketService : IMaintenanceTicketService
    {
        private readonly IMaintenanceTicketRepository _maintenanceTicketRepository;
        private readonly IVehicleCheckinRepository _vehicleCheckinRepository;
        private readonly IHistoryLogRepository _historyLogRepository;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public MaintenanceTicketService(
            IMaintenanceTicketRepository maintenanceTicketRepository,
            IVehicleCheckinRepository vehicleCheckinRepository,
            IHistoryLogRepository historyLogRepository,
            IMapper mapper,
            CarMaintenanceDbContext context)
        {
            _maintenanceTicketRepository = maintenanceTicketRepository;
            _vehicleCheckinRepository = vehicleCheckinRepository;
            _historyLogRepository = historyLogRepository;
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
                CustomerAddress = fullTicket.Car != null && fullTicket.Car.User != null && fullTicket.Car.User.Address != null
                    ? string.Join(", ", new [] {
                        fullTicket.Car.User.Address.Street,
                        fullTicket.Car.User.Address.Ward != null ? fullTicket.Car.User.Address.Ward.Name : null,
                        fullTicket.Car.User.Address.Province != null ? fullTicket.Car.User.Address.Province.Name : null
                    }.Where(s => !string.IsNullOrWhiteSpace(s)))
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

            _mapper.Map(request, existingTicket);
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(existingTicket);
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

            maintenanceTicket.StatusCode = statusCode;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
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

            // ✅ VALIDATION: Chỉ cho phép gán kỹ thuật viên khi phiếu ở trạng thái PENDING
            if (maintenanceTicket.StatusCode != "PENDING")
                throw new ArgumentException($"Không thể gán kỹ thuật viên cho phiếu có trạng thái {maintenanceTicket.StatusCode}. Chỉ có thể gán khi phiếu ở trạng thái PENDING.");

            if (technicianIds == null || technicianIds.Count == 0)
                throw new ArgumentException("Phải chọn ít nhất một kỹ thuật viên");
            
            // ✅ VALIDATION: Đảm bảo phiếu chưa có kỹ thuật viên được gán
            if (maintenanceTicket.TechnicianId.HasValue || maintenanceTicket.MaintenanceTicketTechnicians != null && maintenanceTicket.MaintenanceTicketTechnicians.Count > 0)
                throw new ArgumentException("Phiếu này đã có kỹ thuật viên được gán. Không thể gán lại.");

            // Lưu trạng thái cũ để ghi log
            var oldStatus = maintenanceTicket.StatusCode;

            // Thêm các bản ghi mới vào bảng trung gian nếu chưa tồn tại
            var existing = maintenanceTicket.MaintenanceTicketTechnicians.Select(x => x.TechnicianId).ToHashSet();
            foreach (var techId in technicianIds.Distinct())
            {
                if (!existing.Contains(techId))
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

            // ✅ QUAN TRỌNG: Chuyển trạng thái từ PENDING sang IN_PROGRESS khi gán kỹ thuật viên
            maintenanceTicket.StatusCode = "IN_PROGRESS";
            _context.MaintenanceTickets.Update(maintenanceTicket);
            await _context.SaveChangesAsync();

            // ✅ Tạo history log để ghi nhận việc gán kỹ thuật viên
            var techNames = string.Join(", ", technicianIds.Select(techId => $"Technician ID: {techId}"));
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.ConsulterId,
                action: "ASSIGN_TECHNICIAN",
                oldData: $"Status: {oldStatus}, Technicians: None",
                newData: $"Status: IN_PROGRESS, Technicians: {techNames}, Primary: {maintenanceTicket.TechnicianId}"
            );

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

            maintenanceTicket.StatusCode = "IN_PROGRESS";
            maintenanceTicket.StartTime = DateTime.UtcNow;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
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
            await CreateHistoryLogAsync(
                userId: maintenanceTicket.TechnicianId, // Technician là người hoàn thành
                action: "COMPLETE_MAINTENANCE",
                oldData: $"Status: {oldStatus}",
                newData: $"Status: COMPLETED, EndTime: {DateTime.UtcNow}"
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
        private async Task CreateHistoryLogAsync(long? userId, string action, string? oldData = null, string? newData = null)
        {
            var historyLog = new HistoryLog
            {
                UserId = userId,
                Action = action,
                OldData = oldData,
                NewData = newData,
                CreatedAt = DateTime.UtcNow
            };
            await _historyLogRepository.CreateAsync(historyLog);
        }
    }
}


