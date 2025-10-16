using System.ComponentModel.DataAnnotations;

namespace BE.vn.fpt.edu.DTOs.ServiceTask
{
    /// <summary>
    /// DTO cho việc tạo mới ServiceTask
    /// </summary>
    public class ServiceTaskRequestDto
    {
        [Required(ErrorMessage = "Maintenance Ticket ID is required")]
        public long MaintenanceTicketId { get; set; }

        [Required(ErrorMessage = "Task name is required")]
        [StringLength(100, ErrorMessage = "Task name cannot exceed 100 characters")]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string? StatusCode { get; set; } = "PENDING";

        [StringLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO cho việc cập nhật ServiceTask
    /// </summary>
    public class ServiceTaskUpdateDto
    {
        [Required(ErrorMessage = "ID is required")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Maintenance Ticket ID is required")]
        public long MaintenanceTicketId { get; set; }

        [Required(ErrorMessage = "Task name is required")]
        [StringLength(100, ErrorMessage = "Task name cannot exceed 100 characters")]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string? StatusCode { get; set; }

        [StringLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO cho việc cập nhật status của ServiceTask
    /// </summary>
    public class ServiceTaskUpdateStatusDto
    {
        [Required(ErrorMessage = "Status code is required")]
        [StringLength(50, ErrorMessage = "Status code cannot exceed 50 characters")]
        public string StatusCode { get; set; } = string.Empty;
    }
}