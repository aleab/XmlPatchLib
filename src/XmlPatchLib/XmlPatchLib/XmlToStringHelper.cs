using System.Linq;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    internal static class XmlToStringHelper
    {
        public static string Head(XElement element)
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
