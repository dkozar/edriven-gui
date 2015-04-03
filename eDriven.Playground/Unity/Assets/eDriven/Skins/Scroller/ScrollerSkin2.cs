using System;
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;

namespace Assets.eDriven.Skins
{
    [HostComponent(typeof(Scroller))]

    [Style(Name = "horizontalScrollBarSkin", Type = typeof(Type), Default = typeof(HScrollBarSkin3))]
    [Style(Name = "verticalScrollBarSkin", Type = typeof(Type), Default = typeof(VScrollBarSkin3))]

    public class ScrollerSkin2 : Skin
    {
        #region Skin parts

        public HScrollBar HorizontalScrollBar;

        public VScrollBar VerticalScrollBar;

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            VerticalScrollBar = new VScrollBar { Visible = false, SkinClass = (Type)GetStyle("verticalScrollBarSkin") };
            AddChild(VerticalScrollBar);

            HorizontalScrollBar = new HScrollBar { Visible = false, SkinClass = (Type)GetStyle("horizontalScrollBarSkin") };
            AddChild(HorizontalScrollBar);
        }
    }
}