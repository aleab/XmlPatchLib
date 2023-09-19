using System.Xml.Linq;
using Tizuby.XmlPatchLib;

namespace XmlPatchLibTests
{
    internal static class Shared
    {
        public static XmlPatcher Patcher { get; } = new XmlPatcher();
        public static XDocument TestSample => XDocument.Load(@"TestData\test-sample.xml");
        public static XDocument TestSampleWithNamespaces => XDocument.Load(@"TestData\test-sample-with-namespaces.xml");

        public static void TestWith<T>(T o, Action<T> tests)
        {
            tests.Invoke(o);
        }
    }
}
