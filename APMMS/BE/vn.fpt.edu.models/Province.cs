using System;
using System.Collections.Generic;

namespace BE.models;

public partial class Province
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
