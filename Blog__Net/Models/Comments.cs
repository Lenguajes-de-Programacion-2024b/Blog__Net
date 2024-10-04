using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog__Net.Models;

public partial class Comments
{
    public int CommentId { get; set; }

    public string? Content { get; set; }

    public DateTime? Creationdate { get; set; }

    public int? IdUser { get; set; }

    public int? PostId { get; set; }

    public int? CommentparentId { get; set; }

    public List<Comments>? SonComments { get; set; }
    [NotMapped]
    public string? UserName {  get; set; }
    public int? CommentgrandparentId { get; set; }

    public virtual Comments? Commentparent { get; set; }

    public virtual InfoUser? IdUserNavigation { get; set; }

    public virtual ICollection<Comments> InverseCommentparent { get; set; } = new List<Comments>();

    public virtual Posts? Post { get; set; }
}
