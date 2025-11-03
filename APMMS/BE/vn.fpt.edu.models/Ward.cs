using System;
using System.Collections.Generic;

namespace BE.models;

public partial class Ward
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? ProvinceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Province? Province { get; set; }
}
