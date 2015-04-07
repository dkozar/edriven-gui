using eDriven.Gui.Components;

namespace eDriven.Gui.Layout
{
    internal sealed class FlexChildInfo
    {
        public ILayoutElement LayoutElement;

        //----------------------------------
        //  size
        //----------------------------------

        public float Size;
        
        //----------------------------------
        //  percent
        //----------------------------------

        public float Percent;

        //----------------------------------
        //  min
        //----------------------------------

        public float Min;

        //----------------------------------
        //  max
        //----------------------------------

        public float Max;

    }
}