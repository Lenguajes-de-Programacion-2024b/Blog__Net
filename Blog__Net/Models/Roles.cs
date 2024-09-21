using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class Roles
{
    public int RolId { get; set; }

    public string? RolName { get; set; }
    public ICollection<InfoUser>? InfoUsers { get; set; }

}
