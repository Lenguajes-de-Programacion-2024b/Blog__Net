using System.ComponentModel;

namespace Blog__Net.Data.Enums
{
    public enum CategoriaEnum
    {
        [Description("Noticias recientes")]
        Noticias,
        [Description("Novedades en tecnologia")]
        Tecnologia,
        [Description("Novedades de los videojuegos")]
        Videojuegos
    }
}
