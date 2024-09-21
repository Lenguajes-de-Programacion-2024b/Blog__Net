using Blog__Net.Data.Enums;
using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class Posts
{
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public CategoriaEnum Category { get; set; }

    public DateTime? Publicationdate { get; set; }

    public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();
}
