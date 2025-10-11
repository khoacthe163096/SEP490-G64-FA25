using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAL.vn.fpt.edu.models;

public partial class VehicleCheckin
{
    public long Id { get; set; }

    public long? CarId { get; set; }

    public long? MaintenanceRequestId { get; set; }

    public int? Mileage { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Car? Car { get; set; }

    public virtual MaintenanceRequest? MaintenanceRequest { get; set; }

    [JsonIgnore]
    public virtual ICollection<VehicleCheckinImage> VehicleCheckinImages { get; set; } = new List<VehicleCheckinImage>();
}
