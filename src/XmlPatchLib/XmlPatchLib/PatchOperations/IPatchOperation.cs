using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal interface IPatchOperation
    {
        void Apply(XDocument sourceDocument, IPatchOperationOptions options, IXmlNamespaceResolver nsResolver = null);
    }
}
