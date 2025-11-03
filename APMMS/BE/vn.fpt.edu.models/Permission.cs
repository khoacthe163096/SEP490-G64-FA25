using System;
using System.Collections.Generic;

namespace BE.models;

public partial class Permission
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
