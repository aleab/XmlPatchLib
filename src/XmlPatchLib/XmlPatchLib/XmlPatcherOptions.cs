using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Options to use with an <see cref="XmlPatcher"/>.<br/>
    ///     These options allow to use a custom <see cref="IXPathEvaluator"/> or enable behaviours and features not normally allowed by the spec.
    /// </summary>
    public class XmlPatcherOptions : IPatchOperationOptions
    {
        public IXPathEvaluator XPathEvaluator { get; set; }

        /// <summary>
        ///     The xs:NCName of the patch document's root element.
        /// </summary>
        public string RootElementName { get; set; } = "diff";

        /// <summary>
        ///     Whether to continue with further patch operations if an error is encountered or not.
        /// </summary>
        public bool UseBestEffort { get; set; }

        /// <summary>
        ///     Whether to allow the 'sel' attribute to select multiple nodes or not.<br/><br/>
        ///     The default behaviour (as per section 4.1 of the RFC) does not allow multi-node selectors and throws an
        ///     <see cref="UnlocatedNodeException"/> if any XPath query returns multiple nodes or none at all.
        /// </summary>
        public bool AllowMultiNodeSelectors { get; set; }

        /// <summary>
        ///     Whether or not to disable the following restrictions on <c>&lt;replace&gt;</c> operations.<br/><br/>
        ///     &#x2022; A <c>&lt;replace&gt;</c> operation MUST have exactly one node; it MAY be empty when replacing a text node.<br/>
        ///     &#x2022; A <c>&lt;replace&gt;</c> operation MUST replace a target node with a node of the same type.
        /// </summary>
        public bool DisableReplaceRestrictions { get; set; }

        /// <summary>
        ///     Whether to try executing processing instructions encountered in the diff document or not.
        /// </summary>
        public bool UseProcessingInstrutions { get; set; }

        public XmlPatcherOptions() { }

        internal XmlPatcherOptions(XmlPatcherOptions options)
        {
            if (options != null)
            {
                this.XPathEvaluator = options.XPathEvaluator;
                this.RootElementName = options.RootElementName;
                this.UseBestEffort = options.UseBestEffort;

                this.AllowMultiNodeSelectors = options.AllowMultiNodeSelectors;
                this.DisableReplaceRestrictions = options.DisableReplaceRestrictions;
                this.UseProcessingInstrutions = options.UseProcessingInstrutions;
            }
        }
    }

    public interface IPatchOperationOptions
    {
        IXPathEvaluator XPathEvaluator { get; }
        bool AllowMultiNodeSelectors { get; }
        bool DisableReplaceRestrictions { get; }
        bool UseProcessingInstrutions { get; }
    }
}
