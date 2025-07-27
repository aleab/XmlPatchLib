using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests;

/// <summary>
///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.13
/// </summary>
[TestClass]
[TestCategory("<remove>")]
public class A13_RemoveAttribute
{
    [TestMethod]
    public void ExistingAttribute()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A13_Remove\RemoveAttribute.xml");

        Shared.Patcher.PatchXml(doc, diff);

        var elem = doc.XPathSelectElement("//attributedNode")!;
        Assert.IsNull(elem.Attribute("test"));
    }

    [TestMethod]
    public void MissingAttribute_ShouldThrowException()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A13_Remove\RemoveAttribute_Missing.xml");

        Assert.ThrowsException<UnlocatedNodeException>(() => Shared.Patcher.PatchXml(doc, diff));
    }
}
