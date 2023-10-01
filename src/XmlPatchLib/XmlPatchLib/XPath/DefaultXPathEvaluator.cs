using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Tizuby.XmlPatchLib.XPath
{
    /// <summary>
    ///     Implementation of <see cref="IXPathEvaluator"/> that uses <see cref="System.Xml.XPath"/>.
    /// </summary>
    internal class DefaultXPathEvaluator : IXPathEvaluator
    {
        /// <summary>
        ///     Returns the only <see cref="XObject"/> matching the provided XPath expression,
        ///     resolving namespace prefixes using the specified <see cref="IXmlNamespaceResolver"/>.<br/>
        ///     Throws an exception if the XPath expression matches multiple nodes.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="XObject"/></typeparam>
        /// <param name="container">The <see cref="XContainer"/> on which to evaluate the XPath expression.</param>
        /// <param name="xpath">The XPath expression.</param>
        /// <param name="resolver">An <see cref="IXmlNamespaceResolver"/> for the namespace prefixes in the XPath expression.</param>
        /// <returns>A T-typed <see cref="XObject"/>.</returns>
        /// <exception cref="UnlocatedNodeException">The XPath expression could not uniquely locate the requested node.</exception>
        /// <exception cref="XPathException">The XPath expression was not well-formed.</exception>
        /// <exception cref="InvalidOperationException">The XPath expression selected an invalid or illegal node.</exception>
        public T SelectSingle<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject
        {
            var result = container.XPathEvaluate(xpath, resolver);
            if (result is IEnumerable<object> nodes)
            {
                var r = nodes.Where(node => node is T).Cast<T>().ToList();
                if (r.Count == 0)
                    throw new UnlocatedNodeException("XPath evaluation returned no matching nodes.");
                if (r.Count > 1)
                    throw new UnlocatedNodeException("XPath evaluation returned multiple matching nodes.");
                return r[0];
            }

            throw new InvalidOperationException($"Unexpected evaluation result type: {result.GetType()}");
        }

        /// <summary>
        ///     Returns the only <see cref="XObject"/> matching the provided XPath expression or null if it matches none,
        ///     resolving namespace prefixes using the specified <see cref="IXmlNamespaceResolver"/>.<br/>
        ///     Throws an exception if the XPath expression matches multiple nodes.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="XObject"/></typeparam>
        /// <param name="container">The <see cref="XContainer"/> on which to evaluate the XPath expression.</param>
        /// <param name="xpath">The XPath expression.</param>
        /// <param name="resolver">An <see cref="IXmlNamespaceResolver"/> for the namespace prefixes in the XPath expression.</param>
        /// <returns>A T-typed <see cref="XObject"/>.</returns>
        /// <exception cref="UnlocatedNodeException">The XPath expression selected multiple nodes.</exception>
        /// <exception cref="XPathException">The XPath expression was not well-formed.</exception>
        /// <exception cref="InvalidOperationException">The XPath expression selected an invalid or illegal node.</exception>
        public T SelectSingleOrDefault<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject
        {
            var result = container.XPathEvaluate(xpath, resolver);
            if (result is IEnumerable<object> nodes)
            {
                var r = nodes.Where(node => node is T).Cast<T>().ToList();
                if (r.Count == 0)
                    return null;
                if (r.Count > 1)
                    throw new UnlocatedNodeException("XPath evaluation returned multiple matching nodes.");
                return r[0];
            }

            throw new InvalidOperationException($"Unexpected evaluation result type: {result.GetType()}");
        }

        /// <summary>
        ///     Returns all <see cref="XObject"/>s matching the provided XPath expression,
        ///     resolving namespace prefixes using the specified <see cref="IXmlNamespaceResolver"/>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="XObject"/></typeparam>
        /// <param name="container">The <see cref="XContainer"/> on which to evaluate the XPath expression.</param>
        /// <param name="xpath">The XPath expression.</param>
        /// <param name="resolver">An <see cref="IXmlNamespaceResolver"/> for the namespace prefixes in the XPath expression.</param>
        /// <returns>All T-typed <see cref="XObject"/>s.</returns>
        /// <exception cref="XPathException">The XPath expression was not well-formed.</exception>
        /// <exception cref="InvalidOperationException">The XPath expression selected an invalid or illegal node.</exception>
        public IEnumerable<T> SelectAll<T>(XContainer container, string xpath, IXmlNamespaceResolver resolver = null) where T : XObject
        {
            var result = container.XPathEvaluate(xpath, resolver);
            if (result is IEnumerable<object> nodes)
                return nodes.Where(node => node is T).Cast<T>().ToList();

            throw new InvalidOperationException($"Unexpected evaluation result type: {result.GetType()}");
        }
    }
}
