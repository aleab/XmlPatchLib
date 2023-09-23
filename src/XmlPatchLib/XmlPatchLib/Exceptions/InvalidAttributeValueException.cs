using System.Collections.Generic;
using System.Linq;

namespace Tizuby.XmlPatchLib
{
    /// <summary>
    ///     Exception thrown when the validity constraints of the 'sel', 'type', 'ws' or 'pos' attribute values are not met.
    /// </summary>
    public class InvalidAttributeValueException : XmlPatcherException
    {
        private const string MessageFormat = "Invalid value for {0}attribute '{1}': \"{2}\". Allowed values: {3}";

        public InvalidAttributeValueException(string message) : base(message) { }

        public InvalidAttributeValueException(string name, string value, IEnumerable<string> allowedValues, bool optional = false)
            : base(string.Format(MessageFormat, optional ? "optional " : "", name, value, string.Join(", ", allowedValues.Select(s => $"\"{s}\"")))) { }
    }
}
