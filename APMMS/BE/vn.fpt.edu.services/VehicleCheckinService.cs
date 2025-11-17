using BE.vn.fpt.edu.DTOs.VehicleCheckin;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class VehicleCheckinService : IVehicleCheckinService
    {
        private readonly IVehicleCheckinRepository _vehicleCheckinRepository;
        private readonly CarMaintenanceDbContext _context;

        public VehicleCheckinService(IVehicleCheckinRepository vehicleCheckinRepository, CarMaintenanceDbContext context)
        {
            _vehicleCheckinRepository = vehicleCheckinRepository;
            _context = context;
        }

        public async Task<ResponseDto> CreateVehicleCheckinAsync(VehicleCheckinRequestDto request)
        {
            var car = await _context.Cars
                .Include(c => c.User)
                .Include(c => c.Branch)
                .Include(c => c.VehicleType)
                .FirstOrDefaultAsync(c => c.Id == request.CarId);

            if (car == null)
            {
                throw new ArgumentException("Car not found");
            }

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == request.BranchId);

            var vehicleCheckin = new VehicleCheckin
            {
                CarId = request.CarId,
                MaintenanceRequestId = request.MaintenanceRequestId,
                Mileage = request.Mileage,
                Notes = request.Notes,
                BranchId = request.BranchId,
                Code = await GenerateVehicleCheckinCodeAsync(),
                StatusCode = "PENDING", // Default status - chỉ có PENDING và CONFIRMED
                CreatedAt = DateTime.UtcNow,
                VehicleCheckinImages = request.ImageUrls.Select((url, index) => new VehicleCheckinImage
                {
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.ImageDescriptions != null && index < request.ImageDescriptions.Count 
                        ? request.ImageDescriptions[index] 
                        : null,
                    Category = request.ImageCategories != null && index < request.ImageCategories.Count 
                        ? request.ImageCategories[index] 
                        : null
                }).ToList(),
                SnapshotCarName = car.CarName,
                SnapshotCarModel = car.CarModel,
                SnapshotVehicleType = car.VehicleType?.Name,
                SnapshotVehicleTypeId = car.VehicleTypeId,
                SnapshotLicensePlate = car.LicensePlate,
                SnapshotVinNumber = car.VinNumber,
                SnapshotEngineNumber = car.VehicleEngineNumber,
                SnapshotYearOfManufacture = car.YearOfManufacture,
                SnapshotColor = car.Color,
                SnapshotMileage = request.Mileage,
                SnapshotCustomerName = $"{car.User?.FirstName} {car.User?.LastName}".Trim(),
                SnapshotCustomerPhone = car.User?.Phone,
                SnapshotCustomerEmail = car.User?.Email,
                SnapshotCustomerAddress = car.User?.Address,
                SnapshotBranchName = branch?.Name ?? car.Branch?.Name,
                SnapshotConsulterName = null // sẽ bổ sung nếu có thông tin consulter trong request
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
            existingVehicleCheckin.SnapshotMileage = request.Mileage;

            // Update images if provided
            if (request.ImageUrls.Any())
            {
                // Remove existing images
                existingVehicleCheckin.VehicleCheckinImages.Clear();
                
                // Add new images
                existingVehicleCheckin.VehicleCheckinImages = request.ImageUrls.Select((url, index) => new VehicleCheckinImage
                {
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.ImageDescriptions != null && index < request.ImageDescriptions.Count 
                        ? request.ImageDescriptions[index] 
                        : null,
                    Category = request.ImageCategories != null && index < request.ImageCategories.Count 
                        ? request.ImageCategories[index] 
                        : null
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

            return await MapToResponseDTO(vehicleCheckin);
        }

        public async Task<List<ListResponseDto>> GetAllVehicleCheckinsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? statusCode = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var vehicleCheckins = await _vehicleCheckinRepository.GetAllWithDetailsAsync(page, pageSize, searchTerm, statusCode, fromDate, toDate);
            return vehicleCheckins.Select(MapToListResponseDTO).ToList();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm = null, string? statusCode = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await _vehicleCheckinRepository.GetTotalCountAsync(searchTerm, statusCode, fromDate, toDate);
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

        private async Task<ResponseDto> MapToResponseDTO(VehicleCheckin vehicleCheckin)
        {
            // Debug log
            Console.WriteLine($"Debug - VehicleCheckin ID: {vehicleCheckin.Id}");
            Console.WriteLine($"Debug - BranchId: {vehicleCheckin.BranchId}");
            Console.WriteLine($"Debug - Branch: {vehicleCheckin.Branch?.Name}");
            Console.WriteLine($"Debug - Car.BranchId: {vehicleCheckin.Car?.BranchId}");
            Console.WriteLine($"Debug - Car.Branch: {vehicleCheckin.Car?.Branch?.Name}");
            
            // Get branch name as fallback
            var branchId = vehicleCheckin.BranchId ?? vehicleCheckin.Car?.BranchId;
            var branchName = vehicleCheckin.Branch?.Name ?? vehicleCheckin.Car?.Branch?.Name;
            if (string.IsNullOrEmpty(branchName) && branchId.HasValue)
            {
                branchName = await GetBranchNameById(branchId);
            }
            
            return new ResponseDto
            {
                Id = vehicleCheckin.Id,
                CarId = vehicleCheckin.CarId ?? 0,
                MaintenanceRequestId = vehicleCheckin.MaintenanceRequestId ?? 0,
                Mileage = vehicleCheckin.Mileage ?? 0,
                Notes = vehicleCheckin.Notes,
                CreatedAt = vehicleCheckin.CreatedAt,
                Code = vehicleCheckin.Code,
                
                // Car information (snapshot preferred)
                CarName = vehicleCheckin.SnapshotCarName ?? vehicleCheckin.Car?.CarName,
                CarModel = vehicleCheckin.SnapshotCarModel ?? vehicleCheckin.Car?.CarModel,
                LicensePlate = vehicleCheckin.SnapshotLicensePlate ?? vehicleCheckin.Car?.LicensePlate,
                VinNumber = vehicleCheckin.SnapshotVinNumber ?? vehicleCheckin.Car?.VinNumber,
                VehicleEngineNumber = vehicleCheckin.SnapshotEngineNumber ?? vehicleCheckin.Car?.VehicleEngineNumber,
                Color = vehicleCheckin.SnapshotColor ?? vehicleCheckin.Car?.Color,
                YearOfManufacture = vehicleCheckin.SnapshotYearOfManufacture ?? vehicleCheckin.Car?.YearOfManufacture,
                VehicleType = vehicleCheckin.SnapshotVehicleType ?? vehicleCheckin.Car?.VehicleType?.Name,
                VehicleTypeId = vehicleCheckin.SnapshotVehicleTypeId ?? vehicleCheckin.Car?.VehicleTypeId,
                
                // Customer information
                CustomerName = vehicleCheckin.SnapshotCustomerName ?? $"{vehicleCheckin.Car?.User?.FirstName} {vehicleCheckin.Car?.User?.LastName}".Trim(),
                CustomerPhone = vehicleCheckin.SnapshotCustomerPhone ?? vehicleCheckin.Car?.User?.Phone,
                CustomerEmail = vehicleCheckin.SnapshotCustomerEmail ?? vehicleCheckin.Car?.User?.Email,
                CustomerAddress = vehicleCheckin.SnapshotCustomerAddress ?? vehicleCheckin.Car?.User?.Address,
                
                // Branch information
                BranchId = branchId,
                BranchName = vehicleCheckin.SnapshotBranchName ?? branchName,
                
                // Images
                Images = vehicleCheckin.VehicleCheckinImages?.Select(img => new VehicleCheckinImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    CreatedAt = img.CreatedAt,
                    Description = img.Description,
                    Category = img.Category
                }).ToList() ?? new List<VehicleCheckinImageDto>(),
                
                // Maintenance request info
                MaintenanceRequestStatus = vehicleCheckin.MaintenanceRequest?.StatusCode,
                RequestDate = vehicleCheckin.MaintenanceRequest?.RequestDate,
                
                // VehicleCheckin status
                StatusCode = vehicleCheckin.StatusCode
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
                Code = vehicleCheckin.Code,
                CarName = vehicleCheckin.SnapshotCarName ?? vehicleCheckin.Car?.CarName,
                LicensePlate = vehicleCheckin.SnapshotLicensePlate ?? vehicleCheckin.Car?.LicensePlate,
                VinNumber = vehicleCheckin.SnapshotVinNumber ?? vehicleCheckin.Car?.VinNumber,
                CustomerName = vehicleCheckin.SnapshotCustomerName ?? $"{vehicleCheckin.Car?.User?.FirstName} {vehicleCheckin.Car?.User?.LastName}".Trim(),
                VehicleType = vehicleCheckin.SnapshotVehicleType ?? vehicleCheckin.Car?.VehicleType?.Name,
                Mileage = vehicleCheckin.Mileage ?? 0,
                CreatedAt = vehicleCheckin.CreatedAt,
                Notes = vehicleCheckin.Notes,
                FirstImageUrl = vehicleCheckin.VehicleCheckinImages?.FirstOrDefault()?.ImageUrl,
                BranchId = vehicleCheckin.BranchId ?? vehicleCheckin.Car?.BranchId,
                BranchName = vehicleCheckin.SnapshotBranchName ?? vehicleCheckin.Branch?.Name ?? vehicleCheckin.Car?.Branch?.Name,
                MaintenanceRequestStatus = vehicleCheckin.MaintenanceRequest?.StatusCode,
                StatusCode = vehicleCheckin.StatusCode
            };
        }

        private async Task<string?> GetBranchNameById(long? branchId)
        {
            if (!branchId.HasValue) return null;
            
            try
            {
                var branch = await _context.Branches.FindAsync(branchId.Value);
                return branch?.Name;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> GenerateVehicleCheckinCodeAsync()
        {
            string code;
            bool isUnique;
            
            do
            {
                // Tạo code theo format VCI-xxxxx (5 số)
                var random = new Random();
                var number = random.Next(10000, 99999); // 5 số từ 10000-99999
                code = $"VCI-{number}";
                
                // Kiểm tra code có trùng không
                isUnique = !await _context.VehicleCheckins.AnyAsync(vc => vc.Code == code);
            } while (!isUnique);
            
            return code;
        }

        public async Task<ResponseDto> UpdateStatusAsync(long id, string statusCode)
        {
            var vehicleCheckin = await _context.VehicleCheckins
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.User)
                .Include(vc => vc.Car)
                    .ThenInclude(c => c.Branch)
                .Include(vc => vc.Branch)
                .Include(vc => vc.MaintenanceRequest)
                .Include(vc => vc.VehicleCheckinImages)
                .FirstOrDefaultAsync(vc => vc.Id == id);

            if (vehicleCheckin == null)
            {
                throw new ArgumentException("Vehicle check-in not found");
            }

            // Validate status
            if (statusCode != "PENDING" && statusCode != "CONFIRMED")
            {
                throw new ArgumentException("Trạng thái chỉ được phép là PENDING hoặc CONFIRMED");
            }

            vehicleCheckin.StatusCode = statusCode;
            await _context.SaveChangesAsync();

            return await MapToResponseDTO(vehicleCheckin);
        }
    }
}
