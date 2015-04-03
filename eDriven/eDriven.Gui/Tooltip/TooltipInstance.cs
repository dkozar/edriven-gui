using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Tooltip
{
    //[StyleProxy(typeof(TooltipStyleProxy))]
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(TooltipStyle))]

    /// <summary>
    /// A graphical alert component
    /// It is raised using static Show methods of an Alert class
    /// </summary>
    public class TooltipInstance : Label
    {
        public TooltipInstance()
        {
            //SetStyle("labelStyle", TooltipStyle.Instance); // TooltipStyle.Instance);
            //SetStyle("showEffect", new TooltipFadeIn()); // TODO: these must be factories!
            //SetStyle("hideEffect", new TooltipFadeOut());

            MouseEnabled = false;
            MouseChildren = false;
            FocusEnabled = false;
            ResizeWithStyleBackground = true;
            Visible = false;
            //Alpha = 0;
        }

        //public override void StyleChanged(string styleName, object s)
        //{
        //    base.StyleChanged(styleName, s);

        //    switch (styleName)
        //    {
        //        case "labelStyle":
        //            ActiveStyle = s as GUIStyle;
        //            break;
        //    }
        //}
    }
}