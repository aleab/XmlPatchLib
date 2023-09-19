using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A07_ReplaceAttribute
    {
        [TestMethod]
        public void ExistingAttribute()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var elem = doc.XPathSelectElement("//attributedNode")!;
            Assert.AreEqual("new-attr-value", elem.Attribute("test")?.Value);
        }

        [TestMethod]
        public void MissingAttribute_ShouldThrowException()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute_Missing.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
