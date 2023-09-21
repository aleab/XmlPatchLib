using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal sealed class ReplaceOperation : PatchOperation
    {
        public ReplaceOperation(string sel, XElement operationNode) : base(sel, operationNode) { }

        public override void Apply(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver = null)
        {
            var content = this.OperationNode.Nodes().ToList();
            var target = xPathEvaluator.SelectSingle<XObject>(sourceDocument, this.XPathExpression, nsResolver);

            var isValidTextContent = content.Count == 0 || (content.Count == 1 && content[0].NodeType == XmlNodeType.Text);
            switch (target)
            {
                case var _ when target is XAttribute attribute:
                    if (!isValidTextContent)
                        throw new InvalidOperationException("A \"replace\" operation targeting an attribute may have at most one node and it MUST be text.");
                    attribute.SetValue(this.OperationNode.Value);
                    break;

                case var _ when target is XText textNode:
                    if (!isValidTextContent)
                        throw new InvalidOperationException("A \"replace\" operation targeting a text node may have at most one node and it MUST be text.");
                    textNode.Value = this.OperationNode.Value;
                    break;

                case var _ when target is XNode node:
                    node.ReplaceWith(this.OperationNode.Nodes());
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected target node type: {target.NodeType}.");
            }
        }
    }
}
