using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.XPath
{
    public interface IXPathEvaluator
    {
        T SelectSingle<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject;
        T SelectSingleOrDefault<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject;
        IEnumerable<T> SelectAll<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject;
    }
}
