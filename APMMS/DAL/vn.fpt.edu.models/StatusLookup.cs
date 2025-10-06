using System;
using System.Collections.Generic;

namespace DAL.vn.fpt.edu.models;

public partial class StatusLookup
{
    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }
}
