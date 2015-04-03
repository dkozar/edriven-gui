using UnityEngine;

namespace eDriven.Gui.Styles
{
    internal class GlobalStyle
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static GlobalStyle _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private GlobalStyle()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GlobalStyle Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating GlobalStyle instance"));
#endif
                    _instance = new GlobalStyle();
                    //_instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        internal void Initialize()
        {
            //StyleDeclaration style = StyleManager.Instance.GetStyleDeclaration("global");
            //if (null == style)
            //{
            //    style = new StyleDeclaration();
                
            //    var dict = new Dictionary<string, object> { { "foo", "bar" } };
            //    style.Factory = new StyleTableValuesFactory(dict); // ?? DefaultValues ???????

            //    StyleManager.Instance.SetStyleDeclaration("global", style, false);
            //}

            var styleManager = StyleManager.Instance;
            var selector = new Selector("global", null, null);
            var mergedStyle = styleManager.GetMergedStyleDeclaration("global");
            var style = new StyleDeclaration(selector, mergedStyle == null);

            if (style.Set1 == null)
            {
                //Debug.Log("Creating DefaultValues for " + selector);
                var dict = new StyleTable(); // { { "foo", "bar" } };
                style.Set1 = new StyleTableValuesFactory(dict);
            }

            if (null != mergedStyle && 
                (null == mergedStyle.Set1 ||
                style.Set1.Produce().Equals(mergedStyle.Set1.Produce())))
            {
                styleManager.SetStyleDeclaration(style.SelectorString, style, false);
            }

            //Debug.Log("Global style initialized: " + styleManager.GetStyleDeclaration("global"));
        }
    }
}
