using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// Dictionary util
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal static class DictionaryUtil<TKey, TValue>
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string Format(Dictionary<TKey, TValue> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in dict)
            {
                sb.AppendLine(string.Format(@"""{0}"" -> {1}", element.Key, element.Value));
            }
            return sb.ToString();
        }
    }
}