namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when a processing instruction could not be properly parsed.
    /// </summary>
    public class InvalidProcessingInstructionException : XmlPatcherException
    {
        public InvalidProcessingInstructionException(string message) : base(message) { }
    }
}
