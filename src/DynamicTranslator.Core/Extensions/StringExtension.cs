﻿#region using

using System;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace DynamicTranslator.Core.Extensions
{

    #region using

    #endregion

    public static class StringExtension
    {
        private static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string GenerateSlug(this string phrase)
        {
            var str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            //str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            // str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            //return Regex.Replace(Regex.Replace(str.TrimEnd(Environment.NewLine.ToCharArray()).Trim(),@"\t|\n|\r", " "),"[^ -~]", ""); //REmove non-AsCII characters
            return Regex.Replace(str.TrimEnd(Environment.NewLine.ToCharArray()).Trim(), @"\t|\n|\r", " "); //REmove non-AsCII characters
        }

        public static string StripTagsCharArray(this string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var @let in source)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }
                if (@let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = @let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string StripTagsRegex(this string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        public static string StripTagsRegexCompiled(this string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        private static string RemoveAccent(this string txt)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string GetEncodedUrlForTurkishCharacters(this string textToEncode)
        {
            var temp = textToEncode;

            for (var i = 0; i < temp.Length; i++)
            {
                switch (temp[i])
                {
                    case 'ç':
                        temp = temp.Replace("ç", "%C3%A7");
                        break;
                    case 'Ç':
                        temp = temp.Replace("Ç", "%C3%87");
                        break;
                    case 'ğ':
                        temp = temp.Replace("ğ", "%C4%9F");
                        break;
                    case 'Ğ':
                        temp = temp.Replace("Ğ", "%C4%9E");
                        break;
                    case 'ı':
                        temp = temp.Replace("ı", "%C4%B1");
                        break;
                    case 'İ':
                        temp = temp.Replace("İ", "%C4%B0");
                        break;
                    case 'ö':
                        temp = temp.Replace("ö", "%C3%B6");
                        break;
                    case 'Ö':
                        temp = temp.Replace("Ö", "%C3%96");
                        break;
                    case 'ş':
                        temp = temp.Replace("ş", "%C5%9F");
                        break;
                    case 'Ş':
                        temp = temp.Replace("Ş", "%C5%9E");
                        break;
                    case 'ü':
                        temp = temp.Replace("ü", "%C3%BC");
                        break;
                    case 'Ü':
                        temp = temp.Replace("Ü", "%C3%9C");
                        break;
                }
            }

            return temp;
        }
    }
}
