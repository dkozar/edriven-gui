using System.Collections.Generic;
using Assets.eDriven.Demo;
using Assets.eDriven.Demo.Gui.Code.GridDemo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Tweens;
using eDriven.Gui.Managers;
using Action = eDriven.Animation.Action;
using Random = System.Random;

/// <summary>
/// </summary>
public class GridDemo : Gui
{
    private ArrayList _dataProvider;

    private readonly Random _random = new Random();

    private Label _lblSum;
    private Label _lblNumberOfSelectedItems;

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
            _dataProvider.AddItem(
                new ExampleItem
                {
                    FirstName = "First" + _random.Next(1, 100),
                    LastName = "Last" + _random.Next(1, 100),
                    Age = _random.Next(1, 80),
                    DrivingLicense = _random.Next(0, 2) == 1
                }
            );
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

        #region Fill the data provider

        List<string> names = new List<string>
        {
            "Failure",
            "Teaches",
            "Success",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six"
        };

        List<object> source = new List<object>();
        foreach (var theName in names)
        {
            source.Add(
                new ExampleItem
                {
                    FirstName = theName,
                    LastName = theName,
                    Age = _random.Next(1, 10),
                    DrivingLicense = _random.Next(0, 2) == 1
                }
                );
        }

        _dataProvider = new ArrayList(source);

        #endregion

        HGroup hGroup = new HGroup { PercentWidth = 100, PercentHeight = 100 };
        viewport.AddChild(hGroup);

        #region Grid (header + list + footer)

        VGroup vGroup = new VGroup { PercentWidth = 100, PercentHeight = 100, Gap = 0 };
        hGroup.AddChild(vGroup);

        #region Header

        /* Implement your own header logic here */

        /*HGroup header = new HGroup {PercentWidth = 100, Gap = 0};
        vGroup.AddChild(header);

        for (int i = 0; i < 5; i++)
        {
            Button btnCol = new Button // example header
            {
                Text = "Column " + (i + 1),
                ToggleMode = true,
                PercentWidth = 100
            };
            btnCol.Click += delegate(Event e)
            {
                // sort, filder and refresh (do list.DataProvider = list.DataProvider)
            };
            header.AddChild(btnCol);
        }*/

        #endregion

        #region List

        List list = new List
        {
            Id = "list1",
            PercentWidth = 100,
            PercentHeight = 100,
            DataProvider = _dataProvider,
            LabelField = "FirstName", // this must be set due to list logic legacy (the field must exist in DTO)
            SkinClass = typeof (ListSkin2),
            ItemRenderer = new ItemRendererFactory<MyGridItemRenderer>()
        };
        vGroup.AddChild(list);

        #endregion

        #region Footer

        HGroup footer = new HGroup { PercentWidth = 100, Gap = 30, PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 0 };
        vGroup.AddChild(footer);

        _lblSum = new Label {Text = "Age sum: ", StyleName = "footer" };
        footer.AddChild(_lblSum);

        _lblNumberOfSelectedItems = new Label { Text = "Driving licenses: ", StyleName = "footer" };
        footer.AddChild(_lblNumberOfSelectedItems);

        UpdateFooter();

        #endregion

        #endregion

        #region Process renderer click

        AddEventListener(MyGridItemRenderer.ADD_BUTTON_CLICKED, delegate(Event e)
        {
            IItemRenderer itemRenderer = (IItemRenderer)e.Target;
            Alert.Show("Info", itemRenderer.Data.ToString(), AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, (object) ImageLoader.Instance.Load("Icons/information")));
        });

        AddEventListener(MyGridItemRenderer.EDIT_BUTTON_CLICKED, delegate(Event e)
        {
            var editor = new ExamplePopupEditor { 
                Title = "Edit", 
                Icon = ImageLoader.Instance.Load("Icons/edit"), 
                Data = ((MyGridItemRenderer) e.Target).Data, 
                CloseOnEsc = true
            };
            editor.AddEventListener(CloseEvent.CLOSE, delegate
            {
                PopupManager.Instance.RemovePopup(editor);
            });

            PopupManager.Instance.AddPopup(editor, true);
        });

        #endregion

        #region Process property change

        _dataProvider.AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, delegate(Event e)
        {
            //PropertyChangeEvent pce = (PropertyChangeEvent) e;
            //Debug.Log(pce.NewValue);
            UpdateFooter();
        });

        #endregion

    }

    private void UpdateFooter()
    {
        var sum = 0;
        foreach (var item in _dataProvider.Source)
        {
            ExampleItem ei = (ExampleItem) item;
            sum += ei.Age;
        }

        _lblSum.Text = "Age sum: " + sum;

        var count = 0;
        foreach (var item in _dataProvider.Source)
        {
            ExampleItem ei = (ExampleItem) item;
            if (ei.DrivingLicense)
                count ++;
        }

        _lblNumberOfSelectedItems.Text = "Driving licenses: " + count;
    }
}