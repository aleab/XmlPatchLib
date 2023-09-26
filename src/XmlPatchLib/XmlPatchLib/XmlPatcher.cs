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
        protected XmlPatcherOptions Options { get; }

        /// <summary></summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentException"></exception>
        public XmlPatcher(XmlPatcherOptions options = null)
        {
            this.Options = new XmlPatcherOptions(options);

            if (this.Options.XPathEvaluator == null)
                this.Options.XPathEvaluator = new DefaultXPathEvaluator();

            if (!Utils.XmlNCName.IsMatch(this.Options.RootElementName))
                throw new ArgumentException($"\"{this.Options.RootElementName}\" is not a valid XML tag name.");
        }

        /// <summary>
        ///     Attempts to patch the given XML document using the given diff xml document.
        /// </summary>
        /// <param name="sourceDocument">The original XML document to patch.</param>
        /// <param name="patchDocument">The diff XML document containing the patch operations.</param>
        /// <returns>A list of encountered exceptions when useBestEffort is true.</returns>
        /// <exception cref="ArgumentNullException">One of the documents is null.</exception>
        /// <exception cref="XmlException">The root of one of the documents is null.</exception>
        /// <exception cref="XmlPatcherException"></exception>
        public IEnumerable<XmlPatcherError> PatchXml(XDocument sourceDocument, XDocument patchDocument)
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
            if (root.Name.LocalName != this.Options.RootElementName)
                throw new InvalidDiffFormatException($"The patch document's root is \"{root.Name.LocalName}\"; expected \"{this.Options.RootElementName}\"");

            // TODO: Use a validator on the diff doc. Any original doc is considered valid.

            var errors = new List<XmlPatcherError>();
            var patchNamespaceResolver = patchDocument.GetNamespaceResolver();

            foreach (var operationNode in root.Elements())
            {
                try
                {
                    var operation = PatchOperationsParser.Parse(operationNode, this.Options);
                    operation.Apply(sourceDocument, patchNamespaceResolver);
                }
                catch (Exception ex)
                {
                    if (!this.Options.UseBestEffort)
                        throw;
                    if (ex is XmlPatcherException || ex is XPathException || ex is InvalidOperationException)
                        errors.Add(new XmlPatcherError(operationNode, ex));
                }
            }

            return errors;
        }
    }
}
