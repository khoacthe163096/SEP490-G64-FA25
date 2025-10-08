using System;
using System.Collections.Generic;

namespace DAL.vn.fpt.edu.models;

public partial class Address
{
    public long Id { get; set; }

    public string? Street { get; set; }

    public string? PostalCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? ProvinceId { get; set; }

    public long? WardId { get; set; }

    public virtual Province? Province { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual Ward? Ward { get; set; }
}
