﻿using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.6
    /// </summary>
    [TestClass]
    public class A06_ReplaceElement
    {
        [TestMethod]
        public void ExistingElement()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var child1 = doc.XPathSelectElement("//main/child[@id='1']");
            Assert.IsNotNull(child1);
            Assert.AreEqual("Replaced test!", child1.Value);
        }

        [TestMethod]
        public void MissingElement_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A06_Replace\ReplaceElement_Missing.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));

            var child1 = doc.XPathSelectElement("//main/child[@id='3']");
            Assert.IsNull(child1);
        }
    }
}
