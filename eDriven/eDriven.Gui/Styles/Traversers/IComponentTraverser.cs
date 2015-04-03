using System.Collections.Generic;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Component hierarchy traverser interface
    /// </summary>
    public interface IComponentTraverser
    {
        /// <summary>
        /// Runs the initializing procedure specific to each style module<br/>
        /// Used when initializing the scene
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="styles"></param>
        void InitStyles(Selector selector, StyleTable styles);

        /// <summary>
        /// Runs the style update procedure specific to each style module<br/>
        /// Used by the editor for changing styles on-the-fly<br/>
        /// Also handles the visual feedback of what's changed (gizmos etc.)
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="delta"></param>
        void UpdateStyles(Selector selector, DictionaryDelta delta);
    }
}