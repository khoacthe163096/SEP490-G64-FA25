using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAL.vn.fpt.edu.models;

public partial class Branch
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? AddressId { get; set; }

    public string? Phone { get; set; }

    [JsonIgnore]
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();

    [JsonIgnore]
    public virtual ICollection<ScheduleService> ScheduleServices { get; set; } = new List<ScheduleService>();

    [JsonIgnore]
    public virtual ICollection<TotalReceipt> TotalReceipts { get; set; } = new List<TotalReceipt>();

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
