using System;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// A wrapper around Unity component<br/>
    /// Defines rules by which the ID and Style name of the component are evaluated<br/>
    /// Note: the subject of selector is not evaluated here - it is always the full type name
    /// </summary>
    internal class DefaultUnityStyleClientAdapter : StyleClientAdapterBase
    {
        #region ISimpleStyleClient

        /// <summary>
        /// Getting gameObject name as ID
        /// </summary>
        override public string Id
        {
            get
            {
                return Component.gameObject.name;
            }
            set { throw new Exception("ID is read-only"); }
        }

        /// <summary>
        /// Getting an empty string as style name
        /// </summary>
        override public object StyleName
        {
            get
            {
                return string.Empty;
            }
            set { throw new Exception("Style name is read-only"); }
        }

        #endregion
    }
}