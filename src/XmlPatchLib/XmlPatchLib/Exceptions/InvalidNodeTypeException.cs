namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when the node type of a &lt;replace&gt; operation did not match the target node. Also thrown when
    ///     a &lt;replace&gt; operation contains multiple nodes.
    /// </summary>
    public class InvalidNodeTypeException : XmlPatcherException
    {
        public InvalidNodeTypeException(string message) : base(message) { }
    }
}
