using Blog__Net.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Blog__Net.Data;

public class Contexto
{
    public string CadenaSQl { get; }

        public Contexto(string Valor)
        {
            CadenaSQl = Valor;
        }
    }

    public DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<Posts> Posts { get; set; }
}




