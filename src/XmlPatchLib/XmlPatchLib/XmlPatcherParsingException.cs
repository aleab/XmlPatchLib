using System;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib
{
    internal class XmlPatcherParsingException : Exception
    {
        public XmlPatcherParsingException(string message, XElement context) : base(message) { }
    }
}
