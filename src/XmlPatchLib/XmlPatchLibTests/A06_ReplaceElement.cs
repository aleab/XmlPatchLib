using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests;

/// <summary>
///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.6
/// </summary>
[TestClass]
[TestCategory("<replace>")]
public class A06_ReplaceElement
{
    [TestMethod]
    public void ExistingElement()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement.xml");

        var oldElem = doc.XPathSelectElement("//main/child[@id='1']")!;

        Shared.Patcher.PatchXml(doc, diff);

        Assert.IsNull(oldElem.Document);

        var child1 = doc.XPathSelectElement("//main/child[@id='new-id']");
        Assert.IsNotNull(child1);
        Assert.AreEqual("Replaced test!", child1.Value);
    }

    [TestMethod]
    public void MissingElement_ShouldThrowException()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement_Missing.xml");

        Assert.ThrowsException<UnlocatedNodeException>(() => Shared.Patcher.PatchXml(doc, diff));

        var child = doc.XPathSelectElement("//main/child[@id='3']");
        Assert.IsNull(child);
    }

    [TestMethod]
    public void WithDifferentNodeType_ShouldThrowException()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement_WithDifferentNodeType.xml");

        Assert.ThrowsException<InvalidNodeTypeException>(() => Shared.Patcher.PatchXml(doc, diff));
    }

    [TestMethod]
    public void WithMultipleNodes_ShouldThrowException()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement_WithMultipleNodes.xml");

        Assert.ThrowsException<InvalidNodeTypeException>(() => Shared.Patcher.PatchXml(doc, diff));
    }
}
