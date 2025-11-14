namespace BE.vn.fpt.edu.DTOs.ServicePackage
{
    public class RequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Code { get; set; }
        public long? BranchId { get; set; }
        public string? StatusCode { get; set; }
    }
}
