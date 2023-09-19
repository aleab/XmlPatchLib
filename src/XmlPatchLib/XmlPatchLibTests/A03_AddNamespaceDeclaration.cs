using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A03_AddNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);
            
            var nsmap = GetNamespaceMap(doc.XPathSelectElement("//main")!);
            Assert.AreEqual("urn:ns:x", nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);
            
            var nsmap = GetNamespaceMap(doc.XPathSelectElement("//main")!);
            Assert.AreEqual("urn:ns:empty", nsmap[""]);
        }

        [TestMethod]
        public void ExistingNamespace_ShouldThrowException()
        {
            var doc = Shared.TestSampleWithNamespaces;
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration_Empty.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
            
            var nsmap = GetNamespaceMap(doc.Root!);
            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml", nsmap["x"]);
        }

        private static Dictionary<string, string> GetNamespaceMap(XElement element)
        {
            return element
               .Attributes().Where(attr => attr.IsNamespaceDeclaration)
               .ToDictionary(x => x.Name.LocalName == "xmlns" ? string.Empty : x.Name.LocalName, x => x.Value);
        }
    }
}
