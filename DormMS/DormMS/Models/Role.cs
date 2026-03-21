using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<HostelUser> HostelUsers { get; set; } = new List<HostelUser>();
}
