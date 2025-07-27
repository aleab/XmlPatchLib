using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests;

/// <summary>
///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.17
/// </summary>
[TestClass]
[TestCategory("<remove>")]
public class A17_RemoveText
{
    [TestMethod]
    public void ExistingText()
    {
        var doc = Shared.GetTestSample();
        var diff = XDocument.Load(@"TestData\A17_Remove\RemoveText.xml");

        Shared.Patcher.PatchXml(doc, diff);

        Assert.IsTrue(doc.XPathSelectElement("//main/child[@id='1']")!.IsEmpty);
    }
}
