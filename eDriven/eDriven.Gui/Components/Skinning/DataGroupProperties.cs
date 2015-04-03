using eDriven.Gui.Data;
using eDriven.Gui.Layout;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// An object holding the actual values OR flags indicating those values have been set on group itself
    /// </summary>
    internal class DataGroupProperties
    {
        // values
        public bool? AutoLayout;
        public LayoutBase Layout;
        public IList DataProvider;
        public IFactory ItemRenderer;
        public ItemRendererFunction ItemRendererFunction;
        public object TypicalItem;

        // flags
        public bool AutoLayoutSet;
        public bool LayoutSet;
        public bool DataProviderSet;
        public bool ItemRendererSet;
        public bool ItemRendererFunctionSet;
        public bool TypicalItemSet;
    }
}