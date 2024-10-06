
﻿using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;
namespace Blog__Net.Data
{
    public class Contexto
    {
        public string CadenaSQl { get; } // Propiedad pública solo de lectura

        public Contexto(string Valor)
        {
            CadenaSQl = Valor;
        }
    }
}




