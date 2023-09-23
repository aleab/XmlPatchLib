using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal sealed class AddOperation : PatchOperation
    {
        public enum Position { Append, Before, After, Prepend }

        public enum Type { None, Attribute, Namespace }

        private static readonly Regex NsRegex = new Regex($"^namespace(?:::({Utils.XmlNCName}))?$");

        private readonly Position _position;
        private readonly (Type, string) _type;

        public AddOperation(string sel, XElement operationNode) : base(sel, operationNode)
        {
            this._position = ParsePosition(operationNode.Attribute("pos")?.Value);
            this._type = ParseType(operationNode.Attribute("type")?.Value);
        }

        protected override void ApplyPatch(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver)
        {
            var targetElement = xPathEvaluator.SelectSingle<XElement>(sourceDocument, this.XPathExpression, nsResolver);

            switch (this._type.Item1)
            {
                case Type.None:
                    this.AddNodes(targetElement);
                    break;

                case Type.Attribute:
                    this.AddAttribute(targetElement, Utils.GetXName(this._type.Item2, nsResolver));
                    break;

                case Type.Namespace:
                    this.AddNamespace(targetElement, this._type.Item2);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(this._type.Item1), this._type.Item1, null);
            }
        }

        private void AddNodes(XContainer target)
        {
            var nodes = this.OperationNode.Nodes();
            switch (this._position)
            {
                case Position.Append:
                    target.Add(nodes);
                    break;
                case Position.Before:
                    target.AddBeforeSelf(nodes);
                    break;
                case Position.After:
                    target.AddAfterSelf(nodes);
                    break;
                case Position.Prepend:
                    target.AddFirst(nodes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this._position), this._position, null);
            }
        }

        private void AddAttribute(XElement targetElement, XName attributeName, bool isNamespace = false)
        {
            var content = this.OperationNode.Nodes().ToList();

            var isValidTextContent = content.Count == 0 || (content.Count == 1 && content[0].NodeType == XmlNodeType.Text);
            if (!isValidTextContent)
                throw new InvalidPatchDirectiveException($"An <add> operation targeting {(isNamespace ? "a namespace" : "an attribute")} may have at most one node and it MUST be text.");

            if (targetElement.Attribute(attributeName) != null)
                throw new InvalidPatchDirectiveException($"Target element already has {(isNamespace ? "namespace" : "attribute")} \"{attributeName}\"");

            var attributeValue = content.FirstOrDefault()?.ToString()?.Trim() ?? string.Empty;
            targetElement.SetAttributeValue(attributeName, attributeValue);
        }

        private void AddNamespace(XElement targetElement, string nsPrefix)
        {
            var attributeName = string.IsNullOrWhiteSpace(nsPrefix) ? XName.Get("xmlns", "") : XName.Get(nsPrefix, "http://www.w3.org/2000/xmlns/");
            this.AddAttribute(targetElement, attributeName, true);
        }

        private static Position ParsePosition(string pos)
        {
            switch (pos)
            {
                case null:      return Position.Append;
                case "before":  return Position.Before;
                case "after":   return Position.After;
                case "prepend": return Position.Prepend;
                default:
                    throw new InvalidAttributeValueException("pos", pos, new[] { "before", "after", "prepend" }, true);
            }
        }

        private static (Type, string) ParseType(string type)
        {
            switch (type)
            {
                case null:                                                 return (Type.None, null);
                case var _ when type.StartsWith("@"):                      return (Type.Attribute, type.Substring(1));
                case var _ when NsRegex.Match(type) is var m && m.Success: return (Type.Namespace, m.Groups[1].Value);
                default:
                    throw new InvalidAttributeValueException("type", type, new[] { "@QNAME", "namespace(::NCNAME)?" });
            }
        }
    }
}
