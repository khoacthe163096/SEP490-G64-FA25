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
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public MaintenanceTicketService(
            IMaintenanceTicketRepository maintenanceTicketRepository,
            IVehicleCheckinRepository vehicleCheckinRepository,
            IMapper mapper,
            CarMaintenanceDbContext context)
        {
            _maintenanceTicketRepository = maintenanceTicketRepository;
            _vehicleCheckinRepository = vehicleCheckinRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResponseDto> CreateMaintenanceTicketAsync(RequestDto request)
        {
            var maintenanceTicket = _mapper.Map<MaintenanceTicket>(request);
            // Tự sinh code 7 ký tự ngẫu nhiên
            maintenanceTicket.Code = await GenerateUniqueCodeAsync();
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

            // Tạo Maintenance Ticket từ thông tin Check-in
            var maintenanceTicket = new MaintenanceTicket
            {
                VehicleCheckinId = request.VehicleCheckinId,
                CarId = vehicleCheckin.CarId,
                ConsulterId = request.ConsulterId,
                TechnicianId = request.TechnicianId ?? (request.TechnicianIds != null && request.TechnicianIds.Count > 0 ? request.TechnicianIds[0] : null), // Kỹ thuật viên chính là người đầu tiên
                BranchId = request.BranchId,
                ScheduleServiceId = request.ScheduleServiceId,
                StatusCode = request.StatusCode ?? "PENDING",
                PriorityLevel = "NORMAL",
                Description = request.Description,
                Code = await GenerateUniqueCodeAsync() // Tự sinh code 7 ký tự ngẫu nhiên
            };

            var createdTicket = await _maintenanceTicketRepository.CreateAsync(maintenanceTicket);

            // Lưu nhiều kỹ thuật viên vào bảng MaintenanceTicketTechnician
            if (request.TechnicianIds != null && request.TechnicianIds.Count > 0)
            {
                foreach (var technicianId in request.TechnicianIds.Distinct())
                {
                    _context.MaintenanceTicketTechnicians.Add(new MaintenanceTicketTechnician
                    {
                        MaintenanceTicketId = createdTicket.Id,
                        TechnicianId = technicianId,
                        AssignedDate = DateTime.UtcNow,
                        RoleInTicket = technicianId == maintenanceTicket.TechnicianId ? "PRIMARY" : "ASSISTANT"
                    });
                }
                await _context.SaveChangesAsync();
            }

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
                CreatedDate = fullTicket.StartTime, // dùng StartTime làm created hiển thị
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
                CarModel = fullTicket.Car != null ? fullTicket.Car.CarModel : null
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

            return _mapper.Map<ResponseDto>(maintenanceTicket);
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

            maintenanceTicket.StatusCode = "COMPLETED";
            maintenanceTicket.EndTime = DateTime.UtcNow;
            var updatedTicket = await _maintenanceTicketRepository.UpdateAsync(maintenanceTicket);
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
    }
}


