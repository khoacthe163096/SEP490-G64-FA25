using System.Collections.Generic;

namespace BE.vn.fpt.edu.models
{
    public partial class TypeComponent
    {
        public TypeComponent()
        {
            Components = new HashSet<Component>();
        }

        public long Id { get; set; }
        public long? BranchId { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? StatusCode { get; set; }

        // Navigation
        public virtual Branch? Branch { get; set; }
        public virtual StatusLookup? StatusCodeNavigation { get; set; }
        public virtual ICollection<Component> Components { get; set; }
    }
}