using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.4
    /// </summary>
    [TestClass]
    public class A04_AddComment
    {
        [TestMethod]
        public void Prepend()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A04_Add\AddComment_Prepend.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var firstNode = doc.XPathSelectElement("//main")!.FirstNode!;
            Assert.AreEqual(XmlNodeType.Comment, firstNode.NodeType);
            Assert.AreEqual(" New Comment ", ((XComment)firstNode).Value);
        }

        [TestMethod]
        public void Append()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A04_Add\AddComment_Append.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var lastNode = doc.XPathSelectElement("//main")!.LastNode!;
            Assert.AreEqual(XmlNodeType.Comment, lastNode.NodeType);
            Assert.AreEqual(" New Comment ", ((XComment)lastNode).Value);
        }

        [TestMethod]
        public void Before()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A04_Add\AddComment_Before.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var node = doc.XPathSelectElement("//main/child[@id='1']")!.NextNode!;
            Assert.AreEqual(XmlNodeType.Comment, node.NodeType);
            Assert.AreEqual(" New Comment ", ((XComment)node).Value);
        }

        [TestMethod]
        public void After()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A04_Add\AddComment_After.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var node = doc.XPathSelectElement("//main/child[@id='1']")!.NextNode!;
            Assert.AreEqual(XmlNodeType.Comment, node.NodeType);
            Assert.AreEqual(" New Comment ", ((XComment)node).Value);
        }
    }
}
