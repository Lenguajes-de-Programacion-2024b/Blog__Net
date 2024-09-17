using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class InfoUser
{
    public int IdUser { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Passcode { get; set; }

    public int? RolId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Role? Rol { get; set; }
}
