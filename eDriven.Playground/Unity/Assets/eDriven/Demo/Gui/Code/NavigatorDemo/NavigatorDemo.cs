using Assets.eDriven.Demo;
using Assets.eDriven.Demo.Tweens;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Util;
using System.Collections.Generic;
using Event = eDriven.Core.Events.Event;
using Random=System.Random;

/// <summary>
/// </summary>
// ReSharper disable UnusedMember.Global
public class NavigatorDemo : eDriven.Gui.Gui
// ReSharper restore UnusedMember.Global
{
//// ReSharper disable UnusedMember.Local
//    void Awake()
//// ReSharper restore UnusedMember.Local
//    {
//        //TweenErrorSignal.Instance.Connect(TweenErrorSlot);
//    }

    private ArrayList _dataProvider;

    private ButtonBar _buttonBar;
    private ViewStack _viewstack;

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

        Dialog.AddedEffect = _windowShow;
        ModalOverlay.DefaultSkin = typeof(ModalOverlaySkin2);
        ModalOverlay.AddedEffect = _overlayShow;

        Layout = new VerticalLayout {Gap = 0};
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

        button = new Button
        {
            Text = "Previous tab",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/previous")
        };
        button.Click += delegate
        {
            _buttonBar.SelectedIndex--;
            _viewstack.Previous();
        };
        toolbar.AddContentChild(button);

        button = new Button
        {
            Text = "Next tab",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/next")
        };
        button.Click += delegate
        {
            _buttonBar.SelectedIndex++;
            _viewstack.Next();
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

        viewport.AddChild(new Label {Text = "ButtonBar:"});

        #region Button bar

        _buttonBar = new ButtonBar
        {
            DataProvider = new ArrayList(new List<object> {"List 1", "List 2", "List 3", "List 4", "List 5"}),
            SkinClass = typeof(ButtonBarSkin2),
            RequireSelection = true
        };
        _buttonBar.AddEventListener(Event.CHANGE, delegate(Event e)
        {
            IndexChangeEvent ice = e as IndexChangeEvent;
            if (null != ice)
            {
                int newIndex = ice.NewIndex;
                //Debug.Log("Changed to: " + newIndex);
                _viewstack.SelectedIndex = newIndex;
            }
        });
        viewport.AddChild(_buttonBar);

        #endregion
        
        #region ViewStack

        viewport.AddChild(new Label { Text = "ViewStack (having 5 lists as children):" });

        _viewstack = new ViewStack { ResizeToContent = true };
        viewport.AddChild(_viewstack);

        #endregion

        /* Factory is used for creating renderer instances */
        var factory = new ItemRendererFactory<DefaultItemRenderer>();

        /* LISTS */

        #region List 1

        List list = new List
                        {
                            Id = "list1",
                            //Layout = new TileLayout { RequestedColumnCount = 4 },
                            Width = 200, Height = 200,
                            DataProvider = _dataProvider,
                            //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
                            
                            /* ItemRendererFunction is used for switching between different factories, based on the supplied item */
                            ItemRendererFunction = delegate { return factory; }
                        };
        _viewstack.AddChild(list);

        #endregion

        #region List 2

        list = new List
                    {
                        Id = "list2",
                        //Layout = new TileLayout { RequestedColumnCount = 3 },
                        Width = 200, Height = 245,
                        DataProvider = _dataProvider,
                        //ItemRenderer = new ItemRendererFactory <DefaultItemRenderer>(),
                        ItemRendererFunction = delegate { return factory; }
                    };
        _viewstack.AddChild(list);

        #endregion

        #region List 3

        list = new List
                    {
                        Id = "list3",
                        Width = 400,
                        Height = 450,
                        DataProvider = _dataProvider,
                        SkinClass = typeof(ListSkin2),
                        ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
                    };
        _viewstack.AddChild(list);

        #endregion

        #region List 4

        list = new List
                    {
                        Id = "list4",
                        Width = 600, Height = 800,
                        Layout = new TileLayout { HorizontalGap = 0, VerticalGap = 0, RequestedRowCount = 4, RequestedColumnCount = 3 },
                        DataProvider = _dataProvider,
                        SkinClass = typeof(ListSkin2),
                        ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
                    };
        _viewstack.AddChild(list);

        #endregion

        #region List 5 (horizontal)

        list = new List
                    {
                        Id = "list5",
                        Width = 800,
                        Height = 100,
                        Layout = new HorizontalLayout { Gap = 0, RequestedColumnCount = 3 },
                        DataProvider = _dataProvider,
                        SkinClass = typeof(ListSkin2),
                        ItemRenderer = new ItemRendererFactory<BigItemRenderer>()
                    };
        _viewstack.AddChild(list);

        #endregion

        #region Process renderer click

        AddEventListener(BigItemRenderer.ADD_BUTTON_CLICKED, delegate(Event e)
        {
            IItemRenderer itemRenderer = (IItemRenderer)e.Target;
            Alert.Show("Info", itemRenderer.Data.ToString(), AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, ImageLoader.Instance.Load("Icons/information")));
        });

        #endregion
    }
}