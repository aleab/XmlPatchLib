﻿using System.Xml.Linq;

namespace Tizuby.XmlPatchLib.PatchOperations
{
    internal static class PatchOperation
    {
        /// <summary/>
        /// <exception cref="InvalidPatchDirectiveException"></exception>
        internal static IPatchOperation Parse(XElement operationNode)
        {
            var sel = ParseXPath(operationNode);
            switch (operationNode.Name.LocalName)
            {
                case "add":     return new AddOperation(sel, operationNode);
                case "replace": return new ReplaceOperation(sel, operationNode);
                case "remove":  return new RemoveOperation(sel, operationNode);
                default:
                    throw new InvalidPatchDirectiveException($"<{operationNode.Name.LocalName}> is not a valid patch operation.");
            }
        }

        private static string ParseXPath(XElement operationNode)
        {
            var selAttribute = operationNode.Attribute("sel");
            if (selAttribute == null || string.IsNullOrWhiteSpace(selAttribute.Value))
                throw new InvalidAttributeValueException("'sel' attribute is required and must have a non-empty value!");

            return selAttribute.Value;
        }
    }
}
