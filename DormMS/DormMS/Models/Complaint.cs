using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public string? Issue { get; set; }

    public DateOnly? DateFiled { get; set; }

    public string? Status { get; set; }

    public virtual Room? Room { get; set; }

    public virtual HostelUser? User { get; set; }
}
