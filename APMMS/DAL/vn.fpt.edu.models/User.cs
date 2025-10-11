using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAL.vn.fpt.edu.models;

public partial class User
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Gender { get; set; }

    public string? Image { get; set; }

    public string? TaxCode { get; set; }

    public string? StatusCode { get; set; }

    public bool? IsDelete { get; set; }

    public string? ResetKey { get; set; }

    public DateTime? ResetDate { get; set; }

    public long? RoleId { get; set; }

    public long? BranchId { get; set; }

    public long? AddressId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? LastModifiedBy { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual Address? Address { get; set; }

    public virtual Branch? Branch { get; set; }

    [JsonIgnore]
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [JsonIgnore]
    public virtual ICollection<HistoryLog> HistoryLogs { get; set; } = new List<HistoryLog>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceTicket> MaintenanceTicketConsulters { get; set; } = new List<MaintenanceTicket>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceTicket> MaintenanceTicketTechnicians { get; set; } = new List<MaintenanceTicket>();

    [JsonIgnore]
    public virtual ICollection<MaintenanceTicketTechnician> MaintenanceTicketTechniciansNavigation { get; set; } = new List<MaintenanceTicketTechnician>();

    public virtual Role? Role { get; set; }

    [JsonIgnore]
    public virtual ICollection<ScheduleService> ScheduleServices { get; set; } = new List<ScheduleService>();

    [JsonIgnore]
    public virtual ICollection<TotalReceipt> TotalReceipts { get; set; } = new List<TotalReceipt>();
}
