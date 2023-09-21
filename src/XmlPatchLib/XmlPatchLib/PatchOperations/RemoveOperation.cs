using System;
using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal sealed class RemoveOperation : PatchOperation
    {
        public enum Whitespace { None, Before, After, Both }

        private readonly Whitespace _whitespace;

        public RemoveOperation(string sel, XElement operationNode) : base(sel, operationNode)
        {
            this._whitespace = ParseWhitespace(operationNode.Attribute("ws")?.Value, this.OperationNode);
        }

        public override void Apply(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver = null)
        {
            // TODO: Support ws removal for sibling whitespace nodes.
            var target = xPathEvaluator.SelectSingle<XObject>(sourceDocument, this.XPathExpression, nsResolver);

            switch (target)
            {
                case var _ when target is XAttribute attribute:
                    attribute.Remove();
                    break;

                case var _ when target is XNode node:
                    node.Remove();
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected target node type: {target.NodeType}.");
            }
        }

        private static Whitespace ParseWhitespace(string ws, XElement context)
        {
            switch (ws)
            {
                case null:     return Whitespace.None;
                case "before": return Whitespace.Before;
                case "after":  return Whitespace.After;
                case "both":   return Whitespace.Both;
                default:
                    throw new XmlPatcherParsingException($"Invalid <remove> \"ws\" value: \"{ws}\"", context);
            }
        }
    }
}
