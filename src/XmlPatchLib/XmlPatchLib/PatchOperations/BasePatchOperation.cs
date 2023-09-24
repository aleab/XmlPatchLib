﻿using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    public abstract class BasePatchOperation<T> : IPatchOperation where T : XObject
    {
        protected readonly XElement OperationNode;
        protected readonly IPatchOperationOptions Options;
        protected readonly string XPathExpression;

        protected BasePatchOperation(string sel, XElement operationNode, IPatchOperationOptions options)
        {
            this.OperationNode = operationNode;
            this.Options = options;
            this.XPathExpression = sel;
        }

        protected abstract void ApplyPatch(T target, IXmlNamespaceResolver nsResolver);

        /// <summary>
        ///     Apply the patch operation to the specified document.
        /// </summary>
        /// <param name="sourceDocument">The document targeted by the patch.</param>
        /// <param name="nsResolver">
        ///     Resolves prefixes used in the XPath queries to namespaces.<br/>
        ///     If null, a default one is built from the patch document this operation belongs to.
        /// </param>
        public void Apply(XDocument sourceDocument, IXmlNamespaceResolver nsResolver = null)
        {
            var xPathEvaluator = this.Options.XPathEvaluator ?? new DefaultXPathEvaluator();
            if (nsResolver == null)
                nsResolver = this.OperationNode.Document.GetNamespaceResolver();

            if (!this.Options.AllowMultiNodeSelectors)
            {
                var target = xPathEvaluator.SelectSingle<T>(sourceDocument, this.XPathExpression, nsResolver);
                this.ApplyPatch(target, nsResolver);
            }
            else
            {
                var targets = xPathEvaluator.SelectAll<T>(sourceDocument, this.XPathExpression, nsResolver);
                foreach (var target in targets)
                {
                    this.ApplyPatch(target, nsResolver);
                }
            }
        }
    }
}
