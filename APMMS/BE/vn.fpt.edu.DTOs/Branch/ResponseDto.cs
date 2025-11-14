namespace BE.vn.fpt.edu.DTOs.Branch
{
    public class BranchResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? LaborRate { get; set; }
    }
}

