using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Hostel
{
    public int HostelId { get; set; }

    public string? Name { get; set; }

    public string? Location { get; set; }

    public int? TotalRooms { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<HostelUser> HostelUsers { get; set; } = new List<HostelUser>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
