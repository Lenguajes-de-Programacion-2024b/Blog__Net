using Blog__Net.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog__Net.Models;

public partial class Posts
{
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
    public int IdUser { get; set; }

    public CategoriaEnum Category { get; set; }

    public DateTime Publicationdate { get; set; }

    public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();
    [NotMapped]
    public string? UserName { get; set; }
}
