#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Text;

namespace eDriven.Gui.Editor.Util
{
    internal static class StringUtil
    {
        /// <summary>
        /// Converts the phrase to specified convention.
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="cases">The cases.</param>
        /// <returns>string</returns>
        public static string ConvertCaseString(string phrase, Case cases)
        {
            string[] splittedPhrase = phrase.Split(' ', '-', '.');
            var sb = new StringBuilder();

            if (cases == Case.CamelCase)
            {
                sb.Append(splittedPhrase[0].ToLower());
                splittedPhrase[0] = string.Empty;
            }
            else if (cases == Case.PascalCase)
                sb = new StringBuilder();

            foreach (String s in splittedPhrase)
            {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
            }
            return sb.ToString();
        }

        //public static string TypeNameToCamelCase(string fullName)
        //{
        //    string[] parts = fullName.Split('.');
        //    if (parts.Length == 0)
        //        throw new Exception("Error: the supplied type name has no length");

        //    return ConvertCaseString(parts[parts.Length - 1], Case.CamelCase);
        //}

        public static string TypeNameToCamelCase(string fullName)
        {
            string[] parts = fullName.Split('.');
            if (parts.Length == 0)
                throw new Exception("Error: the supplied type name has no length");

            var className = parts[parts.Length - 1];

            return ToCamelCase(className);
        }

        public static string ToCamelCase(string className)
        {
            string firstChar = className[0].ToString();
            className = className.Remove(0, 1);
            firstChar = firstChar.ToLower();
            className = className.Insert(0, firstChar);
            return className;
        }

        internal enum Case
        {
            PascalCase,
            CamelCase
        }

        public static string Indent(int numberOfChars, string text)
        {
            string s = string.Empty;
            for (int i = 0; i < numberOfChars; i++)
            {
                s += "    ";
            }
            return s + text;
        }

        /// <summary>
        /// Returns the capitals of the input as a string<br/>
        /// Used for comparing strings like in the component reference search
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string Capitals(string input/*, char firstChar*/)
        {
            var output = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsUpper(c) || // capital
                    char.IsDigit(c) || // digit
                    i == 0) // first letter
                    output += c;
            }

            /*foreach (char t in input)
            {
                if (char.IsUpper(t) || char.IsDigit(t))
                    output += t;
            }*/

            return output;
        }

        public static string WrapColor(string text, string color)
        {
            return string.Format(@"<color={0}>{1}</color>", color, text);
        }

        public static string WrapTag(string text, string tag)
        {
            return string.Format(@"<{1}>{0}</{1}>", text, tag);
        }
    }
}