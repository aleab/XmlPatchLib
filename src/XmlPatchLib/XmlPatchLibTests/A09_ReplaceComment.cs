using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    [TestClass]
    public class A09_ReplaceComment
    {
        [TestMethod]
        public void ExistingComment()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A09_Replace\ReplaceComment.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var res = doc.XPathEvaluate("//main/comment()");
            Assert.IsInstanceOfType<IEnumerable>(res);
            Shared.TestWith(((IEnumerable)res).Cast<XComment>().ToList(), comments =>
            {
                Assert.AreEqual(1, comments.Count);
                Assert.AreEqual(" New Comment Node ", comments[0].Value);
            });
        }

        [TestMethod]
        public void MissingComment_ShouldThrowException()
        {
            var doc = Shared.TestSample;
            var diff = XDocument.Load(@"TestData\A09_Replace\ReplaceComment_Missing.xml");

            Assert.ThrowsException<XPathException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
