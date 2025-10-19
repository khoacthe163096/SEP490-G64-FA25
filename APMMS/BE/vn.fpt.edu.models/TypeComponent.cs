using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class TypeComponent
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}
