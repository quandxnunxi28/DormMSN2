using System;
using System.Collections.Generic;

namespace DormMS.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public string? Summary { get; set; }

    public bool? IsImportant { get; set; }
}
