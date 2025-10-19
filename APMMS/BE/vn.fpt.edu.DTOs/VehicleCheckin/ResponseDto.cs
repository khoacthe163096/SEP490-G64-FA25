using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.DTOs.VehicleCheckin
{
    /// <summary>
    /// DTO cho response chi tiết Vehicle Check-in
    /// </summary>
    public class ResponseDto
    {
        public long Id { get; set; }
        public long CarId { get; set; }
        public long MaintenanceRequestId { get; set; }
        public int Mileage { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        // Thông tin xe
        public string? CarName { get; set; }
        public string? CarModel { get; set; }
        public string? LicensePlate { get; set; }
        public string? VinNumber { get; set; }
        public string? VehicleEngineNumber { get; set; }
        public string? Color { get; set; }
        public int? YearOfManufacture { get; set; }
        
        // Thông tin khách hàng
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        
        // Thông tin chi nhánh
        public string? BranchName { get; set; }
        
        // Hình ảnh
        public List<VehicleCheckinImageDto> Images { get; set; } = new List<VehicleCheckinImageDto>();
        
        // Thông tin yêu cầu bảo dưỡng
        public string? MaintenanceRequestStatus { get; set; }
        public DateTime? RequestDate { get; set; }
        
        // Trạng thái VehicleCheckin
        public string? StatusCode { get; set; }
    }
    
    /// <summary>
    /// DTO cho hình ảnh Vehicle Check-in
    /// </summary>
    public class VehicleCheckinImageDto
    {
        public long Id { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO cho danh sách Vehicle Check-in (dạng tóm tắt)
    /// </summary>
    public class ListResponseDto
    {
        public long Id { get; set; }
        public long CarId { get; set; }
        public string? CarName { get; set; }
        public string? LicensePlate { get; set; }
        public string? VinNumber { get; set; }
        public string? CustomerName { get; set; }
        public int Mileage { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Notes { get; set; }
        public string? FirstImageUrl { get; set; }
        public string? BranchName { get; set; }
        public string? MaintenanceRequestStatus { get; set; }
        public string? StatusCode { get; set; }
    }
}


