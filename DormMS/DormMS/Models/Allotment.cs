using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Allotment
{
    public int AllotmentId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public DateOnly? AllotDate { get; set; }

    public DateOnly? LeaveDate { get; set; }

    public string? Status { get; set; }

    public virtual Room? Room { get; set; }

    public virtual HostelUser? User { get; set; }
}
