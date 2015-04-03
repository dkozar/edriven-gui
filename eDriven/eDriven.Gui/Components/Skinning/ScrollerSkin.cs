using System;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    [HostComponent(typeof(Scroller))]

    [Style(Name = "horizontalScrollBarSkin", Type = typeof(Type), Default = typeof(HScrollBarSkin))]
    [Style(Name = "verticalScrollBarSkin", Type = typeof(Type), Default = typeof(VScrollBarSkin))]

    public class ScrollerSkin : Skin
    {
        #region Skin parts

        public HScrollBar HorizontalScrollBar;

        public VScrollBar VerticalScrollBar;

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            VerticalScrollBar = new VScrollBar { Visible = false, SkinClass = EvaluateSkinClassFromStyle("verticalScrollBarSkin") };
            AddChild(VerticalScrollBar);

            HorizontalScrollBar = new HScrollBar { Visible = false, SkinClass = EvaluateSkinClassFromStyle("horizontalScrollBarSkin") };
            AddChild(HorizontalScrollBar);
        }
    }
}