using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    [TestCategory("Options")]
    public class Options_UseProcessingInstrutions
    {
        private static readonly XmlPatcher Patcher = new XmlPatcher(new XmlPatcherOptions { UseProcessingInstrutions = true });

        [TestMethod]
        public void CopyNode()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\Options\UseProcessingInstructions_CopyNode.xml");

            Patcher.PatchXml(doc, diff);

            var main = doc.XPathSelectElement("//main")!;
            Assert.AreSame(doc.XPathSelectElement("//wrapper"), main.Parent);
        }
    }
}
