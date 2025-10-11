using System.ComponentModel.DataAnnotations;

namespace BLL.vn.fpt.edu.DTOs.MaintenanceTicket
{
    public class RequestDto
    {
        [Required(ErrorMessage = "Car ID is required")]
        public long CarId { get; set; }

        [Required(ErrorMessage = "Consulter ID is required")]
        public long ConsulterId { get; set; }

        public long? TechnicianId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        public long BranchId { get; set; }

        public long? ScheduleServiceId { get; set; }

        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string? StatusCode { get; set; } = "PENDING"; // Default status
    }

    /// <summary>
    /// DTO để tạo Maintenance Ticket từ Vehicle Check-in
    /// </summary>
    public class CreateFromCheckinDto
    {
        [Required(ErrorMessage = "Vehicle Check-in ID is required")]
        public long VehicleCheckinId { get; set; }

        [Required(ErrorMessage = "Consulter ID is required")]
        public long ConsulterId { get; set; }

        public long? TechnicianId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        public long BranchId { get; set; }

        public long? ScheduleServiceId { get; set; }

        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string? StatusCode { get; set; } = "PENDING";
    }

    /// <summary>
    /// DTO để cập nhật status của Maintenance Ticket
    /// </summary>
    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "Status code is required")]
        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string StatusCode { get; set; } = string.Empty;
    }
}


