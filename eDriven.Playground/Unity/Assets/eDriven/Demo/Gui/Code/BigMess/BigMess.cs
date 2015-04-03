using Assets.eDriven.Demo;
using Assets.eDriven.Demo.Gui.Code;
using Assets.eDriven.Demo.Gui.Code.PanelDemo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Action=eDriven.Animation.Action;
using Event=eDriven.Core.Events.Event;

public class BigMess : Gui
{
    #region Effects

    private readonly TweenFactory _windowShow = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
            new Jumpy()
        )
    );

    private readonly TweenFactory _overlayShow = new TweenFactory(
        new Sequence( 
            new FadeIn()
        )
    );

    #endregion

    private int _count;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // currently initializing skins from code (should be done through styles)
        //Alert.DefaultSkin = typeof(AlertSkin2);
        ModalOverlay.DefaultSkin = typeof(ModalOverlaySkin2);
        
        Dialog.AddedEffect = _windowShow;
        ModalOverlay.AddedEffect = _overlayShow;

        Layout = new VerticalLayout
        {
            PaddingLeft = 0,
            PaddingRight = 0,
            PaddingTop = 0,
            PaddingBottom = 0,
            Gap = 0
        };
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Controls

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        #region Alert

        Button btnAlert = new Button
        {
            Text = "Alert",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnAlert.Click += delegate
        {
            Alert.Show("Info", "This is the example alert.", AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/information")),
                new AlertOption(AlertOptionType.Icon, Resources.Load<Texture>("Icons/star_big")));
        };
        toolbar.AddContentChild(btnAlert);

        #endregion

        #region Window

        Button btnWindow = new Button
        {
            Text = "New window",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnWindow.Click += delegate
        {
            _count++;
            var window = new MyWindow
            {
                Title = "The Window " + _count,
                Id = "window_" + _count,
                SkinClass = typeof(WindowSkin2),
                Icon = ImageLoader.Instance.Load("Icons/balloon_32"),
                Width = 400,
                Height = 600
            };

            window.SetStyle("addedEffect", _windowShow);
            window.Plugins.Add(new Resizable { ShowOverlay = false });
            window.AddEventListener(CloseEvent.CLOSE, delegate
            {
                PopupManager.Instance.RemovePopup(window);
            });
            PopupManager.Instance.AddPopup(window, false);
            PopupManager.Instance.CenterPopUp(window);
        };
        toolbar.AddContentChild(btnWindow);

        #endregion

        #endregion

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof (ScrollerSkin2),
            PercentWidth = 100, PercentHeight = 100
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
        
        #region Horizontal Scrollbars

        viewport.AddChild(new HScrollBar { SkinClass = typeof(HScrollBarSkin3), PercentWidth = 100, MinWidth = 300, Maximum = 300, PageSize = 100 });
        viewport.AddChild(new HScrollBar { SkinClass = typeof(HScrollBarSkin3), PercentWidth = 50, Maximum = 500, Value = 200, PageSize = 100 });
        viewport.AddChild(new HScrollBar { SkinClass = typeof(HScrollBarSkin2), MinWidth = 600, Maximum = 1000, PageSize = 100 });
        viewport.AddChild(new HScrollBar { SkinClass = typeof(HScrollBarSkin3), MinWidth = 700, Maximum = 300, PageSize = 100 });
        viewport.AddChild(new HScrollBar { PercentWidth = 100, MinWidth = 600, SkinClass = typeof(HScrollBarSkin3), Maximum = 1000, PageSize = 100 });

        #endregion

        #region HGroup

        HGroup hGroup = new HGroup { /*PercentWidth = 100, */Gap = 10 };
        viewport.AddChild(hGroup);

        #endregion

        #region Vertical scrollbars

        VScrollBar vScrollBar = new VScrollBar { PercentHeight = 100, Maximum = 300 };
        vScrollBar.Change += delegate(Event e) { Debug.Log("Change: " + e); };
        hGroup.AddChild(vScrollBar);

        hGroup.AddChild(new VScrollBar { PercentHeight = 100, Maximum = 400, PageSize = 100 });
        hGroup.AddChild(new VScrollBar { SkinClass = typeof(VScrollBarSkin2), PercentHeight = 100, Maximum = 1000, PageSize = 100 });
        hGroup.AddChild(new VScrollBar { SkinClass = typeof(VScrollBarSkin2), Height = 400, Maximum = 400, PageSize = 100 });
        hGroup.AddChild(new VScrollBar { SkinClass = typeof(VScrollBarSkin3), PercentHeight = 100, Maximum = 200, PageSize = 100 });
        hGroup.AddChild(new VScrollBar { SkinClass = typeof(VScrollBarSkin3), Height = 400, Maximum = 300, PageSize = 100 });

        #endregion

        #region Panels

        //hGroup.AddChild(new Spacer { PercentWidth = 50 });

        Panel panel = new MyPanel
        {
            Width = 360,
            Height = 600,
            Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
            Title = "First panel",
            StyleName = "default"
        };
        hGroup.AddChild(panel);

        panel = new MyPanel2
        {
            MaxWidth = 500,
            Height = 600,
            SkinClass = typeof (PanelSkin2),
            Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
            Title = "Second panel"
        };
        panel.SetStyle("titleColor", 0xffff00);
        hGroup.AddChild(panel);

        //hGroup.AddChild(new Spacer { PercentWidth = 50 });

        #endregion

        #region Vertical sliders

        hGroup.AddChild(new VSlider { PercentHeight = 100 });
        hGroup.AddChild(new VSlider { Width = 30, Height = 400, SkinClass = typeof(VSliderSkin2) });
        hGroup.AddChild(new VSlider { Width = 30, Height = 400, SkinClass = typeof(VSliderSkin2), Enabled = false });
        hGroup.AddChild(new VSlider { Width = 50, Height = 400, SkinClass = typeof(VSliderSkin2) });
        hGroup.AddChild(new VSlider { Width = 80, Height = 400, SkinClass = typeof(VSliderSkin3) });
        hGroup.AddChild(new VSlider { Width = 80, PercentHeight = 100, Maximum = 1000, SkinClass = typeof(VSliderSkin3) });

        #endregion

        #region Horizontal sliders

        viewport.AddChild(new HSlider { Maximum = 400, PercentWidth = 100 });
        viewport.AddChild(new HSlider { Width = 400, Maximum = 400, Height = 30, SkinClass = typeof(HSliderSkin2) });
        viewport.AddChild(new HSlider { Width = 400, Maximum = 400, Height = 50, SkinClass = typeof(HSliderSkin2) });
        viewport.AddChild(new HSlider { PercentWidth = 50, Height = 80, SkinClass = typeof(HSliderSkin3) });
        viewport.AddChild(new HSlider { PercentWidth = 100, Maximum = 1000, Height = 80, SkinClass = typeof(HSliderSkin3) });

        #endregion

    }
}