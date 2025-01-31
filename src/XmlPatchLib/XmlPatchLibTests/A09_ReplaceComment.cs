﻿using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.9
    /// </summary>
    [TestClass]
    [TestCategory("<replace>")]
    public class A09_ReplaceComment
    {
        [TestMethod]
        public void ExistingComment()
        {
            var doc = Shared.GetTestSample();
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
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A09_Replace\ReplaceComment_Missing.xml");

            Assert.ThrowsException<UnlocatedNodeException>(() => Shared.Patcher.PatchXml(doc, diff));
        }

        [TestMethod]
        public void WithDifferentNodeType_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A09_Replace\ReplaceComment_WithDifferentNodeType.xml");

            Assert.ThrowsException<InvalidNodeTypeException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
