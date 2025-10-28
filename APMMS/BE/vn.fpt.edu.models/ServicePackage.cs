using System.Collections.Generic;

namespace BE.vn.fpt.edu.models
{
    public partial class ServicePackage
    {
        public ServicePackage()
        {
            Components = new HashSet<Component>();
        }

        public long Id { get; set; }
        public long? BranchId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? StatusCode { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual StatusLookup? StatusCodeNavigation { get; set; }
        public virtual ICollection<Component> Components { get; set; }
    }
}