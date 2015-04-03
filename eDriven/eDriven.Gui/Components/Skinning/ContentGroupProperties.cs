using eDriven.Gui.Layout;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// An object holding the actual values OR flags indicating those values have been set on group itself
    /// </summary>
    internal class ContentGroupProperties
    {
        // values
        public bool? AutoLayout;
        public LayoutBase Layout;

        // flags
        public bool AutoLayoutSet;
        public bool LayoutSet;
    }
}