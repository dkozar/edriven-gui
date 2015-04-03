using System;

namespace eDriven.Gui.Styles
{
    public class StyleRelation
    {
        public string StyleName;
        public Type GuiStyle;

        public StyleRelation(string styleName, Type guiStyle)
        {
            StyleName = styleName;
            GuiStyle = guiStyle;
        }
    }
}