namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when a given patch operation could not be fulfilled either because it was not well-formed or
    ///     because of the state of the target document.<br/>
    ///     Examples include an unrecognized and unparsable patch element, or an &lt;add&gt; operation trying to add an already
    ///     existing attribute or namespace.
    /// </summary>
    public class InvalidPatchDirectiveException : XmlPatcherException
    {
        public InvalidPatchDirectiveException(string message) : base(message) { }
    }
}
