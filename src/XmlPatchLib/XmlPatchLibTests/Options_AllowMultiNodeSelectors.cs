using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    [TestCategory("Options")]
    public class Options_AllowMultiNodeSelectors
    {
        private static readonly XmlPatcher Patcher = new XmlPatcher(new XmlPatcherOptions { AllowMultiNodeSelectors = true });

        [TestMethod]
        public void AddElement()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\Options\AllowMultiNodeSelectors_AddElement.xml");

            var children = doc.XPathSelectElement("//main")!.Elements("child");

            Patcher.PatchXml(doc, diff);

            if (children.Any(child => (child.NextNode as XElement)?.Name.LocalName != "new-child"))
                Assert.Fail();
        }

        [TestMethod]
        public void AddAttribute()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\Options\AllowMultiNodeSelectors_AddAttribute.xml");

            var children = doc.XPathSelectElement("//main")!.Elements("child");

            Patcher.PatchXml(doc, diff);

            if (children.Any(child => child.Attribute("new-attr")?.Value != "value"))
                Assert.Fail();
        }

        [TestMethod]
        public void AddComment()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\Options\AllowMultiNodeSelectors_AddComment.xml");

            var children = doc.XPathSelectElement("//main")!.Elements("child");

            Patcher.PatchXml(doc, diff);

            if (children.Any(child => (child.NextNode as XComment)?.Value != " New Comment "))
                Assert.Fail();
        }
    }
}
