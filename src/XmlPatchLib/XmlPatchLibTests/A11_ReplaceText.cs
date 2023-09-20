using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.11
    /// </summary>
    [TestClass]
    public class A11_ReplaceText
    {
        [TestMethod]
        public void WithText()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var child1 = doc.XPathSelectElement("//main/child[@id='1']")!;
            Assert.AreEqual(1, child1.Nodes().Count());
            Assert.AreEqual(XmlNodeType.Text, child1.FirstNode!.NodeType);
            Assert.AreEqual("New Text", child1.Value);
        }

        [TestMethod]
        public void WithElement()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText_WithElement.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var child1 = doc.XPathSelectElement("//main/child[@id='1']")!;
            Assert.AreEqual(1, child1.Nodes().Count());
            Assert.AreEqual(XmlNodeType.Element, child1.FirstNode!.NodeType);
            Assert.AreEqual("child", ((XElement)child1.FirstNode).Name.LocalName);
        }

        [TestMethod]
        public void EmptyNode_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText_EmptyNode.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
