using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Models;

public partial class Zapros
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string UnitMeasure { get; set; } = null!;

    public DateOnly DateCreate { get; set; }

    public string StatusRequest { get; set; } = null!;

    public int FkUser { get; set; }

    public decimal? Amount { get; set; }

    public string? Ozm { get; set; }

    public DateTime? DateProcess { get; set; }

    public DateTime? DateComplete { get; set; }

    public virtual User FkUserNavigation { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
