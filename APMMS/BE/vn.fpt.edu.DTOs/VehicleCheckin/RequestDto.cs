using System.ComponentModel.DataAnnotations;

namespace BE.vn.fpt.edu.DTOs.VehicleCheckin
{
    /// <summary>
    /// DTO cho việc tạo mới Vehicle Check-in
    /// </summary>
    public class VehicleCheckinRequestDto
    {
        [Required(ErrorMessage = "Car ID is required")]
        public long CarId { get; set; }
        
        /// <summary>
        /// Maintenance Request ID - có thể null nếu chưa tạo phiếu bảo dưỡng
        /// </summary>
        public long? MaintenanceRequestId { get; set; }
        
        [Required(ErrorMessage = "Mileage is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number")]
        public int Mileage { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "At least one image is required")]
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// DTO cho việc cập nhật Vehicle Check-in
    /// </summary>
    public class UpdateDto
    {
        [Required(ErrorMessage = "ID is required")]
        public long Id { get; set; }
        
        [Required(ErrorMessage = "Mileage is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number")]
        public int Mileage { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
        
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}


