using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    internal static class Utils
    {
        public static Regex XmlNCName = new Regex("[_A-Za-z][-._A-Za-z0-9]*");
        public static Regex XmlQName = new Regex($"({XmlNCName}:)?{XmlNCName}");

        public static Dictionary<string, string> GetNamespaceMap(this XElement element)
        {
            return element.Attributes().Where(attr => attr.IsNamespaceDeclaration)
               .ToDictionary(x => x.Name.LocalName == "xmlns" ? string.Empty : x.Name.LocalName, x => x.Value);
        }

        public static IXmlNamespaceResolver GetNamespaceManager(this XDocument doc, string defaultNamespace = null)
        {
            var resolver = new XmlNamespaceManager(new NameTable());
            resolver.CopyNamespacesFrom(doc, defaultNamespace);
            return resolver;
        }

        public static void CopyNamespacesFrom(this XmlNamespaceManager xmlNamespaceManager, XDocument doc, string defaultNamespace = null)
        {
            if (doc?.Root == null)
                return;

            foreach (var attr in doc.Root.Attributes().Where(attr => attr.IsNamespaceDeclaration))
            {
                var prefix = attr.Name.LocalName == "xmlns" ? string.Empty : attr.Name.LocalName;

                if (!string.IsNullOrWhiteSpace(prefix))
                    xmlNamespaceManager.AddNamespace(prefix, attr.Value);
                else if (defaultNamespace != null)
                    xmlNamespaceManager.AddNamespace(defaultNamespace.Trim(), attr.Value);
            }
        }

        /// <summary>
        ///     Parses an XML NCName or QName into an <see cref="XName"/>.
        /// </summary>
        /// <param name="name">A valid NCName or QName</param>
        /// <param name="resolver">The <see cref="IXmlNamespaceResolver"/> used to map prefixes to namespace URIs.</param>
        public static XName GetXName(string name, IXmlNamespaceResolver resolver)
        {
            var colon = name.IndexOf(':');
            var prefix = colon > 0 ? name.Substring(0, colon) : "";
            var uri = resolver.LookupNamespace(prefix);
            return !string.IsNullOrWhiteSpace(uri)
                ? XName.Get(colon > 0 ? name.Substring(colon + 1) : name, uri)
                : XName.Get(name);
        }

        public static bool IsWhitespace(this XNode node)
        {
            if (node == null)
                return false;
            return node.NodeType == XmlNodeType.Whitespace || (node.NodeType == XmlNodeType.Text && string.IsNullOrWhiteSpace(((XText)node).Value));
        }

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
