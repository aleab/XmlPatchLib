using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Tizuby.XmlPatchLib.Xsl
{
    internal class DelegateContextFunction : IXsltContextFunction
    {
        private readonly Func<object[], object> fn;
        public int Minargs { get; }
        public int Maxargs { get; }
        public XPathResultType ReturnType { get; }
        public XPathResultType[] ArgTypes { get; }

        public DelegateContextFunction(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes, Func<object[], object> fn)
        {
            this.Minargs    = minArgs;
            this.Maxargs    = maxArgs;
            this.ReturnType = returnType;
            this.ArgTypes   = argTypes;
            this.fn         = fn;
        }

        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            return this.fn(args);
        }
    }
}
