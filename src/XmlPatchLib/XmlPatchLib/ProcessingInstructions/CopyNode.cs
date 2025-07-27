using System.Xml;
using System.Xml.Linq;
using Tizuby.XmlPatchLib.XPath;

namespace Tizuby.XmlPatchLib.ProcessingInstructions
{
    internal class CopyNode : IProcessingInstruction
    {
        internal const string Name = "XmlPatchLib.CopyNode";

        private readonly XNode _destination;
        private readonly IXPathEvaluator _xPathEvaluator;
        private readonly string _xPathExpression;

        /// <summary/>
        /// <param name="sel">XPath expression. It MUST select a single unique node.</param>
        /// <param name="destination">The dummy processing instruction node that will be replaced with the new node.</param>
        /// <param name="xPathEvaluator"></param>
        public CopyNode(string sel, XNode destination, IXPathEvaluator xPathEvaluator)
        {
            this._destination     = destination;
            this._xPathExpression = sel;
            this._xPathEvaluator  = xPathEvaluator ?? new DefaultXPathEvaluator();
        }

        public void Execute(XDocument sourceDocument, IXmlNamespaceResolver nsResolver = null)
        {
            if (nsResolver == null)
                nsResolver = this._destination.Document.GetNamespaceResolver();

            var target = this._xPathEvaluator.SelectSingle<XNode>(sourceDocument, this._xPathExpression, nsResolver);
            this._destination.ReplaceWith(target);
        }
    }
}
