using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class MaintenanceRequest
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? CarId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? StatusCode { get; set; }

    public long? BranchId { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Car? Car { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<VehicleCheckin> VehicleCheckins { get; set; } = new List<VehicleCheckin>();
}
