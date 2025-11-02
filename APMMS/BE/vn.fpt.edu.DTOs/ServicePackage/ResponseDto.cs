namespace BE.vn.fpt.edu.DTOs.ServicePackage
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public long? BranchId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? StatusCode { get; set; }

        // Summary of included components
        public List<ComponentSummary>? Components { get; set; }

        public class ComponentSummary
        {
            public long Id { get; set; }
            public string? Code { get; set; }
            public string? Name { get; set; }
            public decimal? UnitPrice { get; set; }
        }
    }
}


