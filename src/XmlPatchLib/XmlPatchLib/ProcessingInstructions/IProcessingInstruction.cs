using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.ProcessingInstructions
{
    internal interface IProcessingInstruction
    {
        void Execute(XDocument sourceDocument, IXmlNamespaceResolver nsResolver = null);
    }
}
