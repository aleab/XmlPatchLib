global using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Xml.Linq;
using Tizuby.XmlPatchLib;

namespace XmlPatchLibTests
{
    internal static class Shared
    {
        public static XmlPatcher Patcher { get; } = new XmlPatcher();

        public static XDocument GetTestSample(LoadOptions loadOptions = LoadOptions.None)
        {
            return XDocument.Load(@"TestData\test-sample.xml", loadOptions);
        }

        public static XDocument GetTestSampleWithNamespaces(LoadOptions loadOptions = LoadOptions.None)
        {
            return XDocument.Load(@"TestData\test-sample-with-namespaces.xml", loadOptions);
        }

        public static void TestWith<T>(T o, Action<T> tests)
        {
            tests.Invoke(o);
        }
    }
}
