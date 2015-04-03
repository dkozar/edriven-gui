using System.Collections.Generic;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The base class for component traversers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponentTraverser<T> : IComponentTraverser
    {
        ///<summary>
        /// Finds all the components matching the selector
        ///</summary>
        public abstract List<T> GetComponentsMatchingSelector(Selector selector);

        /// <summary>
        /// Initialires styles
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="styles"></param>
        public abstract void InitStyles(Selector selector, StyleTable styles);

        public abstract void UpdateStyles(Selector selector, DictionaryDelta delta);
    }
}