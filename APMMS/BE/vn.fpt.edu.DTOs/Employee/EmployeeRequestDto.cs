namespace BE.vn.fpt.edu.DTOs.Employee
{
    public class EmployeeRequestDto
    {
        //public string? Code { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public string? Dob { get; set; } // Format: dd-MM-yyyy
        public string? CitizenId { get; set; }
        public string? Address { get; set; }
        public long? RoleId { get; set; }
        public long? BranchId { get; set; }
        public string? TaxCode { get; set; }
        public string? StatusCode { get; set; }
    }
    
    public class EmployeeUpdateStatusDto
    {
        public string StatusCode { get; set; } = null!;
    }
}


