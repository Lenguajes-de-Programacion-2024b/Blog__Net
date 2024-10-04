using System.ComponentModel;
using System.Reflection;

namespace Blog__Net.Data.Enums
{
    public class CategoriaEnumHelper
    {
        public static string ObtainDescription(CategoriaEnum categoria)
        {
            FieldInfo? field = categoria.GetType().GetField(categoria.ToString());
            DescriptionAttribute? attribute= field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute != null ? attribute.Description : categoria.ToString();
        }
    }
}
