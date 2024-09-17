using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Category { get; set; }

    public DateTime? Publicationdate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
