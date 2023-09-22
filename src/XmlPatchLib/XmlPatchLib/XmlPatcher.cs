using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tizuby.XmlPatchLib.PatchOperations;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcher
    {
        protected string RootElementName { get; }
        protected bool UseBestEffort { get; }
        protected IXPathEvaluator XPathEvaluator { get; }

        /// <summary></summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentException"></exception>
        public XmlPatcher(XmlPatcherOptions options = null)
        {
            options = options ?? new XmlPatcherOptions();

            this.RootElementName = options.RootElementName;
            this.UseBestEffort = options.UseBestEffort;
            this.XPathEvaluator = options.XPathEvaluator ?? new DefaultXPathEvaluator();

            if (!Utils.XmlNCName.IsMatch(this.RootElementName))
                throw new ArgumentException($"\"{this.RootElementName}\" is not a valid XML tag name.");
        }

        /// <summary>
        ///     Attempts to patch the given XML document using the given diff xml document.
        /// </summary>
        /// <param name="sourceDocument">The original XML document to patch.</param>
        /// <param name="patchDocument">The diff XML document containing the patch operations.</param>
        /// <returns>A list of encountered exceptions when useBestEffort is true.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="XmlException"></exception>
        /// <exception cref="XmlPatcherParsingException"></exception>
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

            var root = patchDocument.Root;
            if (root.Name.LocalName != this.RootElementName)
                throw new XmlPatcherParsingException($"The patch document's root is \"{root.Name.LocalName}\"; expected \"{this.RootElementName}\"", root);

            // TODO: Use a validator on the diff doc. Any original doc is considered valid.

            var exceptionList = new List<Exception>();

            var sourceNamespaceResolver = sourceDocument.GetNamespaceResolver();
            var patchNamespaceResolver = patchDocument.GetNamespaceResolver();

            foreach (var operationNode in root.Elements())
            {
                var operation = PatchOperation.Parse(operationNode);

                try
                {
                    operation?.Apply(sourceDocument, this.XPathEvaluator, patchNamespaceResolver);
                }
                catch (Exception ex)
                {
                    if (!this.UseBestEffort)
                        throw;
                    if (ex is XmlPatcherParsingException || ex is XPathException || ex is InvalidOperationException)
                        exceptionList.Add(ex);
                }
            }

            return exceptionList;
        }
    }
}
