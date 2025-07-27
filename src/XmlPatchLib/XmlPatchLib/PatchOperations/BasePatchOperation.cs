using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib.ProcessingInstructions;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal interface IPatchOperation
    {
        void Apply(XDocument sourceDocument, IXmlNamespaceResolver nsResolver = null);
    }

    public abstract class BasePatchOperation<T> : IPatchOperation where T : XObject
    {
        protected readonly XElement operationNode;
        protected readonly IPatchOperationOptions options;
        protected readonly string xPathExpression;

        protected BasePatchOperation(string sel, XElement operationNode, IPatchOperationOptions options)
        {
            this.operationNode   = operationNode;
            this.options         = options;
            this.xPathExpression = sel;
        }

        protected abstract void ApplyPatch(T target, IXmlNamespaceResolver nsResolver);

        private void ExecuteProcessingInstructions(XDocument sourceDocument, IXmlNamespaceResolver nsResolver)
        {
            var nodes = ((IEnumerable<object>)this.operationNode.XPathEvaluate("//processing-instruction()")).Cast<XProcessingInstruction>();
            foreach (var node in nodes)
            {
                var pi = ProcessingInstructionsParser.Parse(node, this.options);
                pi?.Execute(sourceDocument, nsResolver);
            }
        }

        /// <summary>
        ///     Apply the patch operation to the specified document.
        /// </summary>
        /// <param name="sourceDocument">The document targeted by the patch.</param>
        /// <param name="nsResolver">
        ///     Resolves prefixes used in the XPath queries to namespaces.<br/>
        ///     If null, a default one is built from the patch document this operation belongs to.
        /// </param>
        public void Apply(XDocument sourceDocument, IXmlNamespaceResolver nsResolver = null)
        {
            var xPathEvaluator = this.options.XPathEvaluator ?? new DefaultXPathEvaluator();
            if (nsResolver == null)
                nsResolver = this.operationNode.Document.GetNamespaceManager();

            if (this.options.UseProcessingInstrutions)
                this.ExecuteProcessingInstructions(sourceDocument, nsResolver);

            try
            {
                if (!this.options.AllowMultiNodeSelectors)
                {
                    var target = xPathEvaluator.SelectSingle<T>(sourceDocument, this.xPathExpression, nsResolver);
                    this.ApplyPatch(target, nsResolver);
                }
                else
                {
                    var targets = xPathEvaluator.SelectAll<T>(sourceDocument, this.xPathExpression, nsResolver);
                    foreach (var target in targets)
                        this.ApplyPatch(target, nsResolver);
                }
            }
            catch (UnlocatedNodeException ex)
            {
                ex.Source = $"<{this.operationNode.Name} {string.Join(" ", this.operationNode.Attributes().Select(x => x.ToString()))}>";
                throw;
            }
        }
    }
}
