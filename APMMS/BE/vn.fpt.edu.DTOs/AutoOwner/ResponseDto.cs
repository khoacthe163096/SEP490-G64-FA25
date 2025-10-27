namespace BE.vn.fpt.edu.DTOs.AutoOwner
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string Username { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public string? TaxCode { get; set; }
        public string? StatusCode { get; set; }
        public long? RoleId { get; set; }
        public long? BranchId { get; set; }
        public long? AddressId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
