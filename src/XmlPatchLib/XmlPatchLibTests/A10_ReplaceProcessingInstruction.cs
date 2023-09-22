using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.10
    /// </summary>
    [TestClass]
    [TestCategory("<replace>")]
    public class A10_ReplaceProcessingInstruction
    {
        [TestMethod]
        public void ExistingProcessingInstruction()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A10_Replace\ReplaceProcessingInstruction.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var res = doc.XPathEvaluate("/original/processing-instruction()");
            Assert.IsInstanceOfType<IEnumerable>(res);
            Shared.TestWith(((IEnumerable)res).Cast<XProcessingInstruction>().ToList(), processingInstructions =>
            {
                Assert.AreEqual(1, processingInstructions.Count);
                Assert.AreEqual("bar=\"foo\"", processingInstructions[0].Data);
            });
        }

        [TestMethod]
        public void MissingProcessingInstruction_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A10_Replace\ReplaceProcessingInstruction_Missing.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void WithDifferentNodeType_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A10_Replace\ReplaceProcessingInstruction_WithDifferentNodeType.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
