namespace BE.vn.fpt.edu.DTOs.TypeComponent
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? BranchName { get; set; }
        public string? StatusName { get; set; }
    }
}