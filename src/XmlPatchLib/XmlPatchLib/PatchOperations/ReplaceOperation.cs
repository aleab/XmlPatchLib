using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal sealed class ReplaceOperation : BasePatchOperation<XObject>
    {
        public ReplaceOperation(string sel, XElement operationNode) : base(sel, operationNode) { }

        protected override void ApplyPatch(XObject target, IXmlNamespaceResolver nsResolver)
        {
            this.CheckNodeTypeAndContent(target);

            switch (target)
            {
                case var _ when target is XAttribute attribute:
                    attribute.SetValue(this.OperationNode.Value);
                    break;

                case var _ when target is XText textNode:
                    textNode.Value = this.OperationNode.Value;
                    break;

                case var _ when target is XNode node:
                    node.ReplaceWith(this.OperationNode.Nodes());
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected target node type: {target.NodeType}.");
            }
        }

        private void CheckNodeTypeAndContent(XObject target)
        {
            var content = this.OperationNode.Nodes().ToList();

            switch (target.NodeType)
            {
                case XmlNodeType.Attribute:
                case XmlNodeType.Text:
                    if (content.Count > 1 || (content.Count == 1 && content[0].NodeType != XmlNodeType.Text))
                        throw new InvalidNodeTypeException("A <replace> operation targeting an attribute, namespace or text node may have at most one node and it MUST be text.");
                    break;

                default:
                    var type = target.GetType().Name;
                    if (content.Count != 1)
                        throw new InvalidNodeTypeException($"A <replace> operation targeting a \"{type}\" MUST have exactly one node.");
                    if (content[0].NodeType != target.NodeType)
                        throw new InvalidNodeTypeException($"A <replace> operation targeting a \"{type}\" MUST have exactly one node of the same type.");
                    break;
            }
        }
    }
}
