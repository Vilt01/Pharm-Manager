using System;
using System.Collections.Generic;

namespace TaskManager.Models;

public partial class Request
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int NumberOrder { get; set; }

    public int Daas { get; set; }

    public string Party { get; set; } = null!;

    public int FkZapros { get; set; }

    public int FkUser { get; set; }

    public virtual User FkUserNavigation { get; set; } = null!;

    public virtual Zapros FkZaprosNavigation { get; set; } = null!;

    public virtual ICollection<StoryModification> StoryModifications { get; set; } = new List<StoryModification>();
}
