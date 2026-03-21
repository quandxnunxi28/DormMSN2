using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? Date { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public virtual HostelUser? User { get; set; }
}
