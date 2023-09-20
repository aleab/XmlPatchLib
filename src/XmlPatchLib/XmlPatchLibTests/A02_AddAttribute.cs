using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.2
    /// </summary>
    [TestClass]
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

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));

            var child = doc.XPathSelectElement("//main/child[1]")!;
            var attr = child.Attribute("id");
            Assert.IsNotNull(attr);
            Assert.AreEqual("1", attr.Value);
        }
    }
}
