using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.3
    /// </summary>
    [TestClass]
    [TestCategory("<add>")]
    public class A03_AddNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.XPathSelectElement("//main")!.GetNamespaceMap();
            Assert.AreEqual("urn:ns:x", nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.XPathSelectElement("//main")!.GetNamespaceMap();
            Assert.AreEqual("urn:ns:empty", nsmap[""]);
        }

        [TestMethod]
        public void ExistingNamespace_ShouldThrowException()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A03_Add\AddNamespaceDeclaration_Empty.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));

            var nsmap = doc.Root!.GetNamespaceMap();
            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml", nsmap["x"]);
        }
    }
}
