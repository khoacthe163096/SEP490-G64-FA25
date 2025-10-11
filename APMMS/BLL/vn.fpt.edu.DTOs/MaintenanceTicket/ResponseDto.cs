namespace BLL.vn.fpt.edu.DTOs.MaintenanceTicket
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public long? ScheduleServiceId { get; set; }
        public long? CarId { get; set; }
        public long? ConsulterId { get; set; }
        public long? TechnicianId { get; set; }
        public string? StatusCode { get; set; }
        public long? BranchId { get; set; }
        
        // Navigation properties
        public string? CarName { get; set; }
        public string? ConsulterName { get; set; }
        public string? TechnicianName { get; set; }
        public string? BranchName { get; set; }
        public string? ScheduleServiceName { get; set; }
        
        // Vehicle Check-in info (nếu được tạo từ check-in)
        public long? VehicleCheckinId { get; set; }
        public int? Mileage { get; set; }
        public string? CheckinNotes { get; set; }
        public List<string>? CheckinImages { get; set; }
    }

    public class ListResponseDto
    {
        public long Id { get; set; }
        public long? CarId { get; set; }
        public long? ConsulterId { get; set; }
        public long? TechnicianId { get; set; }
        public string? StatusCode { get; set; }
        public long? BranchId { get; set; }
        
        // Basic info
        public string? CarName { get; set; }
        public string? ConsulterName { get; set; }
        public string? TechnicianName { get; set; }
        public string? BranchName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}


