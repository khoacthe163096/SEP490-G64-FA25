using System;
using System.Collections.Generic;

namespace BE.models;

public partial class ServiceTask
{
    public long Id { get; set; }

    public long? MaintenanceTicketId { get; set; }

    public string? TaskName { get; set; }

    public string? Description { get; set; }

    public string? StatusCode { get; set; }

    public string? Note { get; set; }

    public virtual MaintenanceTicket? MaintenanceTicket { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }
}
