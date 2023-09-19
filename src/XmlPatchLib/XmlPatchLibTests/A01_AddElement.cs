using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A01_AddElement
    {
        [TestMethod]
        public void Prepend()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A01_Add\AddElement_Prepend.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
            Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");
            Assert.AreSame(doc.XPathSelectElement("//main")!.FirstNode, addedElement, $"{nameof(addedElement)} is not the first node");

            var nextNode = addedElement.NextNode;
            Assert.IsNotNull(nextNode, $"{nameof(nextNode)} is null");
            Assert.AreEqual(XmlNodeType.Comment, nextNode.NodeType);
        }

        [TestMethod]
        public void Append()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A01_Add\AddElement_Append.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
            Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");
            Assert.AreSame(doc.XPathSelectElement("//main")!.LastNode, addedElement);

            var prevNode = addedElement.PreviousNode;
            Assert.IsNotNull(prevNode);
            Assert.AreEqual(XmlNodeType.Text, prevNode.NodeType);
        }

        [TestMethod]
        public void Before()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A01_Add\AddElement_Before.xml");

            Shared.Patcher.PatchXml(doc, diff);
            TestBeforeAfter(doc);
        }

        [TestMethod]
        public void After()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A1_Add\AddElement_After.xml");

            Shared.Patcher.PatchXml(doc, diff);
            TestBeforeAfter(doc);
        }

        [TestMethod]
        public void ComplexChild()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A01_Add\AddElement_ComplexChild.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var addedElement = doc.XPathSelectElement("//main/complex-child[@id='3']");
            Assert.IsNotNull(addedElement, $"1st {nameof(addedElement)} is null");

            var childNodes = addedElement.Nodes().ToList();
            Assert.AreEqual(5, childNodes.Count);

            // 1st child
            Assert.AreEqual(XmlNodeType.Comment, childNodes[0].NodeType, "[0]");
            Assert.AreEqual(" 1st child node ", ((XComment)childNodes[0]).Value);

            // 2nd child
            Assert.AreEqual(XmlNodeType.Element, childNodes[1].NodeType, "[1]");
            Shared.TestWith((XElement)childNodes[1], child1 =>
            {
                Assert.AreEqual("child", child1.Name.LocalName, "[1]");
                Assert.AreEqual("with-text", child1.Attribute("id")?.Value, "[1]");
                Assert.AreEqual(1, child1.Nodes().Count(), "[1] should have 1 node");
                Assert.IsTrue(Regex.IsMatch(child1.Value, @"\n\s*2nd child inner text\n\s*"), "[1]'s text value");
            });

            // 3rd child
            Assert.AreEqual(XmlNodeType.Element, childNodes[2].NodeType, "[2]");
            Shared.TestWith((XElement)childNodes[2], child2 =>
            {
                Assert.AreEqual("child", child2.Name.LocalName, "[2]");
                Assert.AreEqual("with-nested-nodes", child2.Attribute("id")?.Value, "[2]");
                Shared.TestWith(child2.Nodes().ToList(), nodes =>
                {
                    Assert.AreEqual(3, nodes.Count, "[2] should have 3 nodes");

                    // > 1st child
                    Assert.AreEqual(XmlNodeType.Text, nodes[0].NodeType, "[2][0]");
                    Assert.IsTrue(Regex.IsMatch(((XText)nodes[0]).Value, @"\n\s*Text Node\n\s*"), "[2][0]'s text value");

                    // > 2nd child
                    Assert.AreEqual(XmlNodeType.Element, nodes[1].NodeType, "[2][1]");
                    Shared.TestWith((XElement)nodes[1], child2_1 =>
                    {
                        Assert.AreEqual("child", child2_1.Name.LocalName, "[2][1]");
                        Assert.AreEqual("nested-with-text", child2_1.Attribute("id")?.Value, "[2][1]");
                        Assert.AreEqual(1, child2_1.Nodes().Count(), "[2][1] should have 1 node");
                        Assert.AreEqual("Inner Text", child2_1.Value, "[2][1]'s text value");
                    });

                    // > 3rd child
                    Assert.AreEqual(XmlNodeType.Comment, nodes[2].NodeType, "[2][2]");
                    Assert.AreEqual(" Comment Node ", ((XComment)nodes[2]).Value, "[2][2]");
                });
            });

            // 4th child
            Assert.AreEqual(XmlNodeType.Text, childNodes[3].NodeType, "[3]");
            Assert.IsTrue(Regex.IsMatch(((XText)childNodes[3]).Value, @"\n\s*Text Node\n\s*"), "[3]'s text value");

            // 5th child
            Assert.AreEqual(XmlNodeType.Element, childNodes[4].NodeType, "[4]");
            Shared.TestWith((XElement)childNodes[4], child4 =>
            {
                Assert.AreEqual("child", child4.Name.LocalName, "[4]");
                Assert.AreEqual("empty", child4.Attribute("id")?.Value, "[4]");
                Assert.IsTrue(child4.IsEmpty, "[4] should be empty");
            });
        }

        private static void TestBeforeAfter(XNode doc)
        {
            var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
            Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");

            var prevNode = addedElement.PreviousNode;
            Assert.IsNotNull(prevNode, $"{nameof(prevNode)} is null");
            Assert.AreEqual(XmlNodeType.Element, prevNode.NodeType);
            Assert.IsTrue(((XElement)prevNode).Name.LocalName == "child" && ((XElement)prevNode).Attribute("id")?.Value == "1");

            var nextNode = addedElement.NextNode;
            Assert.IsNotNull(nextNode, $"{nameof(nextNode)} is null");
            Assert.AreEqual(XmlNodeType.Element, prevNode.NodeType);
            Assert.IsTrue(((XElement)nextNode).Name.LocalName == "child" && ((XElement)nextNode).Attribute("id")?.Value == "2");
        }
    }
}
