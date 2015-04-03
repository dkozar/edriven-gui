using System;
using eDriven.Gui.Components;
using Component = UnityEngine.Component;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// A wrapper around Unity component<br/>
    /// Defines rules by which the ID and Style name of the component are evaluated<br/>
    /// Note: the subject of selector is not evaluated here - it is always the full type name<br/>
    /// Adapter class should be written per component and extend StyleClientAdapterBase<br/>
    /// If style client adapter for the particular component not defined - it will fall back to the default one<br/>
    /// Note: The default adapter returns Component.gameObject.name as ID and an empty string as StyleName
    /// </summary>
    public abstract class StyleClientAdapterBase : ISimpleStyleClient
    {
        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="styleName"></param>
        public void StyleChanged(string styleName)
        {
            // do nothing
        }

        /// <summary>
        /// The definition of component ID
        /// </summary>
        public abstract string Id { get; set; }

        /// <summary>
        /// The definition of component style name (CSS class)
        /// </summary>
        public abstract object StyleName { get; set; }

        private Component _component;

        /// <summary>
        /// The component to operate with
        /// </summary>
        protected Component Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Initializes the adapter (the adapter is reused)
        /// </summary>
        /// <param name="component"></param>

        public void Initialize(Component component)
        {
            _component = component;
        }
    }
}