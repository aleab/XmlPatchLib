using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcherOptions
    {
        public string RootElementName { get; set; } = "diff";

        /// <summary>
        ///     Whether to continue with further patch operations if an error is encountered or not.
        /// </summary>
        public bool UseBestEffort { get; set; }

        public IXPathEvaluator XPathEvaluator { get; set; }
    }
}
