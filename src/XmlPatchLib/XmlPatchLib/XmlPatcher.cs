using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcher
    {
        protected string RootElementName { get; }
        protected bool UseBestEffort { get; }
        protected IXPathEvaluator XPathEvaluator { get; }

        // TODO: Instead of taking the string for the root name itself, use some type of extendable system to allow differing methods, such as <patch></patch> (RFC 7351). Default should always be <diff>. Some type of lookup table?
        public XmlPatcher(XmlPatcherOptions options = null)
        {
            options = options ?? new XmlPatcherOptions();

            this.RootElementName = options.RootElementName;
            this.UseBestEffort = options.UseBestEffort;
            this.XPathEvaluator = options.XPathEvaluator ?? new DefaultXPathEvaluator();
        }

        /// <summary>
        ///     Attempts to patch the given XML document using the given diff xml document.
        /// </summary>
        /// <param name="sourceDocument">The original XML document to patch.</param>
        /// <param name="patchDocument">The diff XML document containing the patch operations.</param>
        /// <returns>A list of encountered exceptions when useBestEffort is true.</returns>
        /// <exception cref="XmlException"></exception>
        /// <exception cref="XPathException"></exception>
        /// <exception cref="XPath2Exception"></exception>
        public IEnumerable<Exception> PatchXml(XDocument sourceDocument, XDocument patchDocument)
        {
            if (sourceDocument == null)
                throw new ArgumentNullException(nameof(sourceDocument));
            if (patchDocument == null)
                throw new ArgumentNullException(nameof(patchDocument));

            if (sourceDocument.Root == null)
                throw new XmlException("The source document's root is null");
            if (patchDocument.Root == null)
                throw new XmlException("The patch document's root is null");

            if (patchDocument.Root.Name.LocalName != this.RootElementName)
                throw new XmlException($"The patch document's root is \"{patchDocument.Root.Name.LocalName}\"; expected \"{this.RootElementName}\"");

            // TODO: Use a validator on the diff doc. Any original doc is considered valid.

            var exceptionList = new List<Exception>();

            foreach (var operationNode in patchDocument.Root.Elements())
            {
                Action<XDocument, XElement> operation = null;
                switch (operationNode.Name.LocalName)
                {
                    case "add":
                        operation = this.PatchAdd;
                        break;
                    case "replace":
                        operation = this.PatchReplace;
                        break;
                    case "remove":
                        operation = this.PatchRemove;
                        break;
                }

                if (operation != null)
                    this.RunOperation(operation, sourceDocument, operationNode, exceptionList);
            }

            return exceptionList;
        }

        private void RunOperation(Action<XDocument, XElement> operation, XDocument document, XElement operationNode, ICollection<Exception> exceptionList)
        {
            try
            {
                operation.Invoke(document, operationNode);
            }
            catch (Exception ex) when (ex is XmlException || ex is XPathException || ex is InvalidOperationException)
            {
                if (this.UseBestEffort)
                    exceptionList.Add(ex);
                else
                    throw;
            }
        }

        protected void PatchAdd(XDocument sourceDocument, XElement operationNode)
        {
            var xpath = GetXPath(operationNode);
            var targetElement = this.XPathEvaluator.SelectSingle<XElement>(sourceDocument, xpath);
            var typeAttribute = operationNode.Attribute("type");

            // If there's a type attribute, it means the add operation is adding an attribute to the target element.
            if (typeAttribute != null)
                PatchAddAttribute(targetElement, typeAttribute, operationNode);
            else
                PatchNormalAdd(targetElement, operationNode);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "This method is expected to throw exceptions if the target node is not found")]
        protected void PatchReplace(XDocument sourceDocument, XElement operationNode)
        {
            var xpath = GetXPath(operationNode);
            var targetXObject = this.XPathEvaluator.SelectSingle<XObject>(sourceDocument, xpath);

            if (targetXObject.NodeType == XmlNodeType.Attribute)
            {
                var attribute = targetXObject as XAttribute;
                attribute.SetValue(operationNode.Value);
            }
            else
            {
                var node = targetXObject as XNode;
                node.ReplaceWith(operationNode.Nodes());
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "This method is expected to throw exceptions if the target node is not found")]
        protected void PatchRemove(XDocument sourceDocument, XElement operationNode)
        {
            // TODO: Support ws removal for sibling whitespace nodes.
            var xpath = GetXPath(operationNode);
            var targetXObject = this.XPathEvaluator.SelectSingle<XObject>(sourceDocument, xpath);

            if (targetXObject.NodeType == XmlNodeType.Attribute)
            {
                var attribute = targetXObject as XAttribute;
                attribute.Remove();
            }
            else
            {
                var node = targetXObject as XNode;
                node.Remove();
            }
        }

        /// <summary>
        ///     Handles adding a type attribute to an existing element.
        /// </summary>
        /// <param name="target">The target element to add the attribute to.</param>
        /// <param name="typeAttribute">The actual type attribute from target.</param>
        /// <param name="valueElement">The element containing the text value to add as the value of the new attribute.</param>
        private static void PatchAddAttribute(XElement target, XAttribute typeAttribute, XContainer valueElement)
        {
            // First, get the attribute name as that will need to be added to the target element.
            var attributeName = typeAttribute.Value[0] == '@' ? typeAttribute.Value.Remove(0, 1) : typeAttribute.Value;

            // There's a type included, which means it wants to add an attribute. Make sure the first non-comment element is a text element.
            var valueNode = valueElement.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.Text);

            var attributeValue = valueNode == null ? "" : valueNode.ToString().Trim();
            var attribute = new XAttribute(attributeName, attributeValue);

            // Finally, add the the new attribute and its value.
            target.Add(attribute);
        }

        /// <summary>
        ///     Handles adding a new element to a target element.
        /// </summary>
        /// <param name="targetElement">The target to add to.</param>
        /// <param name="valueElement">The add element.</param>
        /// <exception cref="XmlException"></exception>
        private static void PatchNormalAdd(XContainer targetElement, XElement valueElement)
        {
            // Otherwise we're either going to be adding it as a child, or adding it depending on "pos".
            var positionAttribute = valueElement.Attribute("pos");

            if (positionAttribute == null || string.IsNullOrWhiteSpace(positionAttribute.Value))
                targetElement.Add(valueElement.Elements());
            else if (positionAttribute.Value.Equals("prepend", StringComparison.OrdinalIgnoreCase))
                targetElement.AddFirst(valueElement.Elements());
            else if (positionAttribute.Value.Equals("before", StringComparison.OrdinalIgnoreCase))
                targetElement.AddBeforeSelf(valueElement.Elements());
            else if (positionAttribute.Value.Equals("after", StringComparison.OrdinalIgnoreCase))
                targetElement.AddAfterSelf(valueElement.Elements());
            else
            {
                // Invalid position, throw error.
                throw new XmlException("pos attribute is present, but has an illegal value for Add operation: " + valueElement);
            }
        }

        /// <summary>
        ///     Get the XPath string from the <i>sel</i> attribute.
        /// </summary>
        /// <exception cref="XmlException"></exception>
        /// <exception cref="XPathException"></exception>
        private static string GetXPath(XElement element)
        {
            // TODO: Validate XPath by taking an IXPathValidator in the constructor? Not really a rush since invalid xpath should throw an exception when used.
            var sel = element.Attribute("sel");
            if (sel == null)
                throw new XmlException($"\"sel\" attribute does not exist! All patch operations must include a \"sel\" attribute! Element: {XmlToStringHelper.Head(element)}");

            var value = sel.Value;
            if (string.IsNullOrWhiteSpace(value))
                throw new XPathException($"Invalid XPath. The value of the \"sel\" attribute was empty or all whitespace. Element: {XmlToStringHelper.Head(element)}");

            return value;
        }
    }
}
