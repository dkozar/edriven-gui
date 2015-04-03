using eDriven.Gui.Styles.Serialization;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The factory that could be created only with referencing the serialized declaration<br/>
    /// It is used for creating a new value set
    /// </summary>
    internal class StyleSheetValuesFactory : IStyleValuesFactory
    {
        private readonly Serialization.StyleDeclaration _styleSheetDeclaration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="declaration">Serialized style declaration</param>
        public StyleSheetValuesFactory(Serialization.StyleDeclaration declaration)
        {
            _styleSheetDeclaration = declaration;
        }

        private StyleTable _cached;

        /// <summary>
        /// Reads the serialized declaration and extracts key/value pairs
        /// </summary>
        /// <returns></returns>
        public StyleTable Produce()
        {
            if (null == _cached)
            {
                _cached = new StyleTable();

//#if DEBUG
//                        StringBuilder sb = new StringBuilder();
//                        foreach (StyleProperty property in _styleSheetDeclaration.Properties)
//                        {
//                            sb.AppendLine(string.Format("[{0}, {1}]", property.Name, property.Value));
//                        }
//                        Debug.Log(string.Format(@"StyleSheetValuesFactory->Produced
//{0}", sb));
//#endif

                foreach (StyleProperty property in _styleSheetDeclaration.Properties)
                {
                    _cached.Add(property.Name, property.Value);
                }
            }

            return (StyleTable)_cached.Clone();
        }

        /// <summary>
        /// Overrides the existing values
        /// </summary>
        /// <param name="declaration"></param>
        public void Override(Serialization.StyleDeclaration declaration)
        {
            foreach (StyleProperty property in _styleSheetDeclaration.Properties)
            {
                _cached[property.Name] = property.Value;
            }
        }
    }
}