namespace BE.vn.fpt.edu.DTOs.TypeComponent
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? BranchId { get; set; }
        public string? StatusCode { get; set; }
    }
}
