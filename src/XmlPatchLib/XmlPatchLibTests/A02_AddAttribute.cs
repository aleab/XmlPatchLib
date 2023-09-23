using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.2
    /// </summary>
    [TestClass]
    [TestCategory("<add>")]
    public class A02_AddAttribute
    {
        [TestMethod]
        public void NewAttribute()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A02_Add\AddAttribute.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var child = doc.XPathSelectElement("//main/child[@id='1']")!;
            var attr = child.Attribute("attr");
            Assert.IsNotNull(attr);
            Assert.AreEqual("new-attr", attr.Value);
        }

        [TestMethod]
        public void ExistingAttribute_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A02_Add\AddAttribute_Existing.xml");

            Assert.ThrowsException<InvalidPatchDirectiveException>(() => Shared.Patcher.PatchXml(doc, diff));

            var child = doc.XPathSelectElement("//main/child[1]")!;
            var attr = child.Attribute("id");
            Assert.IsNotNull(attr);
            Assert.AreEqual("1", attr.Value);
        }

        [TestMethod]
        public void PrefixedAttribute()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A02_Add\AddAttribute_Prefixed.xml");

            var nsResolver = new XmlNamespaceManager(new NameTable());
            foreach (var ns in doc.Root!.GetNamespaceMap())
            {
                nsResolver.AddNamespace(string.IsNullOrWhiteSpace(ns.Key) ? "default" : ns.Key, ns.Value);
            }

            Shared.Patcher.PatchXml(doc, diff);

            var main = doc.XPathSelectElement("//x:main", nsResolver)!;
            var attr = main.Attribute(XName.Get("{http://schemas.microsoft.com/winfx/2006/xaml}attr"));
            Assert.IsNotNull(attr);
            Assert.AreEqual("new-attr", attr.Value);
        }
    }
}
