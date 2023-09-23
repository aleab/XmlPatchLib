namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when the diff document is not a well-formed XML document or is not valid according to the schema.
    /// </summary>
    internal class InvalidDiffFormatException : XmlPatcherException
    {
        public InvalidDiffFormatException(string message) : base(message) { }
    }
}
