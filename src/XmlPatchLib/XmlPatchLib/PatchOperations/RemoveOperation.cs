using System;
using System.Xml;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal sealed class RemoveOperation : BasePatchOperation<XObject>
    {
        public enum Whitespace { None, Before, After, Both }

        private readonly Whitespace _whitespace;

        public RemoveOperation(string sel, XElement operationNode) : base(sel, operationNode)
        {
            this._whitespace = ParseWhitespace(operationNode.Attribute("ws")?.Value);
        }

        protected override void ApplyPatch(XObject target, IXmlNamespaceResolver nsResolver)
        {
            switch (target)
            {
                case var _ when target is XAttribute attribute:
                    attribute.Remove();
                    break;

                case var _ when target is XNode node:
                    if (node.NodeType != XmlNodeType.Text)
                        RemoveWhitespaceNodes(node, this._whitespace);
                    node.Remove();
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected target node type: {target.NodeType}.");
            }
        }

        private static void RemoveWhitespaceNodes(XNode node, Whitespace ws)
        {
            // From section 4.5 of the RFC:
            //    When removing an element, a comment, or a processing instruction node that has immediate preceding and following sibling text nodes without
            //    the 'ws' directive, the content of these two text nodes MUST be combined together. The latter text node thus disappears from the document.

            if (ws == Whitespace.None)
            {
                if (node.PreviousNode.IsWhitespace() && node.NextNode.IsWhitespace())
                    node.NextNode.Remove();
                return;
            }

            if ((ws == Whitespace.Before || ws == Whitespace.Both) && node.PreviousNode.IsWhitespace())
                node.PreviousNode.Remove();
            if ((ws == Whitespace.After || ws == Whitespace.Both) && node.NextNode.IsWhitespace())
                node.NextNode.Remove();
        }

        private static Whitespace ParseWhitespace(string ws)
        {
            switch (ws)
            {
                case null:     return Whitespace.None;
                case "before": return Whitespace.Before;
                case "after":  return Whitespace.After;
                case "both":   return Whitespace.Both;
                default:
                    throw new InvalidAttributeValueException("ws", ws, new[] { "before", "after", "both" }, true);
            }
        }
    }
}
