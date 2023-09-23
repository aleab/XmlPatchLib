using System;

namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Generic exception thrown by the XML Patcher.
    /// </summary>
    public class XmlPatcherException : Exception
    {
        public XmlPatcherException(string message) : base(message) { }
    }
}
