using System;
using Assets.eDriven.Demo.Gui.Code.PanelDemo;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Components;
using UnityEngine;

public class PanelDemo : Gui
{
    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Title

        Label label = new TitleLabel {HorizontalCenter = 0, Top = 20, StyleName = "title", Text = "Panel demo"};
        AddChild(label);

        #endregion

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

        Group viewport = new Group();
        scroller.Viewport = viewport;

        #endregion

        #region HGroup

        HGroup hGroup = new HGroup {Gap = 10, HorizontalCenter = 0, VerticalCenter = 0};
        viewport.AddChild(hGroup);

        #endregion

        #region Panel 1

        Panel panel = new MyPanel
        {
            Width = 360,
            Height = 600,
            Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
            Title = "First panel",
            StyleName = "default"
        };
        hGroup.AddChild(panel);

        #endregion
        
        #region Panel 2

        panel = new MyPanel
        {
            //Width = 360,
            Height = 600,
            SkinClass = typeof(PanelSkin2),
            Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
            Title = "Second panel"
        };
        hGroup.AddChild(panel);

        //// NOTE: propagation of styles to skin still not implemented
        //// So, this won't work: panel.SetStyle("headerLabelColor", 0xffff00);
        //// This is a temporary solution:
        //panel.CreationCompleteHandler += delegate
        //{
            //Debug.Log("panel.Skin: " + panel.Skin);
            //panel.Skin.SetStyle("headerLabelColor", Color.yellow);
        //};

        #endregion

        #region Panel 2 skin switch

        VGroup vGroup = new VGroup();
        vGroup.Plugins.Add(new TabManager { ArrowsEnabled = true, UpDownArrowsEnabled = true });
        hGroup.AddChild(vGroup);

        Button button = new Button { Text = "Skin 1", Icon = Resources.Load<Texture>("Icons/skin"), SkinClass = typeof(ImageButtonSkin), PercentWidth = 100 };
        button.Press += delegate
        {
            panel.SkinClass = typeof (PanelSkin);
            //((MyPanel)panel).CreateButtons();
        };
        vGroup.AddChild(button);

        button = new Button { Text = "Skin 2", Icon = Resources.Load<Texture>("Icons/skin"), SkinClass = typeof(ImageButtonSkin), PercentWidth = 100 };
        button.Press += delegate
        {
            panel.SkinClass = typeof (PanelSkin2);
        };
        vGroup.AddChild(button);

        button = new Button { Text = "Skin 3", Icon = Resources.Load<Texture>("Icons/skin"), SkinClass = typeof(ImageButtonSkin), PercentWidth = 100 };
        button.Press += delegate
        {
            panel.SkinClass = typeof (PanelSkin3);
        };
        vGroup.AddChild(button);

        button = new Button { Text = "Skin 4", Icon = Resources.Load<Texture>("Icons/skin"), SkinClass = typeof(ImageButtonSkin), PercentWidth = 100 };
        button.Press += delegate
        {
            panel.SkinClass = typeof (PanelSkin4);
        };
        vGroup.AddChild(button);

        button = new Button { Text = "Add button", AutoRepeat = true, Icon = Resources.Load<Texture>("Icons/add"), SkinClass = typeof(ImageButtonSkin), PercentWidth = 100 };
        /*button.Press += delegate
        {
            ((MyPanel)panel).AddButton("Button");
        };*/
        button.ButtonDown += delegate
        {
            ((MyPanel)panel).AddButton("Button");
        };
        vGroup.AddChild(button);

        #endregion
    }
}