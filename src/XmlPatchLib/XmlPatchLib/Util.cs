using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    internal static class Util
    {
        public static Regex XmlNCName = new Regex("[_A-Za-z][-._A-Za-z0-9]*");
        public static Regex XmlQName = new Regex($"({XmlNCName}:)?{XmlNCName}");

        public static string ToString(XElement element)
        {
            if (element != null)
            {
                var attributes = element.Attributes().Select(a => $"{a.Name}=\"{a.Value}\"");
                return $"<{element.Name}{(element.HasAttributes ? $" {string.Join(" ", attributes)}" : "")}>";
            }

            return "null";
        }
    }
}
