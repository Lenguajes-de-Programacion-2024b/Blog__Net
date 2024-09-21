namespace Blog__Net.Data;

public class Contexto
{
    public string CadenaSQl { get; }

    public Contexto(string Valor)
    {
        CadenaSQl = Valor;
    }
}

