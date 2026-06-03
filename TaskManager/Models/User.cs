using System;
using System.Collections.Generic;

namespace TaskManager.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int FkRole { get; set; }

    public int? FkManagerId { get; set; }

    public int? FkDepartment { get; set; }

    public byte[]? Avatar { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Department? FkDepartmentNavigation { get; set; }

    public virtual User? FkManager { get; set; }

    public virtual Role FkRoleNavigation { get; set; } = null!;

    public virtual ICollection<User> InverseFkManager { get; set; } = new List<User>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<StoryModification> StoryModifications { get; set; } = new List<StoryModification>();

    public virtual ICollection<Zapros> Zapros { get; set; } = new List<Zapros>();
}
