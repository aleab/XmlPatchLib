using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.7
    /// </summary>
    [TestClass]
    [TestCategory("<replace>")]
    public class A07_ReplaceAttribute
    {
        [TestMethod]
        public void ExistingAttribute()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var elem = doc.XPathSelectElement("//attributedNode")!;
            Assert.AreEqual("new-attr-value", elem.Attribute("test")?.Value);
        }

        [TestMethod]
        public void MissingAttribute_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute_Missing.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void PrefixedAttribute()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute_Prefixed.xml");

            var child = doc.XPathSelectElement("//_:child[@x:id='1']", diff.GetNamespaceResolver())!;

            Shared.Patcher.PatchXml(doc, diff);

            Assert.AreEqual("new-id", child.Attribute(XName.Get("id", "http://schemas.microsoft.com/winfx/2006/xaml"))?.Value);
        }

        [TestMethod]
        public void WithXNode_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute_WithXNode.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
