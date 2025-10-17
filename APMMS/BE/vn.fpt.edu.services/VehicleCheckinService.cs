using BE.vn.fpt.edu.DTOs.VehicleCheckin;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class VehicleCheckinService : IVehicleCheckinService
    {
        private readonly IVehicleCheckinRepository _vehicleCheckinRepository;

        public VehicleCheckinService(IVehicleCheckinRepository vehicleCheckinRepository)
        {
            _vehicleCheckinRepository = vehicleCheckinRepository;
        }

        public async Task<ResponseDto> CreateVehicleCheckinAsync(VehicleCheckinRequestDto request)
        {
            var vehicleCheckin = new VehicleCheckin
            {
                CarId = request.CarId,
                MaintenanceRequestId = request.MaintenanceRequestId,
                Mileage = request.Mileage,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                VehicleCheckinImages = request.ImageUrls.Select(url => new VehicleCheckinImage
                {
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            var createdVehicleCheckin = await _vehicleCheckinRepository.CreateAsync(vehicleCheckin);
            return await GetVehicleCheckinByIdAsync(createdVehicleCheckin.Id);
        }

        public async Task<ResponseDto> UpdateVehicleCheckinAsync(UpdateDto request)
        {
            var existingVehicleCheckin = await _vehicleCheckinRepository.GetByIdAsync(request.Id);
            if (existingVehicleCheckin == null)
                throw new ArgumentException("Vehicle check-in not found");

            existingVehicleCheckin.Mileage = request.Mileage;
            existingVehicleCheckin.Notes = request.Notes;

            // Update images if provided
            if (request.ImageUrls.Any())
            {
                // Remove existing images
                existingVehicleCheckin.VehicleCheckinImages.Clear();
                
                // Add new images
                existingVehicleCheckin.VehicleCheckinImages = request.ImageUrls.Select(url => new VehicleCheckinImage
                {
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _vehicleCheckinRepository.UpdateAsync(existingVehicleCheckin);
            return await GetVehicleCheckinByIdAsync(existingVehicleCheckin.Id);
        }

        public async Task<ResponseDto> GetVehicleCheckinByIdAsync(long id)
        {
            var vehicleCheckin = await _vehicleCheckinRepository.GetByIdWithDetailsAsync(id);
            if (vehicleCheckin == null)
                throw new ArgumentException("Vehicle check-in not found");

            return MapToResponseDTO(vehicleCheckin);
        }

        public async Task<List<ListResponseDto>> GetAllVehicleCheckinsAsync(int page = 1, int pageSize = 10)
        {
            var vehicleCheckins = await _vehicleCheckinRepository.GetAllWithDetailsAsync(page, pageSize);
            return vehicleCheckins.Select(MapToListResponseDTO).ToList();
        }

        public async Task<List<ListResponseDto>> GetVehicleCheckinsByCarIdAsync(long carId)
        {
            var vehicleCheckins = await _vehicleCheckinRepository.GetByCarIdAsync(carId);
            return vehicleCheckins.Select(MapToListResponseDTO).ToList();
        }

        public async Task<List<ListResponseDto>> GetVehicleCheckinsByMaintenanceRequestIdAsync(long maintenanceRequestId)
        {
            var vehicleCheckins = await _vehicleCheckinRepository.GetByMaintenanceRequestIdAsync(maintenanceRequestId);
            return vehicleCheckins.Select(MapToListResponseDTO).ToList();
        }

        public async Task<bool> DeleteVehicleCheckinAsync(long id)
        {
            return await _vehicleCheckinRepository.DeleteAsync(id);
        }

        private ResponseDto MapToResponseDTO(VehicleCheckin vehicleCheckin)
        {
            return new ResponseDto
            {
                Id = vehicleCheckin.Id,
                CarId = vehicleCheckin.CarId ?? 0,
                MaintenanceRequestId = vehicleCheckin.MaintenanceRequestId ?? 0,
                Mileage = vehicleCheckin.Mileage ?? 0,
                Notes = vehicleCheckin.Notes,
                CreatedAt = vehicleCheckin.CreatedAt,
                
                // Car information
                CarName = vehicleCheckin.Car?.CarName,
                CarModel = vehicleCheckin.Car?.CarModel,
                LicensePlate = vehicleCheckin.Car?.LicensePlate,
                VinNumber = vehicleCheckin.Car?.VinNumber,
                Color = vehicleCheckin.Car?.Color,
                YearOfManufacture = vehicleCheckin.Car?.YearOfManufacture,
                
                // Customer information
                CustomerName = $"{vehicleCheckin.Car?.User?.FirstName} {vehicleCheckin.Car?.User?.LastName}".Trim(),
                CustomerPhone = vehicleCheckin.Car?.User?.Phone,
                CustomerEmail = vehicleCheckin.Car?.User?.Email,
                
                // Images
                Images = vehicleCheckin.VehicleCheckinImages?.Select(img => new VehicleCheckinImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    CreatedAt = img.CreatedAt
                }).ToList() ?? new List<VehicleCheckinImageDto>(),
                
                // Maintenance request info
                MaintenanceRequestStatus = vehicleCheckin.MaintenanceRequest?.StatusCode,
                RequestDate = vehicleCheckin.MaintenanceRequest?.RequestDate
            };
        }

        public async Task<ResponseDto> LinkMaintenanceRequestAsync(long vehicleCheckinId, long maintenanceRequestId)
        {
            var vehicleCheckin = await _vehicleCheckinRepository.GetByIdAsync(vehicleCheckinId);
            if (vehicleCheckin == null)
                throw new ArgumentException("Vehicle check-in not found");

            vehicleCheckin.MaintenanceRequestId = maintenanceRequestId;
            await _vehicleCheckinRepository.UpdateAsync(vehicleCheckin);
            
            return await GetVehicleCheckinByIdAsync(vehicleCheckinId);
        }

        public async Task<object> SearchVehicleAsync(string searchTerm)
        {
            // Tìm kiếm xe theo biển số hoặc số khung
            var cars = await _vehicleCheckinRepository.SearchCarsAsync(searchTerm);
            
            if (!cars.Any())
            {
                return new { found = false, message = "Không tìm thấy xe trong hệ thống" };
            }

            // Nếu tìm thấy xe, trả về thông tin xe
            var car = cars.First();
            return new 
            { 
                found = true,
                car = new
                {
                    id = car.Id,
                    carName = car.CarName,
                    carModel = car.CarModel,
                    licensePlate = car.LicensePlate,
                    vinNumber = car.VinNumber,
                    color = car.Color,
                    yearOfManufacture = car.YearOfManufacture,
                    vehicleEngineNumber = car.VehicleEngineNumber,
                    customerName = $"{car.User?.FirstName} {car.User?.LastName}".Trim(),
                    customerPhone = car.User?.Phone,
                    customerEmail = car.User?.Email,
                    branchName = car.Branch?.Name
                }
            };
        }

        private ListResponseDto MapToListResponseDTO(VehicleCheckin vehicleCheckin)
        {
            return new ListResponseDto
            {
                Id = vehicleCheckin.Id,
                CarId = vehicleCheckin.CarId ?? 0,
                CarName = vehicleCheckin.Car?.CarName,
                LicensePlate = vehicleCheckin.Car?.LicensePlate,
                VinNumber = vehicleCheckin.Car?.VinNumber,
                CustomerName = $"{vehicleCheckin.Car?.User?.FirstName} {vehicleCheckin.Car?.User?.LastName}".Trim(),
                Mileage = vehicleCheckin.Mileage ?? 0,
                CreatedAt = vehicleCheckin.CreatedAt,
                Notes = vehicleCheckin.Notes,
                FirstImageUrl = vehicleCheckin.VehicleCheckinImages?.FirstOrDefault()?.ImageUrl
            };
        }
    }
}
