using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    [TestCategory("Options")]
    public class Options_DisableReplaceRestrictions
    {
        private static readonly XmlPatcher Patcher = new XmlPatcher(new XmlPatcherOptions { DisableReplaceRestrictions = true });

        [TestMethod]
        public void Element()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement_WithDifferentNodeType.xml");

            var main = doc.XPathSelectElement("//main")!;
            var child = doc.XPathSelectElement("//main/child[@id='1']")!;

            Patcher.PatchXml(doc, diff);

            Assert.IsNull(child.Document);
            Shared.TestWith(main.Nodes().ToList()[1], node =>
            {
                Assert.AreEqual(XmlNodeType.Comment, node.NodeType);
                Assert.AreEqual(((XComment)node).Value, " Not an Element ");
            });
        }

        [TestMethod]
        public void Attribute_WithXNode_ShouldStillThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A07_Replace\ReplaceAttribute_WithXNode.xml");

            Assert.ThrowsException<InvalidNodeTypeException>(() => Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void Comment()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A09_Replace\ReplaceComment_WithDifferentNodeType.xml");

            var comment = (XComment)((IEnumerable<object>)doc.XPathEvaluate("//main/comment()[1]")).First();

            Patcher.PatchXml(doc, diff);

            Assert.IsNull(comment.Document);
            Shared.TestWith(doc.XPathSelectElement("//main")!.FirstNode!, node =>
            {
                Assert.AreEqual(XmlNodeType.Text, node.NodeType);
                Assert.AreEqual(((XText)node).Value, "Not a Comment");
            });
        }

        [TestMethod]
        public void Text()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A11_Replace\ReplaceText_WithDifferentNodeType.xml");

            var text = (XText)((IEnumerable<object>)doc.XPathEvaluate("//main/child[@id='1']/text()[1]")).First();

            Patcher.PatchXml(doc, diff);

            Assert.IsNull(text.Document);
            Shared.TestWith(doc.XPathSelectElement("//main/child[@id='1']")!.FirstNode!, node =>
            {
                Assert.AreEqual(XmlNodeType.Element, node.NodeType);
                Assert.AreEqual(((XElement)node).Name.LocalName, "not-a-text-node");
            });
        }
    }
}
