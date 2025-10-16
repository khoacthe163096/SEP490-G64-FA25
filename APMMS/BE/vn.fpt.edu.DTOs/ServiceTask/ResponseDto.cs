namespace BE.vn.fpt.edu.DTOs.ServiceTask
{
    /// <summary>
    /// DTO cho response chi tiết ServiceTask
    /// </summary>
    public class ServiceTaskResponseDto
    {
        public long Id { get; set; }
        public long MaintenanceTicketId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? StatusCode { get; set; }
        public string? Note { get; set; }

        // Navigation properties
        public string? MaintenanceTicketDescription { get; set; }
        public string? CarName { get; set; }
        public string? CustomerName { get; set; }
        public string? TechnicianName { get; set; }
        public string? BranchName { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách ServiceTask (dạng tóm tắt)
    /// </summary>
    public class ServiceTaskListResponseDto
    {
        public long Id { get; set; }
        public long MaintenanceTicketId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? StatusCode { get; set; }
        public string? Note { get; set; }

        // Basic info
        public string? CarName { get; set; }
        public string? CustomerName { get; set; }
        public string? TechnicianName { get; set; }
        public string? BranchName { get; set; }
    }
}