using System;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    public class XmlPatcherError
    {
        private readonly XElement _operationNode;
        public Exception Exception { get; }

        public XmlPatcherError(XElement operationNode, Exception ex)
        {
            this._operationNode = operationNode;
            this.Exception = ex;
        }

        public override string ToString()
        {
            return $"{this.Exception.Message}\n    {Utils.ToString(this._operationNode)}";
        }
    }
}
