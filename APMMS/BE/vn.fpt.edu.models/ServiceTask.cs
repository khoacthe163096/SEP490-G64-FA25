using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class ServiceTask
{
    public long Id { get; set; }

    public long? MaintenanceTicketId { get; set; }

    public string? TaskName { get; set; }

    public string? Description { get; set; }

    public string? StatusCode { get; set; }

    public string? Note { get; set; }

    public long? ServiceCategoryId { get; set; }

    public decimal? StandardLaborTime { get; set; }

    public decimal? ActualLaborTime { get; set; }

    public decimal? LaborCost { get; set; }

    public virtual MaintenanceTicket? MaintenanceTicket { get; set; }

    public virtual ServiceCategory? ServiceCategory { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }
}
