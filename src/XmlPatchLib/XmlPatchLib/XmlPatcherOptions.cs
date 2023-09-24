using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcherOptions : IPatchOperationOptions
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

        /// <summary>
        ///     Whether to allow the 'sel' attribute to select multiple nodes or not.<br/><br/>
        ///     The default behaviour (as per section 4.1 of the RFC) does not allow multi-node selectors and throws an
        ///     <see cref="UnlocatedNodeException"/> if any XPath query returns multiple nodes or none at all.
        /// </summary>
        public bool AllowMultiNodeSelectors { get; set; }

        // TODO: DisableReplaceRestrictions
        //       Section 4.4 of the RFC states that
        //          "If the located target node is an element, a comment or a processing instruction, then the child of the <replace> element MUST also be of the same type"
        //          "the <replace> element MUST have text content or it MAY be empty when replacing [...] a text node content"

        public XmlPatcherOptions() { }

        internal XmlPatcherOptions(XmlPatcherOptions options)
        {
            if (options != null)
            {
                this.RootElementName = options.RootElementName;
                this.UseBestEffort = options.UseBestEffort;
                this.XPathEvaluator = options.XPathEvaluator;
                this.AllowMultiNodeSelectors = options.AllowMultiNodeSelectors;
            }
        }
    }

    public interface IPatchOperationOptions
    {
        IXPathEvaluator XPathEvaluator { get; }
        bool AllowMultiNodeSelectors { get; }
    }
}
