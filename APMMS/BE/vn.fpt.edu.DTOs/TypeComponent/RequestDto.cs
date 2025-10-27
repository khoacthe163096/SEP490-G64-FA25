namespace BE.vn.fpt.edu.DTOs.TypeComponent
{
    public class RequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long? BranchId { get; set; }
        public string? StatusCode { get; set; } // "ACTIVE", "INACTIVE"
    }
}