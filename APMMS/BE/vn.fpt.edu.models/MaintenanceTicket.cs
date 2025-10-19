using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class MaintenanceTicket
{
    public long Id { get; set; }

    public long? ScheduleServiceId { get; set; }

    public long? CarId { get; set; }

    public long? ConsulterId { get; set; }

    public long? TechnicianId { get; set; }

    public string? StatusCode { get; set; }

    public long? BranchId { get; set; }

    public string? Description { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Car? Car { get; set; }

    public virtual User? Consulter { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<MaintenanceTicketTechnician> MaintenanceTicketTechnicians { get; set; } = new List<MaintenanceTicketTechnician>();

    public virtual ScheduleService? ScheduleService { get; set; }

    public virtual ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();

    public virtual StatusLookup? StatusCodeNavigation { get; set; }

    public virtual User? Technician { get; set; }

    public virtual ICollection<TicketComponent> TicketComponents { get; set; } = new List<TicketComponent>();

    public virtual ICollection<TotalReceipt> TotalReceipts { get; set; } = new List<TotalReceipt>();
}
