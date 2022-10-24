using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace VieweD.Helpers.System
{
    public static class XmlHelper
    {
        public static Dictionary<string, string> ReadNodeAttributes(XmlNode node)
        {
            var res = new Dictionary<string, string>();
            res.Clear();
            if (node.Attributes != null)
            {
                for (var i = 0; i < node.Attributes.Count; i++)
                    res.Add(node.Attributes.Item(i).Name.ToLower(), node.Attributes.Item(i).Value);
            }
            return res;
        }

        public static string GetAttributeString(Dictionary<string, string> list, string attribName)
        {
            if (list.TryGetValue(attribName.ToLower(), out var attrib))
                return attrib;
            else
                return string.Empty;
        }

        public static bool TryAttribParse(string field, out long res)
        {
            if (field == string.Empty)
            {
                res = 0;
                return true;
            }
            bool result = false;
            bool isNegatice = field.StartsWith("-");
            if (isNegatice)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            if (field.StartsWith("0x"))
            {
                try
                {
                    res = long.Parse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if (field.StartsWith("$"))
            {
                try
                {
                    res = long.Parse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                try
                {
                    res = long.Parse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            {
                try
                {
                    res = long.Parse(field);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            if (isNegatice)
                res *= -1;
            return result;
        }

        public static bool TryAttribParseUInt64(string field, out UInt64 res)
        {
            if (field == string.Empty)
            {
                res = 0;
                return true;
            }
            bool result = false;
            // NOTE: The Negative sign is actually ignored for this one
            if (field.StartsWith("-"))
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            if (field.StartsWith("0x"))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if (field.StartsWith("$"))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            {
                try
                {
                    res = UInt64.Parse(field);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            return result;
        }

        public static long GetAttributeInt(Dictionary<string, string> list, string attribName)
        {
            if (list.TryGetValue(attribName.ToLower(), out var attrib))
            {
                if (TryAttribParse(attrib, out var v))
                    return v;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds new attribute to a node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XmlNode AddAttribute(XmlNode node, string attributeName, string attributeValue)
        {
            var typeAttribute = node.OwnerDocument.CreateAttribute(attributeName);
            typeAttribute.Value = attributeValue;
            node.Attributes.Append(typeAttribute);

            return node;
        }

        /// <summary>
        /// Sets a attribute to a new value, or creates it if it doesn't exist
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XmlNode SetAttribute(XmlNode node, string attributeName, string attributeValue)
        {
            XmlAttribute typeAttribute = null;
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.Name == attributeName)
                {
                    typeAttribute = attribute;
                    break;
                }
            }

            if (typeAttribute == null)
                return AddAttribute(node, attributeName, attributeValue);

            typeAttribute.Value = attributeValue;;

            return node;
        }
    }
}