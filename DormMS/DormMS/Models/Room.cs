using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int? HostelId { get; set; }

    public string? RoomNumber { get; set; }

    public int? Capacity { get; set; }

    public int? Occupied { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Allotment> Allotments { get; set; } = new List<Allotment>();

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Hostel? Hostel { get; set; }
}
