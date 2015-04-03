using Assets.eDriven.Demo.Gui.Code.ViewportDemo;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Styles;
using UnityEngine;

public class ViewportDemo : eDriven.Gui.Gui
{
    private Button _btn;
    private Panel _panel;

    private HGroup _hgroup;
    
    override protected void CreateChildren()
    {
        base.CreateChildren();

        /**
         * Note: this demo HAS the overall scroller, but the stage itself SEEMS not to scroll
         * That's because we pushed the scrolled 150 px from both right and bottom
         * so we could place additional scrollbars there.
         * */

        #region Scroller + viewport

        Scroller scroller = new Scroller
        {
            SkinClass = typeof (ScrollerSkin2),
            Left = 10,
            Right = 150,
            Top = 10,
            Bottom = 150
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        Group viewport = new Group();
        scroller.Viewport = viewport;

        #endregion
        
        #region Horizontal scrollers

        HGroup hGroup = new HGroup { Gap = 10 };
        viewport.AddChild(hGroup);

        VGroup hScrollers = new VGroup
                                {
                                    PercentWidth = 100,
                                    Gap = 10,
                                    Left = 10,
                                    Right = 150,
                                    Bottom = 10
                                };
        AddChild(hScrollers);

        HScrollBar hScroll = new HScrollBar
                                    {
                                        SkinClass = typeof(HScrollBarSkin3),
                                        PercentWidth = 100,
                                        Maximum = 600,
                                        Viewport = viewport,
                                        MouseWheelScrollsHorizontally = true
                                    };
        hScrollers.AddChild(hScroll);

        hScroll = new HScrollBar
                        {
                            SkinClass = typeof(HScrollBarSkin2),
                            PercentWidth = 100,
                            Maximum = 600,
                            Viewport = viewport,
                        };
        hScrollers.AddChild(hScroll);

        hScroll = new HScrollBar
                        {
                            PercentWidth = 100,
                            Maximum = 600,
                            Viewport = viewport,
                        };
        hScrollers.AddChild(hScroll);

        #endregion

        #region Vertical scrollers

        HGroup vScrollers = new HGroup
                                {
                                    PercentHeight = 100,
                                    Gap = 10,
                                    Right = 10,
                                    Top = 10,
                                    Bottom = 150
                                };
        AddChild(vScrollers);

        VScrollBar vScroll = new VScrollBar
                                    {
                                        SkinClass = typeof(VScrollBarSkin3), PercentHeight = 100, Maximum = 600,
                                        Viewport = viewport
                                    };
        vScrollers.AddChild(vScroll);

        vScroll = new VScrollBar
        {
            SkinClass = typeof(VScrollBarSkin2),
            PercentHeight = 100,
            Maximum = 600,
            Viewport = viewport
        };
        vScrollers.AddChild(vScroll);

        vScroll = new VScrollBar
        {
            PercentHeight = 100,
            Maximum = 600,
            Viewport = viewport
        };
        vScrollers.AddChild(vScroll);

        #endregion

        #region Content

        VGroup vGroup = new VGroup
        {
            Id = "vGroup",
            VerticalAlign = VerticalAlign.Middle,
            HorizontalCenter = 0,
            VerticalCenter = 0,
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            Gap = 10
        };

        vGroup.SetStyle("showBackground", true);
        vGroup.SetStyle("backgroundColor", Color.white);
        hGroup.AddChild(vGroup);

        HGroup hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        _btn = new Button
        {
            Text = "Default button",
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
            MinWidth = 200,
            MinHeight = 150
        };
        /* Let's just add a panel to the bottom row on double-clicking this button */
        _btn.DoubleClick += delegate { AddPanel(); };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Default toggle",
            FocusEnabled = false,
            ToggleMode = true,
            Selected = true,
            Icon = ImageLoader.Instance.Load("Icons/page_white_text")
        };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Styled button",
            SkinClass = typeof (ButtonSkin2),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
            MinWidth = 200,
            MinHeight = 150
        };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Styled toggle",
            SkinClass = typeof (ButtonSkin2),
            FocusEnabled = false,
            ToggleMode = true,
            Selected = true,
            Icon = ImageLoader.Instance.Load("Icons/page_white_text")
        };
        hgroup.AddChild(_btn);

        // nice buttons 1st row

        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        _btn = new Button
        {
            Text = "Option 1",
            SkinClass = typeof (ButtonSkin4),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/arkshany-bookmarks"),
            MinWidth = 200,
            MinHeight = 200
        };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Option 2",
            SkinClass = typeof (ButtonSkin4),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/arkshany-bookmarks"),
            MinWidth = 200,
            MinHeight = 200
        };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Option 3",
            SkinClass = typeof (ButtonSkin4),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/arkshany-bookmarks"),
            MinWidth = 200,
            MinHeight = 200,
            ToggleMode = true,
            Selected = true
        };
        hgroup.AddChild(_btn);

        // nice buttons 2nd row

        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        _btn = new MyButton
        {
            Text = "Option 1",
            SkinClass = typeof (ButtonSkin5),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/applications-games"),
            MinWidth = 200,
            MinHeight = 200
        };
        hgroup.AddChild(_btn);

        _btn = new MyButton2
        {
            Text = "Option 2",
            SkinClass = typeof (ButtonSkin5),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/gconfeditor"),
            MinWidth = 200,
            MinHeight = 200
        };
        hgroup.AddChild(_btn);

        _btn = new Button
        {
            Text = "Option 3 (disabled)",
            SkinClass = typeof (ButtonSkin5),
            FocusEnabled = false,
            Icon = ImageLoader.Instance.Load("IconsBig/gtk-floppy"),
            MinWidth = 200,
            MinHeight = 200,
            Enabled = false
        };
        hgroup.AddChild(_btn);

        // new row
        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        // image
        var image = new Image
        {
            Texture = (Texture) Resources.Load("eDriven/Editor/Logo/logo2")
        };
        hgroup.AddChild(image);

        // panel 1
        _panel = new Panel
        {
            Title = "Panel 1",
            MinWidth = 200,
            MinHeight = 300
        };
        hgroup.AddChild(_panel);

        // panel 2
        _panel = new Panel
        {
            SkinClass = typeof (PanelSkin3),
            Title = "Panel 2",
            MinWidth = 200,
            MinHeight = 300
        };
        hgroup.AddChild(_panel);

        // panel 3
        _panel = new Panel
        {
            SkinClass = typeof (PanelSkin4),
            Title = "Panel 3",
            MinWidth = 200,
            MinHeight = 300
        };
        hgroup.AddChild(_panel);

        #endregion

        // new row (placeholder)
        _hgroup = new HGroup();
        vGroup.AddChild(_hgroup);
    }

    private int _count = 3;
    
    private void AddPanel()
    {
        _panel = new Panel
                        {
                            SkinClass = typeof(PanelSkin4),
                            Title = "Panel " + _count,
                            MinWidth = 200,
                            MinHeight = 300
                        };
        _panel.SetStyle("headerBackgroundRollOverColor", Color.cyan);
        _hgroup.AddChild(_panel);
        _count++;
    }
}