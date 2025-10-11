using AutoMapper;
using BLL.vn.fpt.edu.DTOs.MaintenanceTicket;
using BLL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.models;

namespace BLL.vn.fpt.edu.services
{
    public class MaintenanceTicketService : IMaintenanceTicketService
    {
        private readonly IMaintenanceTicketRepository _maintenanceTicketRepository;
        private readonly IVehicleCheckinRepository _vehicleCheckinRepository;
        private readonly IMapper _mapper;

        public MaintenanceTicketService(
            IMaintenanceTicketRepository maintenanceTicketRepository,
            IVehicleCheckinRepository vehicleCheckinRepository,
            IMapper mapper)
        {
            _maintenanceTicketRepository = maintenanceTicketRepository;
            _vehicleCheckinRepository = vehicleCheckinRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateMaintenanceTicketAsync(RequestDto request)
        {
            var maintenanceTicket = _mapper.Map<MaintenanceTicket>(request);
            var createdTicket = await _maintenanceTicketRepository.CreateAsync(maintenanceTicket);
            return _mapper.Map<ResponseDto>(createdTicket);
        }

        public async Task<ResponseDto> CreateFromVehicleCheckinAsync(CreateFromCheckinDto request)
        {
            // Lấy thông tin Vehicle Check-in
            var vehicleCheckin = await _vehicleCheckinRepository.GetByIdAsync(request.VehicleCheckinId);
            if (vehicleCheckin == null)
                throw new ArgumentException("Vehicle check-in not found");

            // Tạo Maintenance Ticket từ thông tin Check-in
            var maintenanceTicket = new MaintenanceTicket
            {
                CarId = vehicleCheckin.CarId,
                ConsulterId = request.ConsulterId,
                TechnicianId = request.TechnicianId,
                BranchId = request.BranchId,
                ScheduleServiceId = request.ScheduleServiceId,
                StatusCode = request.StatusCode ?? "PENDING"
            };

            var createdTicket = await _maintenanceTicketRepository.CreateAsync(maintenanceTicket);

            // Cập nhật Vehicle Check-in với MaintenanceTicketId
            vehicleCheckin.MaintenanceRequestId = createdTicket.Id;
            await _vehicleCheckinRepository.UpdateAsync(vehicleCheckin);

            // Lấy thông tin đầy đủ để trả về
            var fullTicket = await _maintenanceTicketRepository.GetByIdAsync(createdTicket.Id);
            var response = _mapper.Map<ResponseDto>(fullTicket);

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
    }
}


