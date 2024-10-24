<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog__Net.Models
{
    public class PostLike
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

    }
}
=======
﻿namespace Blog__Net.Models
{
    public class PostLike
    {

        public Guid Id { get; set; }

        public int PostId { get; set; }

        public int? UserId { get; set; }

        public DateTime LikeDate { get; set; }

    }
}
>>>>>>> refs/remotes/origin/Camilo
