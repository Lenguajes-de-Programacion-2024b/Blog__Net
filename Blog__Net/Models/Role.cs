using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class Role
{
    public int RolId { get; set; }

    public string? RolName { get; set; }

    public virtual ICollection<InfoUser> InfoUsers { get; set; } = new List<InfoUser>();
}
