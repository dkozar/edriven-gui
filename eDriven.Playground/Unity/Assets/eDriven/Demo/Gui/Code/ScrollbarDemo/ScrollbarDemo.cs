using Assets.eDriven.Skins;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Components;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

public class ScrollbarDemo : Gui
{
    private HScrollBar _scrollbar2;

    override protected void CreateChildren()
    {
        base.CreateChildren();

        TitleLabel titleLabel = new TitleLabel
        {
            Text = "Scrollbar Demo",
            StyleName = "title",
            Right = 20,
            Top = 20
        };
        AddChild(titleLabel);

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof (ScrollerSkin2),
            Left = 0,
            Right = 0,
            Top = 0,
            Bottom = 0,
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        Group viewport = new Group
        {
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            }
        };
        scroller.Viewport = viewport;

        #endregion

        #region Vertical scrollbars

        HGroup hGroup = new HGroup {Gap = 10, Id = "hbox2", PercentHeight = 100};
        viewport.AddChild(hGroup);

        VScrollBar s = new VScrollBar {PercentHeight = 100, Maximum = 300};
        s.Change += delegate(Event e)
        {
            Debug.Log("Change: " + e);
        };
        hGroup.AddChild(s);

        s = new VScrollBar {PercentHeight = 100, Maximum = 400, PageSize = 100};
        hGroup.AddChild(s);

        s = new VScrollBar {SkinClass = typeof (VScrollBarSkin2), PercentHeight = 100, Maximum = 1000, PageSize = 100};
        hGroup.AddChild(s);

        s = new VScrollBar {SkinClass = typeof (VScrollBarSkin2), Height = 400, Maximum = 400, PageSize = 100};
        hGroup.AddChild(s);

        s = new VScrollBar {SkinClass = typeof (VScrollBarSkin3), PercentHeight = 100, Maximum = 200, PageSize = 100};
        hGroup.AddChild(s);

        s = new VScrollBar {SkinClass = typeof (VScrollBarSkin3), Height = 400, Maximum = 300, PageSize = 100};
        hGroup.AddChild(s);

        #endregion

        #region Horizontal scrollbars

        Label label = new Label {Text = "Will change the skin on drag: "};
        viewport.AddChild(label);

        HScrollBar scrollBar1 = new HScrollBar {Width = 300, Maximum = 300, PageSize = 100};
        scrollBar1.Change += delegate(Event e)
        {
            scrollBar1.SkinClass = typeof (HScrollBarSkin3);
        };
        viewport.AddChild(scrollBar1);

        label = new Label {Text = "Will change the skin on drag: "};
        viewport.AddChild(label);

        _scrollbar2 = new HScrollBar {PercentWidth = 100, Maximum = 500, Value = 200, PageSize = 100};
        _scrollbar2.Change += delegate(Event e)
        {
            _scrollbar2.SkinClass = typeof (HScrollBarSkin3);
        };
        viewport.AddChild(_scrollbar2);

        HScrollBar scrollbar3 = new HScrollBar
        {
            SkinClass = typeof (HScrollBarSkin2),
            MinWidth = 600,
            Maximum = 1000,
            PageSize = 100
        };
        viewport.AddChild(scrollbar3);

        HScrollBar scrollbar4 = new HScrollBar
        {
            SkinClass = typeof (HScrollBarSkin3),
            MinWidth = 700,
            Maximum = 300,
            PageSize = 100
        };
        viewport.AddChild(scrollbar4);

        HScrollBar scrollbar5 = new HScrollBar
        {
            PercentWidth = 100,
            MinWidth = 600,
            SkinClass = typeof (HScrollBarSkin3),
            Maximum = 1000,
            PageSize = 100
        };
        viewport.AddChild(scrollbar5);

        #endregion

    }
}