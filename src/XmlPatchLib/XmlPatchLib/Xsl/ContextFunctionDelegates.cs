using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Tizuby.XmlPatchLib.Xsl
{
    internal static class ContextFunctionDelegates
    {
        private static string ParseAsString(object o)
        {
            return o as string ?? (o is XPathNodeIterator it && it.MoveNext() ? it.Current?.Value : Convert.ToString(o));
        }

        private static RegexOptions ParseAsRegexFlags(object o)
        {
            var flags = new HashSet<char>(Convert.ToString(o));
            return (flags.Contains('s') ? RegexOptions.Singleline : RegexOptions.None) |
                   (flags.Contains('m') ? RegexOptions.Multiline : RegexOptions.None) |
                   (flags.Contains('i') ? RegexOptions.IgnoreCase : RegexOptions.None) |
                   (flags.Contains('x') ? RegexOptions.IgnorePatternWhitespace : RegexOptions.None);
        }

        public static object LowerCase(object[] args) => ParseAsString(args[0]).ToLower();

        public static object UpperCase(object[] args) => ParseAsString(args[0]).ToUpper();

        public static object EndsWith(object[] args) => ParseAsString(args[0])?.EndsWith(ParseAsString(args[1])) ?? false;

        public static object Matches(object[] args)
        {
            var options = args.Length > 2 ? ParseAsRegexFlags(args[2]) : RegexOptions.None;
            return Regex.IsMatch(ParseAsString(args[0]), Convert.ToString(args[1]), options);
        }

        public static object Replace(object[] args)
        {
            var options = args.Length > 3 ? ParseAsRegexFlags(args[2]) : RegexOptions.None;
            return Regex.Replace(ParseAsString(args[0]), Convert.ToString(args[1]), Convert.ToString(args[2]), options);
        }
    }
}
