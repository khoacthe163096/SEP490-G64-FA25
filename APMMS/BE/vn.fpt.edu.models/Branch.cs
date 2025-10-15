using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class Branch
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? AddressId { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();

    public virtual ICollection<ScheduleService> ScheduleServices { get; set; } = new List<ScheduleService>();

    public virtual ICollection<TotalReceipt> TotalReceipts { get; set; } = new List<TotalReceipt>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
