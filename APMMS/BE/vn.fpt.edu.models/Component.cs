using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.vn.fpt.edu.models;

public partial class Component
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? QuantityStock { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public long? TypeComponentId { get; set; }

    public long? BranchId { get; set; }

    public string? ImageUrl { get; set; }

    public string? StatusCode { get; set; }

    public decimal? PurchasePrice { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual ICollection<TicketComponent> TicketComponents { get; set; } = new List<TicketComponent>();

    public virtual TypeComponent? TypeComponent { get; set; }

    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
}
