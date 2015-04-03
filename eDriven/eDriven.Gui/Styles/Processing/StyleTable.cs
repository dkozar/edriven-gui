using System;
using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Private
    /// </summary>
    public class StyleTable : Dictionary<string, object>, ICloneable
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Style table ({0} elements)", Keys.Count));
            foreach (KeyValuePair<string, object> pair in this)
            {
                sb.AppendLine(string.Format("    [{0}, {1}]", pair.Key, pair.Value));
                //if (pair.Key == "skinClass")
                //{
                //    sb.AppendLine("-- skinClass -->" + ((Type)pair.Value).FullName);
                //}
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the new values to the table and overrides the duplicates
        /// </summary>
        /// <param name="table"></param>
        public void OverrideWith(StyleTable table)
        {
            foreach (KeyValuePair<string, object> pair in table)
            {
                this[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Adds the new values to the table and overrides the duplicates
        /// </summary>
        /// <param name="table"></param>
        public StyleTable CloneAndOverrideWith(StyleTable table)
        {
            StyleTable clone = (StyleTable)Clone();
            foreach (KeyValuePair<string, object> pair in table)
            {
                clone[pair.Key] = pair.Value;
            }
            return clone;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            StyleTable table = new StyleTable();
            foreach (KeyValuePair<string, object> pair in this)
            {
                table.Add(pair.Key, pair.Value);
            }
            return table;
        }

        ///<summary>
        /// Checks if the tables are equal
        ///</summary>
        ///<param name="other"></param>
        ///<returns></returns>
        public bool Equals(StyleTable other)
        {
            if (other.Keys.Count != Keys.Count)
                return false;

            foreach (string key in other.Keys)
            {
                if (!ContainsKey(key))
                    return false;
                if (other[key] != this[key])
                    return false;
            }

            return true;
        }

        ///<summary>
        /// Gets the value if it exists, null otherwise
        ///</summary>
        ///<param name="key"></param>
        ///<returns></returns>
        public object GetValue(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }
    }
}