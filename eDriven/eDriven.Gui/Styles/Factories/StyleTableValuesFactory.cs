using System.Collections.Generic;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The factory that could be created only with referencing the serialized declaration<br/>
    /// It is used for creating a new value set
    /// </summary>
    internal class StyleTableValuesFactory : IStyleValuesFactory
    {
        private readonly StyleTable _table;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"></param>
        public StyleTableValuesFactory(StyleTable table)
        {
            _table = table;
        }

        /// <summary>
        /// Reads the serialized declaration and extracts key/value pairs
        /// </summary>
        /// <returns></returns>
        public StyleTable Produce()
        {
            StyleTable table = new StyleTable();
            foreach (KeyValuePair<string, object> pair in _table)
            {
                table.Add(pair.Key, pair.Value);
            }
            return table;
        }
    }
}