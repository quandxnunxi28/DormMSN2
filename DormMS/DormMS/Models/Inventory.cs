using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Inventory
{
    public int ItemId { get; set; }

    public int? HostelId { get; set; }

    public string? ItemName { get; set; }

    public int? Quantity { get; set; }

    public string? Condition { get; set; }

    public virtual Hostel? Hostel { get; set; }
}
