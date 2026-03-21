using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class HostelUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public string? Name { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Course { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public int? HostelId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Allotment> Allotments { get; set; } = new List<Allotment>();

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Hostel? Hostel { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
}
