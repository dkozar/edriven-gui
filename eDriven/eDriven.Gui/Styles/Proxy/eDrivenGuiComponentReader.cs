using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// eDriven.Gui style proxy
    /// </summary>
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
    public class eDrivenGuiComponentReader : IStyleReader
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global
    {
        #region Implementation of IStyleReader

        /// <summary>
        /// Gets the component ID
        /// </summary>
        /// <returns></returns>
        public string GetId(object current)
        {
            return ((Component)current).Id;
        }

        /// <summary>
        /// Gets the component classname
        /// </summary>
        /// <returns></returns>
        public string GetClassname(object current)
        {
            return ((Component)current).StyleName as string; // Note: could return null
        }

        /// <summary>
        /// Gets the parent based on supplied component
        /// </summary>
        /// <param name="?"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public object GetParent(object current)
        {
            //throw new NotImplementedException();
            return null;
        }

        #endregion
    }
}