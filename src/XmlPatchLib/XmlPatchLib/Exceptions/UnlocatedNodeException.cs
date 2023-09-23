namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when a single unique node could not be located with the given 'sel' attribute value.
    /// </summary>
    public class UnlocatedNodeException : XmlPatcherException
    {
        public UnlocatedNodeException(string message) : base(message) { }
    }
}
