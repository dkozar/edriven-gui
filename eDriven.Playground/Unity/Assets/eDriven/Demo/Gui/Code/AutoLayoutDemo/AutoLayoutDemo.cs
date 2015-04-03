using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

public class AutoLayoutDemo : Gui
{
    #region Properties

    public int NumberOfButtons = 10;

    #endregion

    #region Members

    private Button _btnAddRect;
    private DropDownList _dropDownList;
    private int _count;
    private Panel _panel;
    
    #endregion
    
    #region Lifecycle methods

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Layout = new VerticalLayout
        {
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            HorizontalAlign = HorizontalAlign.Left,
            Gap = 10
        };
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Controls

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        _btnAddRect = new Button
        {
            Text = "New button",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
            FocusEnabled = false,
            AutoRepeat = true
        };
        _btnAddRect.ButtonDown += delegate { AddButton(); };
        toolbar.AddContentChild(_btnAddRect);

        Button btnClear = new Button
        {
            Text = "Clear",
            SkinClass = typeof(ImageButtonSkin),
            Icon = Resources.Load<Texture>("Icons/cancel"),
            FocusEnabled = false
        };
        btnClear.Click += delegate
        {
            _panel.RemoveAllContentChildren();
            _count = 0;
        };
        toolbar.AddContentChild(btnClear);

        toolbar.AddContentChild(new Label { Text = "Action: ", Color = Color.black });

        _dropDownList = new DropDownList
        {
            DataProvider = new ArrayList(new System.Collections.Generic.List<object>
                                                    {
                                                        "Position",
                                                        "Size"
                                                    }),
            //SelectedIndex = 0 // TODO: DataGroup->_indexToRenderer.Count bug! (GetContentChildAt(int index))
            //SelectedItem = "Position"
        };
        toolbar.AddContentChild(_dropDownList);

        _panel = new Panel
        {
            Title = "Choose the action and click this panel's children",
            Icon = Resources.Load<Texture>("Icons/star"),
            SkinClass = typeof(PanelSkin2),
            Width = 600,
            Height = 600,
            Layout = new VerticalLayout
            {
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                HorizontalAlign = HorizontalAlign.Left,
                Gap = 10
            }
        };
        AddChild(_panel);
        
        CheckBox chkAutoLayout = new CheckBox
        {
            //SkinClass = typeof(ImageButtonSkin),
            //Icon = Resources.Load<Texture>("Icons/shape_move_forwards"),
            Text = "Auto layout",
            FocusEnabled = false,
            StyleName = "checkbox",
            Selected = true
        };
        chkAutoLayout.Click += delegate
        {
            Debug.Log("Setting panel's auto layout to " + chkAutoLayout.Selected);
            _panel.AutoLayout = chkAutoLayout.Selected;
        };
        _panel.ControlBarGroup.AddChild(chkAutoLayout);

        AddChild(
            new Label { Text = "This is the single-line label"}
        );

        AddChild(
            new Label
            {
                Text = @"When AutoLayout is turned ON for a container, the layout mechanism will react on each child's size change and run the layout on parent container. However, it won'tdo anything on position change.

When AutoLayout is turned OFF, the layout mechanism won't react on child size change, except at creation time (the layout is always being run at least once).

You would post probably turn the AutoLayout OFF just before running a tween effecting children size.

After the tween is finished, you should turn AutoLayout ON again.",
                Multiline = true,
                WordWrap = true,
                PercentWidth = 100,
                Alignment = TextAnchor.UpperLeft/*,
                ExpandHeightByMeasuringText = true*/
            }
        );

        #endregion

        for (int i = 0; i < NumberOfButtons; i++)
        {
            AddButton();
        }
    }

    protected override void OnCreationComplete()
    {
        base.OnCreationComplete();

        _panel.AddEventListener(ButtonEvent.BUTTON_DOWN, delegate(Event e)
        {
            if (!_panel.ContentChildren.Contains((DisplayListMember)e.Target)) // border or scroller clicked
                return;

            Button btn = e.Target as Button;
            if (null == btn)
                return;

            switch (_dropDownList.SelectedIndex)
            {
                case 0:
                    btn.X += 10;
                    btn.Y += 10;
                    break;
                case 1:
                    btn.Width += 10;
                    btn.Height += 10;
                    break;
            }
        });
    }

    #endregion

    #region Helper

    private void AddButton()
    {
        _count++;
        Button btn = new Button
                         {
                             FocusEnabled = false,
                             Width = 200,
                             Height = 200,
                             MinWidth = 200,
                             MinHeight = 200,
                             SkinClass = typeof(ButtonSkin5),
                             Icon = ImageLoader.Instance.Load("Icons/star_big"),
                             Text = "Button " + _count,
                             Tooltip = "Resizable Button",
                             Data = _count,
                             AutoRepeat = true
                         };

        _panel.AddContentChild(btn);
    }

    #endregion

}