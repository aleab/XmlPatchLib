using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.12
    /// </summary>
    [TestClass]
    public class A12_RemoveElement
    {
        [TestMethod]
        public void ExistingElement()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A12_Remove\RemoveElement.xml");

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsFalse(((IEnumerable)doc.XPathEvaluate("//main/child[@id='1']")).GetEnumerator().MoveNext(), "element was not removed");
        }

        [TestMethod]
        public void MissingElement_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A12_Remove\RemoveElement_Missing.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void WhiteSpace_Before()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A12_Remove\RemoveElement_WhiteSpaceBefore.xml");

            var wsBefore = doc.XPathSelectElement("//main/child[@id='1']")!.PreviousNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsBefore.Document, "previous whitespace node was not removed");
            Assert.AreEqual(nodeCount - 2, doc.XPathSelectElement("//main")!.Nodes().Count());
        }

        [TestMethod]
        public void WhiteSpace_After()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A12_Remove\RemoveElement_WhiteSpaceAfter.xml");

            var wsAfter = doc.XPathSelectElement("//main/child[@id='1']")!.NextNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsAfter.Document, "following whitespace node was not removed");
            Assert.AreEqual(nodeCount - 2, doc.XPathSelectElement("//main")!.Nodes().Count());
        }

        [TestMethod]
        public void WhiteSpace_Both()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A12_Remove\RemoveElement_WhiteSpaceBoth.xml");

            var child = doc.XPathSelectElement("//main/child[@id='1']")!;
            var wsBefore = child.PreviousNode!;
            var wsAfter = child.NextNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsBefore.Document, "previous whitespace node was not removed");
            Assert.IsNull(wsAfter.Document, "following whitespace node was not removed");
            Assert.AreEqual(nodeCount - 3, doc.XPathSelectElement("//main")!.Nodes().Count());
        }
    }
}
