using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using UnityEngine;

/// <summary>
/// Just an example script
/// </summary>
public class Default : Gui
{
    private Button _btnDummy;
    private Group _viewport;

    /// <summary>
    /// Use this override for initializing values
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();

        // for example, this stage has vertical layout for placing it's 2 children: 1) toolbar and 2) scroller
        Layout = new VerticalLayout
        {
            HorizontalAlign = HorizontalAlign.Left,
            Gap = 0
        };
    }

    /// <summary>
    /// Use this override for creating children
    /// </summary>
    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Toolbar

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        _btnDummy = new Button
        {
            Text = "Click me",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/add"),
            FocusEnabled = false,
            AutoRepeat = true
        };
        _btnDummy.Click += delegate { AddButton(); };
        toolbar.AddContentChild(_btnDummy);

        Button btnClear = new Button
        {
            Text = "Clear",
            Icon = Resources.Load<Texture>("Icons/cancel"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false
        };
        btnClear.Click += delegate
        {
            _viewport.RemoveAllChildren();
        };
        toolbar.AddContentChild(btnClear);

        toolbar.AddContentChild(new Spacer {PercentWidth = 100});
        toolbar.AddContentChild(new Label { Text = "eDriven.Gui"});

        #endregion

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof(ScrollerSkin2),
            PercentWidth = 100, PercentHeight = 100
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        _viewport = new Group
        {
            // viewport also has vertical layout
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            }
        };
        scroller.Viewport = _viewport;

        #endregion
    }

    /// <summary>
    /// Just a helper method
    /// </summary>
    private void AddButton()
    {
        Button btn = new Button
        {
            Text = "Button",
            Icon = ImageLoader.Instance.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false,
            PercentWidth = 30, // 30% width of a parent...
            MinWidth = 200, // ...but not smaller than 200px
            Height = 200
        };
        _viewport.AddContentChild(btn);
    }
}