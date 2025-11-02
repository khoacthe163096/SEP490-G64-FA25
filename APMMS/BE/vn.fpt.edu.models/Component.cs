using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models
{
    public partial class Component
    {
        public Component()
        {
            TicketComponents = new HashSet<TicketComponent>();
            ServicePackages = new HashSet<ServicePackage>();
        }

        public long Id { get; set; }
        public long? BranchId { get; set; }
        public string? Code { get; set; }
        public string? ImageUrl { get; set; }
        public string? Name { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int? QuantityStock { get; set; }
        public string? StatusCode { get; set; }
        public long? TypeComponentId { get; set; }
        public decimal? UnitPrice { get; set; }

        // Navigation
        public virtual Branch? Branch { get; set; }
        public virtual StatusLookup? StatusCodeNavigation { get; set; }
        public virtual TypeComponent? TypeComponent { get; set; }

        // Many-to-many with ServicePackage is represented in DbContext using a join table; keep collection
        public virtual ICollection<ServicePackage> ServicePackages { get; set; }

        public virtual ICollection<TicketComponent> TicketComponents { get; set; }
    }
}
