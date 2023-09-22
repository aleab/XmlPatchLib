﻿using System.Xml.Linq;
using Tizuby.XmlPatchLib;

// ReSharper disable InconsistentNaming

namespace XmlPatchLibTests
{
    /// <summary>
    ///     https://datatracker.ietf.org/doc/html/rfc5261#appendix-A.14
    /// </summary>
    [TestClass]
    public class A14_RemoveNamespaceDeclaration
    {
        [TestMethod]
        public void PrefixedNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.Root!.GetNamespaceMap();
            Assert.IsNull(nsmap["x"]);
        }

        [TestMethod]
        public void EmptyNamespace()
        {
            var doc = Shared.GetTestSampleWithNamespaces();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration_Empty.xml");

            Shared.Patcher.PatchXml(doc, diff);

            var nsmap = doc.Root!.GetNamespaceMap();
            Assert.IsNull(nsmap[""]);
        }

        [TestMethod]
        public void MissingNamespace_ShouldThrowException()
        {
            var doc = Shared.GetTestSample();
            var diff = XDocument.Load(@"TestData\A14_Remove\RemoveNamespaceDeclaration.xml");

            Assert.ThrowsException<InvalidOperationException>(() => Shared.Patcher.PatchXml(doc, diff));
        }
    }
}
