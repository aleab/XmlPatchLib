using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.14
    /// </summary>
    [TestClass]
    public class A14_RemoveNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = GetNamespaceMap(doc.Root!);
            Assert.IsNull(nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = GetNamespaceMap(doc.Root!);
            Assert.IsNull(nsmap[""]);
        }

        [TestMethod]
        public void MissingNamespace_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        private static Dictionary<string, string> GetNamespaceMap(XElement element)
        {
            return element
               .Attributes().Where(attr => attr.IsNamespaceDeclaration)
               .ToDictionary(x => x.Name.LocalName == "xmlns" ? string.Empty : x.Name.LocalName, x => x.Value);
        }
    }
}
