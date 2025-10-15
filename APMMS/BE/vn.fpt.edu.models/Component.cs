using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class Component
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? QuantityStock { get; set; }

    public long? TypeComponentId { get; set; }

    public long? BranchId { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<TicketComponent> TicketComponents { get; set; } = new List<TicketComponent>();

    public virtual TypeComponent? TypeComponent { get; set; }

    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
}
