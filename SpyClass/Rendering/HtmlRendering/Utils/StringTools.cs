using System;
using System.Linq;
using System.Text;
using SpyClass.Analysis.DataModel.Documentation;

namespace SpyClass.Rendering.HtmlRendering.Utils
{
    internal static class StringTools
    {
        public static string FNV1A64(string data)
        {
            var offsetBasis = 0xCBF29CE484222325UL;
            var prime = 0x00000100000001B3UL;

            var hash = offsetBasis;
            var bytes = Encoding.UTF8.GetBytes(data);

            for (var i = 0; i < bytes.Length; i++)
            {
                hash ^= bytes[i];
                hash *= prime;
            }

            var hashBytes = BitConverter.GetBytes(hash);

            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x"));
            }

            return sb.ToString();
        }
        
        public static string FlattenTypeName(string typeName)
        {
            return typeName
                .Split('.').Last()
                .Split('/').Last();
        }
        
        public static string FlattenType(TypeDoc typeDoc)
        {
            return typeDoc.DisplayName
                .Split('.').Last()
                .Split('/').Last();
        }
    }
}