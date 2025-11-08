using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class ServiceCategory
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? StatusCode { get; set; }

    public virtual ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();

    public virtual ICollection<ScheduleService> ScheduleServices { get; set; } = new List<ScheduleService>();

    public virtual StatusLookup? StatusCodeNavigation { get; set; }
}
