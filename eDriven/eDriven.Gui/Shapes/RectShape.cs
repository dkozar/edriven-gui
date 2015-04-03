using eDriven.Gui.GUIStyles;
using eDriven.Gui.Styles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Shapes
{
    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(RectShapeStyle))]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]

    public class RectShape : Component
    {
        //public override void StyleChanged(string styleName, object s)
        //{
        //    base.StyleChanged(styleName, s);

        //    switch (styleName)
        //    {
        //        case "backgroundStyle":
        //            ActiveStyle = (GUIStyle)s;
        //            break;
        //    }
        //}

        protected override void UpdateDisplayList(float width, float height)
        {
            ActiveStyle = (GUIStyle)GetStyle("backgroundStyle");
            Color = (Color) GetStyle("backgroundColor");
            
            base.UpdateDisplayList(width, height);
            //Debug.Log("RectShape backgroundStyle: " + (GUIStyle)GetStyle("backgroundStyle"));
        }
    }
}