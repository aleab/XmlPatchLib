using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Wmhelp.XPath2;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcher
    {
        // TODO: Instead of taking the string for the root name itself, use some type of extendable system to allow differing methods, such as <patch></patch> (RFC 7351). Default should always be <diff>. Some type of lookup table?
        public XmlPatcher(string rootElementName = "diff")
        {
            this.RootElementName = rootElementName;
        }

        protected string RootElementName { get; }

        /// <summary>
        ///     Attempts to patch the given XML document using the given diff xml document.
        /// </summary>
        /// <param name="originalDoc">The original XML document to patch.</param>
        /// <param name="diffDoc">The diff XML document containing the patch operations.</param>
        /// <param name="useBestEffort">Whether to continue with further patch operations if an error is encountered or not.</param>
        /// <returns>A list of encountered exceptions when useBestEffort is true.</returns>
        /// <exception cref="XmlException"></exception>
        /// <exception cref="XPathException"></exception>
        /// <exception cref="XPath2Exception"></exception>
        public IEnumerable<Exception> PatchXml(XDocument originalDoc, XDocument diffDoc, bool useBestEffort = false)
        {
            if (originalDoc == null)
                throw new ArgumentNullException(nameof(originalDoc));
            if (diffDoc == null)
                throw new ArgumentNullException(nameof(diffDoc));

            if (originalDoc.Root == null)
                throw new InvalidDataException("The original document's root is null");
            if (diffDoc.Root == null)
                throw new InvalidDataException("The patch document's root is null");

            // TODO: Use a validator on the diff doc. Any original doc is considered valid.
            // ^ So long as the roots match up, just append all children to the source.
            // For now, assume the structure is good.
            // First get a list of all the add/replace/remove elements.
            var addOperations = diffDoc.Root.Elements("add");
            var replaceOperations = diffDoc.Root.Elements("replace");
            var removeOperations = diffDoc.Root.Elements("remove");

            var exceptionList = new List<Exception>();

            var addNodeExceptions = this.RunOperation(this.PatchAddNode, originalDoc, addOperations, useBestEffort);
            var replaceNodeExceptions = this.RunOperation(this.PatchReplaceNode, originalDoc, replaceOperations, useBestEffort);
            var removeNodeExceptions = this.RunOperation(this.PatchRemoveNode, originalDoc, removeOperations, useBestEffort);

            exceptionList.AddRange(addNodeExceptions);
            exceptionList.AddRange(replaceNodeExceptions);
            exceptionList.AddRange(removeNodeExceptions);

            return exceptionList;
        }

        protected void PatchAddNode(XDocument originalDoc, XElement addElement)
        {
            var xpath = this.GetXPath(addElement);
            var targetElement = this.GetSingleTargetNode<XElement>(originalDoc, xpath);
            var typeAttribute = addElement.Attribute("type");

            // If there's a type attribute, it means the add operation is adding an attribute to the target element.
            if (typeAttribute != null)
                this.PatchAddAttribute(targetElement, typeAttribute, addElement);
            else
                this.PatchNormalAdd(targetElement, addElement);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected void PatchReplaceNode(XDocument originalDoc, XElement replaceElement)
        {
            var xpath = this.GetXPath(replaceElement);

            var targetXObject = this.GetSingleTargetNode<XObject>(originalDoc, xpath);

            if (targetXObject.NodeType == XmlNodeType.Attribute)
            {
                var attribute = targetXObject as XAttribute;
                attribute.SetValue(replaceElement.Value);
            }
            else
            {
                var node = targetXObject as XNode;
                node.ReplaceWith(replaceElement.Nodes());
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected void PatchRemoveNode(XDocument originalDoc, XElement removalElement)
        {
            // TODO: Support ws removal for sibling whitespace nodes.
            var xpath = this.GetXPath(removalElement);

            var targetXObject = this.GetSingleTargetNode<XObject>(originalDoc, xpath);

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

        private IEnumerable<Exception> RunOperation(Action<XDocument, XElement> operation, XDocument originalDoc, IEnumerable<XElement> operationElements, bool useBestEffort)
        {
            var result = new List<Exception>();
            foreach (var element in operationElements)
            {
                try
                {
                    operation.Invoke(originalDoc, element);
                }
                catch (Exception ex) when (ex is XmlException || ex is XPathException || ex is XPath2Exception)
                {
                    if (useBestEffort)
                        result.Add(ex);
                    else
                        throw;
                }
            }

            return result;
        }

        private string GetXPath(XElement element)
        {
            // TODO: Validate XPath by taking an IXPathValidator in the constructor? Not really a rush since invalid xpath should throw an exception when used.
            var xPathAttribute = element.Attribute("sel");

            if (xPathAttribute == null)
                throw new XmlException("sel attribute does not exist! All patch operations must include a 'sel' attribute! element:" + element);

            var result = xPathAttribute.Value;

            if (string.IsNullOrWhiteSpace(result))
                throw new XPathException("Invalid xpath. The value of the sel attribute was empty or all whitespace. element: " + element);

            return result;
        }

        private T GetSingleTargetNode<T>(XNode doc, string xpath) where T : XObject
        {
            var foundNodes = doc.XPath2Select<T>(xpath).ToList();

            if (foundNodes == null || foundNodes.Count == 0)
                throw new XPathException("The xpath provided did not correspond to any nodes. xpath:" + xpath);

            if (foundNodes.Count > 1)
                throw new XmlException("Invalid XPath for patching. Xpath returned multiple nodes. Xpath must return exactly one unambiguous node. xpath: " + xpath);

            return foundNodes.First();
        }

        /// <summary>
        ///     Handles adding a type attribute to an existing element.
        /// </summary>
        /// <param name="target">The target element to add the attribute to.</param>
        /// <param name="typeAttribute">The actual type attribute from target.</param>
        /// <param name="valueElement">The element containing the text value to add as the value of the new attribute.</param>
        private void PatchAddAttribute(XElement target, XAttribute typeAttribute, XContainer valueElement)
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
        private void PatchNormalAdd(XContainer targetElement, XElement valueElement)
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
                targetElement.AddAfterSelf(targetElement.Elements());
            else
            {
                // Invalid position, throw error.
                throw new XmlException("pos attribute is present, but has an illegal value for Add operation: " + valueElement);
            }
        }
    }
}
