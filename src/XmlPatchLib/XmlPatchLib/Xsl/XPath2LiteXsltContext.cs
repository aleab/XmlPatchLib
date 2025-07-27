using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Tizuby.XmlPatchLib.Xsl
{
    /// <summary>
    ///     Custom <see cref="XsltContext"/> implementing a subset of XPath 2.0's function library.
    /// </summary>
    public class XPath2LiteXsltContext : XsltContext
    {
        public override bool Whitespace => true;

        public XPath2LiteXsltContext() { }
        public XPath2LiteXsltContext(NameTable table) : base(table) { }

        public override int CompareDocument(string baseUri, string nextbaseUri) => 0;
        public override bool PreserveWhitespace(XPathNavigator node) => false;

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
        {
            if (prefix == string.Empty)
            {
                switch (name)
                {
                    case "lower-case":
                        return new DelegateContextFunction(1, 1, XPathResultType.String, argTypes, ContextFunctionDelegates.LowerCase);
                    case "upper-case":
                        return new DelegateContextFunction(1, 1, XPathResultType.String, argTypes, ContextFunctionDelegates.UpperCase);
                    case "ends-with":
                        return new DelegateContextFunction(2, 2, XPathResultType.Boolean, argTypes, ContextFunctionDelegates.EndsWith);
                    case "matches":
                        return new DelegateContextFunction(2, 3, XPathResultType.Boolean, argTypes, ContextFunctionDelegates.Matches);
                    case "replace":
                        return new DelegateContextFunction(3, 4, XPathResultType.String, argTypes, ContextFunctionDelegates.Replace);
                }
            }

            return null;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return null;
        }
    }
}
