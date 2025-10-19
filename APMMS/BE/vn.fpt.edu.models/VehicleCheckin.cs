using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.vn.fpt.edu.models;

public partial class VehicleCheckin
{
    public long Id { get; set; }

    [Column("car_id")]
    public long? CarId { get; set; }

    [Column("maintenance_request_id")]
    public long? MaintenanceRequestId { get; set; }

    public int? Mileage { get; set; }
    
    [Column("vin_number")]
    public string? VinNumber { get; set; }
    
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("status_code")]
    public string? StatusCode { get; set; }

    public virtual Car? Car { get; set; }

    public virtual MaintenanceRequest? MaintenanceRequest { get; set; }

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual ICollection<VehicleCheckinImage> VehicleCheckinImages { get; set; } = new List<VehicleCheckinImage>();
}
