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

        public abstract void Apply(XDocument sourceDocument, IXPathEvaluator xPathEvaluator, IXmlNamespaceResolver nsResolver = null);

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
