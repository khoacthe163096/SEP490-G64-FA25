using BLL.vn.fpt.edu.DTOs.VehicleCheckin;

namespace BLL.vn.fpt.edu.interfaces
{
    public interface IVehicleCheckinService
    {
        Task<ResponseDto> CreateVehicleCheckinAsync(RequestDto request);
        Task<ResponseDto> UpdateVehicleCheckinAsync(UpdateDto request);
        Task<ResponseDto> GetVehicleCheckinByIdAsync(long id);
        Task<List<ListResponseDto>> GetAllVehicleCheckinsAsync(int page = 1, int pageSize = 10);
        Task<List<ListResponseDto>> GetVehicleCheckinsByCarIdAsync(long carId);
        Task<List<ListResponseDto>> GetVehicleCheckinsByMaintenanceRequestIdAsync(long maintenanceRequestId);
        Task<bool> DeleteVehicleCheckinAsync(long id);
    }
}