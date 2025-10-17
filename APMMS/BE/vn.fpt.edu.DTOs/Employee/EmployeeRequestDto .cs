namespace BE.vn.fpt.edu.DTOs.Employee
{
    public class EmployeeRequestDto
    {
        public string? Code { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public long? RoleId { get; set; }
        public long? BranchId { get; set; }
    }
}
