using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    public static class XmlLinqExtensions
    {
        public static Dictionary<string, string> GetNamespaceMap(this XElement element)
        {
            return element
               .Attributes().Where(attr => attr.IsNamespaceDeclaration)
               .ToDictionary(x => x.Name.LocalName == "xmlns" ? string.Empty : x.Name.LocalName, x => x.Value);
        }
    }
}
