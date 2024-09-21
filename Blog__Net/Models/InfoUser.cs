using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog__Net.Models;

public partial class InfoUser
{
    public int IdUser { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Passcode { get; set; }
    public int? RolId { get; set; }

    public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();

    public virtual Roles? Rol { get; set; }

    [NotMapped]
    public string? ConfirmPasscode { get; set; } // No se guardará en la BD
}
