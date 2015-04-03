using System.Collections.Generic;
using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Tweens;
using Random = System.Random;

/// <summary>
/// </summary>
public class ListDemo : Gui
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
            Icon = ImageLoader.Instance.Load("Icons/star"),
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

        #endregion

        List<object> source = new List<object> { "Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six" };

        _dataProvider = new ArrayList(source);

        HGroup hGroup = new HGroup { PercentHeight = 100 };
        viewport.AddChild(hGroup);

        var factory = new ItemRendererFactory<DefaultItemRenderer>();

        VGroup vGroup2 = new VGroup { PercentHeight = 100 };
        hGroup.AddChild(vGroup2);

        HGroup hGroup2 = new HGroup
        {
            PercentWidth = 100
        };
        vGroup2.AddChild(hGroup2);

        /* LISTS */

        #region List 1

        List list = new List
                        {
                            Id = "list1",
                            //Layout = new TileLayout { RequestedColumnCount = 4 },
                            Width = 200, Height = 200,
                            DataProvider = _dataProvider,
                            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
                            ItemRendererFunction = delegate
                                                        {
                                                            return factory;
                                                        }
                        };
        hGroup2.AddChild(list);

        #endregion

        #region List 2

        list = new List
                    {
                        Id = "list2",
                        //Layout = new TileLayout { RequestedColumnCount = 3 },
                        Width = 200, Height = 245,
                        DataProvider = _dataProvider,
                        //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
                        ItemRendererFunction = delegate
                                                    {
                                                        return factory;
                                                    }
                    };
        hGroup2.AddChild(list);

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
        hGroup2.AddChild(list);

        #endregion
            
        #region List 4

        list = new List
                    {
                        Id = "list4",
                        PercentHeight = 100,
                        MinWidth = 400,
                        Layout = new TileLayout { HorizontalGap = 0, VerticalGap = 0, RequestedColumnCount = 3 },
                        DataProvider = _dataProvider,
                        SkinClass = typeof(ListSkin2),
                        ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
                    };
        hGroup.AddChild(list);

        #endregion

        #region List 5 (horizontal)

        vGroup2.AddChild(new Spacer { PercentHeight = 100 });

        list = new List
                    {
                        Id = "list5",
                        PercentWidth = 100,
                        Height = 100,
                        Layout = new HorizontalLayout { Gap = 0, RequestedColumnCount = 3 },
                        DataProvider = _dataProvider,
                        SkinClass = typeof(ListSkin2),
                        ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
                    };
        vGroup2.AddChild(list);

        #endregion

        #region Process renderer click

        AddEventListener(BigItemRenderer.ADD_BUTTON_CLICKED, delegate(Event e)
        {
            BigItemRenderer itemRenderer = (BigItemRenderer) e.Target;
            Alert.Show("Info", itemRenderer.Data.ToString(), AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, ImageLoader.Instance.Load("Icons/information")));
        });

        #endregion
    }
}