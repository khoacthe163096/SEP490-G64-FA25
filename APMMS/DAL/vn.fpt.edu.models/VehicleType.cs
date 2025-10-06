﻿using System;
using System.Collections.Generic;

namespace DAL.vn.fpt.edu.models;

public partial class VehicleType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
