using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcherOptions
    {
        /// <summary>
        ///     The xs:NCName of the patch document's root element.
        /// </summary>
        public string RootElementName { get; set; } = "diff";

        /// <summary>
        ///     Whether to continue with further patch operations if an error is encountered or not.
        /// </summary>
        public bool UseBestEffort { get; set; }

        public IXPathEvaluator XPathEvaluator { get; set; }

        // TODO: AllowMultiNodeSelectors: IXPathEvaluator.SelectAll<T>() rather than IXPathEvaluator.SelectSingle<T>()
        //       Section 4.1 of the RFC states that
        //          "The 'sel' value is used to locate a single unique target node from the target XML document."
        //          "it is an error condition if multiple nodes are found during the evaluation of this selector value."

        // TODO: DisableReplaceNodeTypeChecks
        //       Section 4.4 of the RFC states that
        //          "If the located target node is an element, a comment or a processing instruction, then the child of the <replace> element MUST also be of the same type"
        //          "the <replace> element MUST have text content or it MAY be empty when replacing [...] a text node content"
    }
}
