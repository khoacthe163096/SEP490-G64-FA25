namespace BE.vn.fpt.edu.DTOs.ServicePackage
{
    public class RequestDto
    {
        public long? Id { get; set; } // null -> create
        public long? BranchId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? StatusCode { get; set; }

        // Components to include in package
        public List<long>? ComponentIds { get; set; }
    }
}


