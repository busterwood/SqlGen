using System.Linq;

namespace SqlGen
{
    public static partial class Extensions
    {
        public static string ToPascalCase(this string sqlName)
        {
            return string.Join("", sqlName.Split('_').Select(w => w.ToLower()).Select(w => char.ToUpper(w[0]) + w.Substring(1)));
        }
    }
}