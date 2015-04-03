using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.Util;
using Assets.eDriven.Demo.Tweens;
using System.Collections.Generic;
using Action=eDriven.Animation.Action;
using Event = eDriven.Core.Events.Event;
using Random=System.Random;

/// <summary>
/// </summary>
// ReSharper disable UnusedMember.Global
public class DataGroupDemo : Gui
// ReSharper restore UnusedMember.Global
{

    private ArrayList _dataProvider;

    private readonly Random _random = new Random();

    #region Effects

    private readonly TweenFactory _windowShow = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }), // dialog_open
        new Jumpy()
        )
    );

    private readonly TweenFactory _overlayShow = new TweenFactory(
        new Sequence(
            new FadeIn()
        )
    );

    #endregion

    protected override void OnInitialize()
    {
        base.OnInitialize();

        //Alert.DefaultSkin = typeof(AlertSkin2);
        Dialog.AddedEffect = _windowShow;
        ModalOverlay.DefaultSkin = typeof(ModalOverlaySkin2);
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

        Button button = new Button
        {
            Text = "Add data",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/add"),
            AutoRepeat = true
        };
        button.ButtonDown += delegate
        {
            _dataProvider.AddItem("data " + _random.Next(1, 100));
        };
        toolbar.AddContentChild(button);

        #endregion

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof (ScrollerSkin2),
            PercentWidth = 100,
            PercentHeight = 100
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        Group viewport = new Group
        {
            MouseEnabled = true,
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
        //AddChild(viewport);

        #endregion

        #region Data controls

        List<object> source = new List<object> {"Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six"};

        _dataProvider = new ArrayList(source);

        var factory = new ItemRendererFactory<DefaultItemRenderer>();

        /* LISTS */

        #region HGroup

        HGroup hGroup1 = new HGroup
                    {
                        PaddingLeft = 10,
                        PaddingRight = 10,
                        PaddingTop = 10,
                        PaddingBottom = 10,
                        Gap = 20
                    };

        //hbox.SetStyle("showBackground", true);
        //hbox.SetStyle("backgroundColor", RgbColor.FromHex(0x004CFF).ToColor());
        viewport.AddChild(hGroup1);

        #endregion

        HGroup hGroup = new HGroup();
        viewport.AddChild(hGroup);

        /* DATA GROUPS */

        #region Data group 1

        hGroup1 = new HGroup
        {
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            Gap = 50
        };
        hGroup.AddChild(hGroup1);

        DataGroup dataGroup = new DataGroup
        {
            Layout = new VerticalLayout(),
            DataProvider = _dataProvider,
            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            ItemRendererFunction = delegate(object item)
            {
                return factory;
            }
        };
        hGroup1.AddChild(dataGroup);

        #endregion

        #region Data group 2

        hGroup1 = new HGroup
        {
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            Gap = 50
        };
        hGroup.AddChild(hGroup1);

        dataGroup = new DataGroup
        {
            Layout = new TileLayout {RequestedColumnCount = 2},
            //Width = 200, Height = 200,
            DataProvider = _dataProvider,
            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            ItemRendererFunction = delegate
            {
                return factory;
            }
            //ClipAndEnableScrolling = true
        };
        hGroup1.AddChild(dataGroup);

        #endregion

        #region Data group 3

        hGroup1 = new HGroup
        {
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            Gap = 50
        };
        hGroup.AddChild(hGroup1);

        dataGroup = new DataGroup
        {
            Layout = new TileLayout { RequestedColumnCount = 4 },
            //Width = 200, Height = 200,
            DataProvider = _dataProvider,
            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            ItemRendererFunction = delegate
            {
                return factory;
            }
            //ClipAndEnableScrolling = true
        };
        hGroup1.AddChild(dataGroup);

        #endregion

        #endregion

        #region VGroup

        VGroup vgroup = new VGroup();
        hGroup.AddChild(vgroup);

        #endregion

        #region List 1

        List list = new List
        {
            //Layout = new TileLayout { RequestedColumnCount = 4 },
            Width = 200,
            Height = 200,
            DataProvider = _dataProvider,
            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            ItemRendererFunction = delegate
            {
                return factory;
            }
        };
        vgroup.AddChild(list);

        #endregion

        #region List 2

        list = new List
        {
            //Layout = new TileLayout { RequestedColumnCount = 3 },
            Width = 200,
            Height = 245,
            DataProvider = _dataProvider,
            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
            ItemRendererFunction = delegate
            {
                return factory;
            }
        };
        vgroup.AddChild(list);

        #endregion

        #region List 3

        list = new List
        {
            Id = "list3",
            Width = 400,
            Height = 600,
            DataProvider = _dataProvider,
            SkinClass = typeof(ListSkin2),
            ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
        };
        hGroup.AddChild(list);

        #endregion

        #region Process renderer click

        AddEventListener(BigItemRenderer.ADD_BUTTON_CLICKED, delegate(Event e)
        {
            BigItemRenderer itemRenderer = (BigItemRenderer)e.Target;
            Alert.Show("Info", itemRenderer.Data.ToString(), AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, ImageLoader.Instance.Load("Icons/information")));
        });

        #endregion
            
    }
}