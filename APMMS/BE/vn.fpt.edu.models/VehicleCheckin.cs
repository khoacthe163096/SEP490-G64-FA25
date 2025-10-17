using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class VehicleCheckin
{
    public long Id { get; set; }

    public long? CarId { get; set; }

    public long? MaintenanceRequestId { get; set; }

    public int? Mileage { get; set; }
    public string? VinNumber { get; set; }
    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? StatusCode { get; set; }

    public virtual Car? Car { get; set; }

    public virtual MaintenanceRequest? MaintenanceRequest { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual ICollection<VehicleCheckinImage> VehicleCheckinImages { get; set; } = new List<VehicleCheckinImage>();
}
