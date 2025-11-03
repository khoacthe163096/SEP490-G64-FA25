namespace BE.vn.fpt.edu.DTOs.ServicePackage
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Code { get; set; }
        public string? BranchName { get; set; }
        public string? StatusName { get; set; }

        // list ID Component in service package
        public List<string>? ComponentNames { get; set; }
    }
}
