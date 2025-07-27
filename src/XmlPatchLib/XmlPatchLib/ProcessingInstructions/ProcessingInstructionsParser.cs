using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.ProcessingInstructions
{
    internal static class ProcessingInstructionsParser
    {
        private static readonly Regex argsRegex = new Regex("([_A-Za-z][-._A-Za-z0-9]*)=\"(.*?)\"");

        /// <summary/>
        /// <exception cref="InvalidProcessingInstructionException"></exception>
        public static IProcessingInstruction Parse(XProcessingInstruction node, IPatchOperationOptions options)
        {
            if (node.Data.EndsWith("XmlPatchLibIgnore"))
            {
                node.Data = node.Data.Replace("XmlPatchLibIgnore", "").Trim();
                return null;
            }

            var args = argsRegex.Matches(node.Data).Cast<Match>().ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
            switch (node.Target)
            {
                case CopyNode.Name:
                    if (!args.ContainsKey("sel") || string.IsNullOrWhiteSpace(args["sel"]))
                        throw new InvalidProcessingInstructionException("<?copy?> requires a non-empty 'sel' attribute.");
                    return new CopyNode(args["sel"], node, options.XPathEvaluator);

                default:
                    return null;
            }
        }
    }
}
