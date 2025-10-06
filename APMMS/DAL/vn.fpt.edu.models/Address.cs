using System;
using System.Collections.Generic;

namespace DAL.vn.fpt.edu.models;

public partial class Address
{
    public long Id { get; set; }

    public string? Country { get; set; }

    public string? Province { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? Street { get; set; }

    public string? PostalCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
