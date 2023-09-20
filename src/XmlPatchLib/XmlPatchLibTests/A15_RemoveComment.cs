using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.15
    /// </summary>
    [TestClass]
    public class A15_RemoveComment
    {
        [TestMethod]
        public void ExistingComment()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A15_Remove\RemoveComment.xml");

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsFalse(((IEnumerable)doc.XPathEvaluate("//main/comment()[1]")).GetEnumerator().MoveNext(), "comment was not removed");
        }

        [TestMethod]
        public void MissingComment_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A15_Remove\RemoveComment_Missing.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void WhiteSpace_Before()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A15_Remove\RemoveComment_WhiteSpaceBefore.xml");

            var wsBefore = ((IEnumerable)doc.XPathEvaluate("//main/comment()")).Cast<XComment>().First().PreviousNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsBefore.Document, "previous whitespace node was not removed");
            Assert.AreEqual(nodeCount - 2, doc.XPathSelectElement("//main")!.Nodes().Count());
        }

        [TestMethod]
        public void WhiteSpace_After()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A15_Remove\RemoveComment_WhiteSpaceAfter.xml");

            var wsAfter = ((IEnumerable)doc.XPathEvaluate("//main/comment()")).Cast<XComment>().First().NextNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsAfter.Document, "following whitespace node was not removed");
            Assert.AreEqual(nodeCount - 2, doc.XPathSelectElement("//main")!.Nodes().Count());
        }

        [TestMethod]
        public void WhiteSpace_Both()
        {
            var doc = Shared.GetTestSample(LoadOptions.PreserveWhitespace);
            var diff = XDocument.Load(@"TestData\A15_Remove\RemoveComment_WhiteSpaceBoth.xml");

            var comment = ((IEnumerable)doc.XPathEvaluate("//main/comment()")).Cast<XComment>().First()!;
            var wsBefore = comment.PreviousNode!;
            var wsAfter = comment.NextNode!;
            var nodeCount = doc.XPathSelectElement("//main")!.Nodes().Count();

            Shared.Patcher.PatchXml(doc, diff);

            Assert.IsNull(wsBefore.Document, "previous whitespace node was not removed");
            Assert.IsNull(wsAfter.Document, "following whitespace node was not removed");
            Assert.AreEqual(nodeCount - 3, doc.XPathSelectElement("//main")!.Nodes().Count());
        }
    }
}
