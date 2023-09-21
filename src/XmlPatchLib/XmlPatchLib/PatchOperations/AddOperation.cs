using System;
using System.Diagnostics.CodeAnalysis;
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

        private static readonly Regex AttributeRegex = new Regex($"^@({Util.XmlQName})$");
        private static readonly Regex NsRegex = new Regex($"^namespace(?:::({Util.XmlNCName}))?$");

        private readonly Position _position;
        private readonly (Type, string) _type;

        public AddOperation(string sel, XElement operationNode) : base(sel, operationNode)
        {
            this._position = ParsePosition(operationNode.Attribute("pos")?.Value, this.OperationNode);
            this._type = ParseType(operationNode.Attribute("type")?.Value, this.OperationNode);
        }

        public override void Apply(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver = null)
        {
            var targetElement = xPathEvaluator.SelectSingle<XElement>(sourceDocument, this.XPathExpression, nsResolver);

            switch (this._type.Item1)
            {
                case Type.None:
                    PatchNormalAdd(targetElement, this.OperationNode, this._position);
                    break;
                case Type.Attribute:
                    PatchAddAttribute(targetElement, this.OperationNode, this._type.Item2);
                    break;
                case Type.Namespace:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(this._type.Item1), this._type.Item1, null);
            }
        }

        /// <summary>
        ///     Handles adding a new element to a target element.
        /// </summary>
        /// <param name="targetElement">The target to add to.</param>
        /// <param name="operationNode">The add element.</param>
        /// <exception cref="XmlException"></exception>
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void PatchNormalAdd(XElement targetElement, XElement operationNode, Position pos)
        {
            switch (pos)
            {
                case Position.Append:
                    targetElement.Add(operationNode.Elements());
                    break;
                case Position.Before:
                    targetElement.AddBeforeSelf(operationNode.Elements());
                    break;
                case Position.After:
                    targetElement.AddAfterSelf(operationNode.Elements());
                    break;
                case Position.Prepend:
                    targetElement.AddFirst(operationNode.Elements());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }

        /// <summary>
        ///     Handles adding a type attribute to an existing element.
        /// </summary>
        /// <param name="targetElement">The target element to add the attribute to.</param>
        /// <param name="operationNode">The element containing the text value to add as the value of the new attribute.</param>
        /// <param name="attributeName">The [qualified] name of the new attribute.</param>
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void PatchAddAttribute(XElement targetElement, XElement operationNode, string attributeName)
        {
            // There's a type included, which means it wants to add an attribute. Make sure the first non-comment element is a text element.
            var textNode = operationNode.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.Text);

            var attributeValue = textNode?.ToString()?.Trim() ?? "";
            var attribute = new XAttribute(attributeName, attributeValue);

            targetElement.Add(attribute);
        }

        private static Position ParsePosition(string pos, XElement context)
        {
            switch (pos)
            {
                case null:      return Position.Append;
                case "before":  return Position.Before;
                case "after":   return Position.After;
                case "prepend": return Position.Prepend;
                default:
                    throw new XmlPatcherParsingException($"Invalid <add> \"pos\" value: \"{pos}\"", context);
            }
        }

        private static (Type, string) ParseType(string type, XElement context)
        {
            switch (type)
            {
                case null:                                                 return (Type.None, null);
                case var _ when AttributeRegex.IsMatch(type):              return (Type.Attribute, type.Substring(1));
                case var _ when NsRegex.Match(type) is var m && m.Success: return (Type.Namespace, m.Groups[1].Value);
                default:
                    throw new XmlPatcherParsingException($"Invalid <add> \"type\" value: \"{type}\"", context);
            }
        }
    }
}
