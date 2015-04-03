using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.States
{
    public class StateTable : Dictionary<string, object>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("State table ({0} elements)", Keys.Count));
            foreach (KeyValuePair<string, object> pair in this)
            {
                sb.AppendLine(string.Format("[{0}, {1}]", pair.Key, pair.Value));
                //if (pair.Key == "skinClass")
                //{
                //    sb.AppendLine("-- skinClass -->" + ((Type)pair.Value).FullName);
                //}
            }
            return sb.ToString();
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