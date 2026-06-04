using System;
using System.Collections.Generic;

namespace TaskManager.Models;

public partial class StoryModification
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Action { get; set; } = null!;

    public int FkUser { get; set; }

    public int? FkRequest { get; set; }

    public virtual Request? FkRequestNavigation { get; set; }

    public virtual User FkUserNavigation { get; set; } = null!;
}
