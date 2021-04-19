using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SnowTopia_ModLoader
{
    class Helpers
    {
        public static TypeAttributes? GetTypeAttributes(string attributes)
        {
            TypeAttributes? result = null;

            attributes.Split(", ").ToList().ForEach(attribute =>
            {
                if (result == null)
                    result = (TypeAttributes)Enum.Parse(typeof(TypeAttributes), attribute);
                else
                    result |= (TypeAttributes)Enum.Parse(typeof(TypeAttributes), attribute);
            });

            return result;
        }

        public static FieldAttributes? GetFieldAttributes(string attributes)
        {
            FieldAttributes? result = null;

            attributes.Split(", ").ToList().ForEach(attribute =>
            {
                if (result == null)
                    result = (FieldAttributes)Enum.Parse(typeof(FieldAttributes), attribute);
                else
                    result |= (FieldAttributes)Enum.Parse(typeof(FieldAttributes), attribute);
            });

            return result;
        }

        public static MethodAttributes? GetMethodAttributes(string attributes)
        {
            MethodAttributes? result = null;

            attributes.Split(", ").ToList().ForEach(attribute =>
            {
                if (result == null)
                    result = (MethodAttributes)Enum.Parse(typeof(MethodAttributes), attribute);
                else
                    result |= (MethodAttributes)Enum.Parse(typeof(MethodAttributes), attribute);
            });

            return result;
        }

        public static string GetClassFromFullName(string FullName)
        {
            Regex regex = new Regex("[a-zA-Z]+\\.[a-zA-Z]+ ([a-zA-Z]+)::[a-zA-Z]+\\([^)]*\\)", RegexOptions.IgnoreCase);
            return regex.Match(FullName).Groups[1].Value;
        }

        public static string GetNameFromFullName(string FullName)
        {
            Regex regex = new Regex("[a-zA-Z]+\\.[a-zA-Z]+ ([a-zA-Z]+)::([a-zA-Z]+)\\([^)]*\\)", RegexOptions.IgnoreCase);
            return regex.Match(FullName).Groups[2].Value;
        }

        public static string GetNameSpaceOfTypeFromFullName(string FullName)
        {
            Regex regex = new Regex("([a-zA-Z]+)\\.([a-zA-Z]+) ([a-zA-Z]+)::([a-zA-Z]+)\\([^)]*\\)", RegexOptions.IgnoreCase);
            return regex.Match(FullName).Groups[1].Value;
        }

        public static string GetTypeFromFullName(string FullName)
        {
            Regex regex = new Regex("([a-zA-Z]+)\\.([a-zA-Z]+) ([a-zA-Z]+)::([a-zA-Z]+)\\([^)]*\\)", RegexOptions.IgnoreCase);
            return regex.Match(FullName).Groups[2].Value;
        }

        public static int ConvertOffsetToInt(string offset) => offset == "" ? 0 : Convert.ToInt32(offset, 16);
    }
}
