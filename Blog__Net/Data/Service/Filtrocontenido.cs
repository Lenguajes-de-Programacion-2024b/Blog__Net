namespace Blog__Net.Services 
{
    public static class FiltroContenido
{
    public static List<string> PalabrasInapropiadas = new List<string>
        {
            // Ejemplos de palabras ofensivas o groseras
            "tonto", "idiota", "estúpido", "imbécil", "burro", "tarado",
            "maldito", "bastardo", "perra", "mierda", "puto", "puta",
            "joder", "malparido", "pendejo", "coño", "carajo", "fuck", "shit",
            "asshole", "bitch", "damn", "hell", "suck", "bastard",
            
            // Frases o expresiones que pueden ser consideradas ofensivas o violentas
            "te voy a matar", "te voy a golpear", "vas a ver",
            "desearía que te mueras", "arruinaré tu vida",
            "vas a pagar por esto", "voy a hacer que te arrepientas", 
            
            // Palabras asociadas con odio o discriminación
            "racista", "machista", "nazi", "homofóbico", "misógino",
            "sexista", "gordo", "fea", "tonto", "negro", "marica",
            "puta", "puto", "anormal", "loco", "mongólico",
            "down", "inválido", "subnormal"
        };

    public static bool ContienePalabrasInapropiadas(string content)
    {
        foreach (var palabra in PalabrasInapropiadas)
        {
            // Búsqueda insensible a mayúsculas y minúsculas
            if (content.IndexOf(palabra, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }
        return false;
    }
}
}
