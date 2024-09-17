using System;
using System.Collections.Generic;

namespace Blog__Net.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string? Content { get; set; }

    public DateTime? Creationdate { get; set; }

    public int? IdUser { get; set; }

    public int? PostId { get; set; }

    public int? CommentparentId { get; set; }

    public virtual Comment? Commentparent { get; set; }

    public virtual InfoUser? IdUserNavigation { get; set; }

    public virtual ICollection<Comment> InverseCommentparent { get; set; } = new List<Comment>();

    public virtual Post? Post { get; set; }
}
