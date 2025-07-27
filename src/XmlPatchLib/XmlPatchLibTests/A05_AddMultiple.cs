using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests;

/// <summary>
///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.5
/// </summary>
[TestClass]
[TestCategory("<add>")]
public class A05_AddMultiple
{
    [TestMethod]
    public void Prepend()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A05_Add\AddMultiple_Prepend.xml");

        var firstNodeBeforePatch = doc.XPathSelectElement("//main")!.FirstNode;
        Shared.Patcher.PatchXml(doc, diff);

        var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
        Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");
        Assert.AreSame(doc.XPathSelectElement("//main")!.FirstNode, addedElement);

        var lastNode = TestNextNodes(addedElement);
        Assert.AreEqual(XmlNodeType.Comment, lastNode.NextNode?.NodeType);
        Assert.AreSame(firstNodeBeforePatch, lastNode.NextNode);
    }

    [TestMethod]
    public void Append()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A05_Add\AddMultiple_Append.xml");

        var lastNodeBeforePatch = doc.XPathSelectElement("//main")!.LastNode;
        Shared.Patcher.PatchXml(doc, diff);

        var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
        Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");
        Assert.AreEqual(XmlNodeType.Text, addedElement.PreviousNode?.NodeType);

        var lastNode = TestNextNodes(addedElement);
        Assert.AreSame(doc.XPathSelectElement("//main")!.LastNode, lastNode);
        Assert.AreSame(lastNodeBeforePatch, addedElement.PreviousNode);
    }

    [TestMethod]
    public void Before()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A05_Add\AddMultiple_Before.xml");

        Shared.Patcher.PatchXml(doc, diff);
        TestBeforeAfter(doc);
    }

    [TestMethod]
    public void After()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A05_Add\AddMultiple_After.xml");

        Shared.Patcher.PatchXml(doc, diff);
        TestBeforeAfter(doc);
    }

    private static void TestBeforeAfter(XNode doc)
    {
        var addedElement = doc.XPathSelectElement("//main/child[@id='3']");
        Assert.IsNotNull(addedElement, $"{nameof(addedElement)} is null");

        Assert.AreEqual(XmlNodeType.Element, addedElement.PreviousNode?.NodeType);
        Shared.TestWith((XElement)addedElement.PreviousNode!, prev =>
        {
            Assert.AreEqual("child", prev.Name.LocalName);
            Assert.AreEqual("1", prev.Attribute("id")?.Value);
        });

        var lastNode = TestNextNodes(addedElement);

        Assert.AreEqual(XmlNodeType.Element, lastNode.NextNode?.NodeType);
        Shared.TestWith((XElement)lastNode.NextNode!, next =>
        {
            Assert.AreEqual("child", next.Name.LocalName);
            Assert.AreEqual("2", next.Attribute("id")?.Value);
        });
    }

    private static XNode TestNextNodes(XNode addedElement)
    {
        var nextAddedNode = addedElement.NextNode;
        Assert.IsNotNull(nextAddedNode, $"2nd {nameof(nextAddedNode)} is null");
        Assert.AreEqual(XmlNodeType.Comment, nextAddedNode.NodeType);
        Assert.AreEqual(" New Comment ", ((XComment)nextAddedNode).Value);

        nextAddedNode = nextAddedNode.NextNode;
        Assert.IsNotNull(nextAddedNode, $"3rd {nameof(nextAddedNode)} is null");
        Assert.AreEqual(XmlNodeType.Text, nextAddedNode.NodeType);
        Assert.IsTrue(Regex.IsMatch(((XText)nextAddedNode).Value, @"\n\s*Text Node\n\s*"));

        return nextAddedNode;
    }
}
