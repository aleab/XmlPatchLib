using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A08_ReplaceNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.TestSampleWithNamespaces;
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = GetNamespaceMap(doc.XPathSelectElement("/original")!);
            Assert.AreEqual("urn:ns:x", nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.TestSampleWithNamespaces;
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = GetNamespaceMap(doc.XPathSelectElement("/original")!);
            Assert.AreEqual("urn:ns:empty", nsmap[""]);
        }

        [TestMethod]
        public void MissingNamespace_ShouldThrowException()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        private static Dictionary<string, string> GetNamespaceMap(XElement element)
        {
            return element
               .Attributes().Where(attr => attr.IsNamespaceDeclaration)
               .ToDictionary(x => x.Name.LocalName == "xmlns" ? string.Empty : x.Name.LocalName, x => x.Value);
        }
    }
}
