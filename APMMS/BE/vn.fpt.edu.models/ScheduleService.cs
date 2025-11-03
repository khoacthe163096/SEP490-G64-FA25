using System;
using System.Collections.Generic;

namespace BE.models;

public partial class ScheduleService
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? CarId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string? StatusCode { get; set; }

    public long? BranchId { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual User? User { get; set; }
}
