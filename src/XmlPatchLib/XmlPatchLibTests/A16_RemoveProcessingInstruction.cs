using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.16
    /// </summary>
    [TestClass]
    [TestCategory("<remove>")]
    public class A16_RemoveProcessingInstruction
    {
        [TestMethod]
        public void ExistingProcessingInstruction()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A16_Remove\RemoveProcessingInstruction.xml");

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsFalse(((IEnumerable)doc.XPathEvaluate("/original/processing-instruction('test')")).GetEnumerator().MoveNext(), "processing-instruction was not removed");
        }

        [TestMethod]
        public void MissingProcessingInstruction_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A16_Remove\RemoveProcessingInstruction_Missing.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
