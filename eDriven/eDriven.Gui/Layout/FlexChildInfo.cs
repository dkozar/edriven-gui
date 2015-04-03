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
        //  preferred
        //----------------------------------

        public float Preferred;

        //----------------------------------
        //  flex
        //----------------------------------

        public float Flex;
	
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

        //----------------------------------
        //  width
        //----------------------------------

        public float Width;

        //----------------------------------
        //  height
        //----------------------------------

        public float Height;

    }
}