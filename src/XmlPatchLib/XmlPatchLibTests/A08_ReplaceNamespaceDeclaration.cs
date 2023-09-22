using System.Xml.Linq;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.8
    /// </summary>
    [TestClass]
    public class A08_ReplaceNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.Root!.GetNamespaceMap();
            Assert.AreEqual("urn:ns:x", nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.Root!.GetNamespaceMap();
            Assert.AreEqual("urn:ns:empty", nsmap[""]);
        }

        [TestMethod]
        public void MissingNamespace_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A08_Replace\ReplaceNamespaceDeclaration.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
