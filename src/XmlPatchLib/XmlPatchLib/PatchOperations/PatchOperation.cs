using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    public abstract class PatchOperation
    {
        protected readonly XElement OperationNode;
        protected readonly string XPathExpression;

        protected PatchOperation(string sel, XElement operationNode)
        {
            this.OperationNode = operationNode;
            this.XPathExpression = sel;
        }

        /// <summary>
        ///     Apply the patch operation to the specified document, using the provided <see cref="IXPathEvaluator"/> and
        ///     <see cref="IXmlNamespaceResolver"/> to evaluate XPath expressions.
        /// </summary>
        /// <param name="sourceDocument">The document targeted by the patch.</param>
        /// <param name="xPathEvaluator">Evaluates XPath expressions.</param>
        /// <param name="nsResolver">
        ///     Resolves prefixes used in the XPath queries to namespaces.<br/>
        ///     If null, a default one is built from the patch document this operation belongs to.
        /// </param>
        public void Apply(XDocument sourceDocument, IXPathEvaluator xPathEvaluator = null, IXmlNamespaceResolver nsResolver = null)
        {
            this.ApplyPatch(sourceDocument, xPathEvaluator ?? new DefaultXPathEvaluator(), nsResolver ?? this.OperationNode.Document.GetNamespaceResolver());
        }

        protected abstract void ApplyPatch(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver);

        public static PatchOperation Parse(XElement operationNode)
        {
            var sel = ParseXPath(operationNode);
            switch (operationNode.Name.LocalName)
            {
                case "add":     return new AddOperation(sel, operationNode);
                case "replace": return new ReplaceOperation(sel, operationNode);
                case "remove":  return new RemoveOperation(sel, operationNode);
                default:        return null;
            }
        }

        private static string ParseXPath(XElement operationNode)
        {
            var selAttribute = operationNode.Attribute("sel");
            if (selAttribute == null || string.IsNullOrWhiteSpace(selAttribute.Value))
                throw new XmlPatcherParsingException("\"sel\" attribute is required and must have a non-empty value!", operationNode);

            return selAttribute.Value;
        }
    }
}
