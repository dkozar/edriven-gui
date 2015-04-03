using System.Collections.Generic;
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
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Tweens;
using Event = eDriven.Core.Events.Event;
using Random = System.Random;

/// <summary>
/// </summary>
public class ComboBoxDemo : Gui
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

        #endregion

        List<object> source = new List<object> { "Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six" };

        _dataProvider = new ArrayList(source);

        HGroup hGroup = new HGroup();
        viewport.AddChild(hGroup);

        //List<object> source = new List<object> {"Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six"};

        var factory = new ItemRendererFactory<DefaultItemRenderer>();

        VGroup vGroup = new VGroup { PercentHeight = 100 };
        hGroup.AddChild(vGroup);

        HGroup hGroup2 = new HGroup
        {
            PercentWidth = 100
        };
        vGroup.AddChild(hGroup2);

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
        hGroup.AddChild(list);

        #endregion

        vGroup.AddChild(new Spacer { PercentHeight = 100 });

        #region Drop down lists

        VGroup vGroup2 = new VGroup();
        vGroup.AddChild(vGroup2);

        DropDownList dropDownList1 = new DropDownList
        {
            DataProvider = _dataProvider
        };
        vGroup2.AddChild(dropDownList1);

        DropDownList dropDownList2 = new DropDownList
        {
            Width = 200,
            DataProvider = _dataProvider,
            Prompt = "Pick an item"

        };
        vGroup2.AddChild(dropDownList2);

        DropDownList dropDownList3 = new DropDownList
        {
            Width = 200,
            SkinClass = typeof(DropDownListSkin2),
            DataProvider = _dataProvider/*,
            SelectedIndex = 0*/
        };
        vGroup2.AddChild(dropDownList3);

        ComboBox comboBox = new ComboBox
        {
            Width = 200,
            DataProvider = _dataProvider/*,
            SelectedIndex = 0*/
        };
        vGroup2.AddChild(comboBox);

        vGroup2.Plugins.Add(new TabManager
        {
            TabChildren = new List<DisplayListMember> { dropDownList1, dropDownList2, dropDownList3, comboBox }
        });

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