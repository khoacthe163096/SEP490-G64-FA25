using BE.vn.fpt.edu.DTOs.VehicleCheckin;

namespace BE.vn.fpt.edu.interfaces
{
    public interface IVehicleCheckinService
    {
        Task<ResponseDto> CreateVehicleCheckinAsync(VehicleCheckinRequestDto request);
        Task<ResponseDto> UpdateVehicleCheckinAsync(UpdateDto request);
        Task<ResponseDto> GetVehicleCheckinByIdAsync(long id);
        Task<List<ListResponseDto>> GetAllVehicleCheckinsAsync(int page = 1, int pageSize = 10);
        Task<int> GetTotalCountAsync();
        Task<List<ListResponseDto>> GetVehicleCheckinsByCarIdAsync(long carId);
        Task<List<ListResponseDto>> GetVehicleCheckinsByMaintenanceRequestIdAsync(long maintenanceRequestId);
        Task<ResponseDto> LinkMaintenanceRequestAsync(long vehicleCheckinId, long maintenanceRequestId);
        Task<bool> DeleteVehicleCheckinAsync(long id);
        Task<object> SearchVehicleAsync(string searchTerm);
        Task<ResponseDto> UpdateStatusAsync(long id, string statusCode);
    }
}
