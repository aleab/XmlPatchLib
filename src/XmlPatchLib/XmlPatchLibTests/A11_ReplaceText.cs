using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A11_ReplaceText
    {
        [TestMethod]
        public void WithText()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var child1 = doc.XPathSelectElement("//main/child[@id='1']")!;
            Assert.AreEqual(1, child1.Nodes().Count());
            Assert.AreEqual(XmlNodeType.Text, child1.FirstNode!.NodeType);
            Assert.AreEqual("New Text", child1.Value);
        }

        [TestMethod]
        public void EmptyNode_ShouldThrowException()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText_Empty.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
