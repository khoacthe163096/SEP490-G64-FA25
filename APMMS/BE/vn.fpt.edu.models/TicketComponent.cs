﻿using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.models;

public partial class TicketComponent
{
    public long Id { get; set; }

    public long? MaintenanceTicketId { get; set; }

    public long? ComponentId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public virtual Component? Component { get; set; }

    public virtual MaintenanceTicket? MaintenanceTicket { get; set; }
}
