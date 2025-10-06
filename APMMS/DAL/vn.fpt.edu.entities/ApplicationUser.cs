using Microsoft.AspNetCore.Identity;

namespace DAL.vn.fpt.edu.entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public long? RoleId { get; set; }
        // You can add more mapped fields later if needed (e.g., FirstName, LastName)
    }
}


