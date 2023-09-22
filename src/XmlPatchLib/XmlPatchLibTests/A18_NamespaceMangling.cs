using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.18
    /// </summary>
    [TestClass]
    public class A18_NamespaceMangling
    {
        [TestMethod]
        public void NamespaceMangling()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A18_NamespaceMangling\NamespaceMangling.xml");

            var nsResolver = new XmlNamespaceManager(new NameTable());
            foreach (var ns in doc.Root!.GetNamespaceMap())
            {
                nsResolver.AddNamespace(string.IsNullOrWhiteSpace(ns.Key) ? "default" : ns.Key, ns.Value);
            }

            Shared.Patcher.PatchXml(doc, diff);

            var addedNode = doc.XPathSelectElement("//default:original")!.LastNode!;
            Assert.AreEqual(XmlNodeType.Element, addedNode.NodeType);
            Shared.TestWith((XElement)addedNode, elem =>
            {
                Assert.AreEqual("elem", elem.Name.LocalName);
                Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml", elem.Name.Namespace.NamespaceName);
                Assert.AreSame(elem, doc.XPathSelectElement("//x:elem"));

                Shared.TestWith((XElement)elem.FirstNode!, child =>
                {
                    Assert.AreEqual("child", child.Name.LocalName);
                    Assert.AreEqual("http://www.w3.org/2001/XMLSchema", child.Name.Namespace.NamespaceName);
                    Assert.AreSame(child, doc.XPathSelectElement("//x:elem/default:child"));
                });
            });
        }
    }
}
