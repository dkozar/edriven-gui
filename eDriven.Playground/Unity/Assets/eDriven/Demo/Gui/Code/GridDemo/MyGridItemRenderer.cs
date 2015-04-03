using System;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Shapes;
using eDriven.Gui.Styles;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Gui.Code.GridDemo
{
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "rollOverColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "caretColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "selectionColor", Type = typeof(Color), Default = 0xaaccee)]

    public class MyGridItemRenderer : Component, IItemRenderer 
    {
        public const string ADD_BUTTON_CLICKED = "addButtonClicked";
        public const string EDIT_BUTTON_CLICKED = "editButtonClicked";

        public MyGridItemRenderer()
        {
            AddEventListener(MouseEvent.ROLL_OVER, ItemRendererRollOverHandler, EventPhase.CaptureAndTarget);
            AddEventListener(MouseEvent.ROLL_OUT, ItemRendererRollOutHandler, EventPhase.CaptureAndTarget);
        }

        #region Implementation of IItemRenderer

        private int _itemIndex;

        ///<summary>
        /// Item index
        ///</summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
            set
            {
                if (value == _itemIndex)
                    return;

                _itemIndex = value;
                InvalidateDisplayList();
            }
        }

        private bool _dragging;
        ///<summary>
        ///</summary>
        public bool Dragging
        {
            get { return _dragging; }
            set { _dragging = value; }
        }

        private string _text = string.Empty;

        ///<summary>
        ///</summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text)
                    return;

                _text = value;

                // Push the label down into the labelDisplay,
                // if it exists
                if (null != FirstNameLabel)
                    FirstNameLabel.Text = _text;
            }
        }

        private HGroup _hgroup;

        //----------------------------------
        //  labelDisplay
        //----------------------------------
        
        public TextBase FirstNameLabel;

        public TextBase LastNameLabel;

        public TextBase AgeLabel;

        public NumericStepper NsAge;

        public CheckBox ChkDrivingLicense;

        private Button _buttonInfo;
        private Button _buttonEdit;
        private Button _buttonRemove;
        
        private bool _selected;
        ///<summary>
        ///</summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    InvalidateDisplayList();
                }
            }
        }

        private bool _showsCaret;
        ///<summary>
        ///</summary>
        public bool ShowsCaret
        {
            get { return _showsCaret; }
            set
            {
                if (value == _showsCaret)
                    return;

                _showsCaret = value;
                InvalidateDisplayList();
            }
        }

        /**
         *  Flag that is set when the mouse is hovered over the item renderer.
         */
        private bool hovered;

        #endregion

        private bool _dataChanged;
        private object _data;
        public override object Data
        {
            get { 
                return _data;
            }
            set
            {
                _data = value;
                _dataChanged = true;
                InvalidateProperties();

                DispatchEvent(new FrameworkEvent(FrameworkEvent.DATA_CHANGE));
            }
        }

        private RectShape _rect;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _rect = new RectShape();
            AddChild(_rect);

            _hgroup = new HGroup { VerticalAlign = VerticalAlign.Middle, PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10 };
            AddChild(_hgroup);

            if (null == FirstNameLabel)
            {
                FirstNameLabel = new Label
                {
                    Width = 80
                };
                _hgroup.AddChild(FirstNameLabel);
            }

            if (null == LastNameLabel)
            {
                LastNameLabel = new Label
                {
                    Width = 80
                };
                _hgroup.AddChild(LastNameLabel);
            }

            if (null == AgeLabel)
            {
                AgeLabel = new Label
                {
                    Width = 20
                };
                _hgroup.AddChild(AgeLabel);
            }

            if (null == NsAge)
            {
                NsAge = new NumericStepper { Width = 60, FocusEnabled = false };
                NsAge.ValueCommit += delegate
                {
                    // update item
                    ExampleItem item = (ExampleItem)Data;
                    item.Age = (int)NsAge.Value;
                };
                _hgroup.AddChild(NsAge);
            }

            if (null == ChkDrivingLicense)
            {
                ChkDrivingLicense = new CheckBox { FocusEnabled = false };
                ChkDrivingLicense.Change += delegate
                {
                    // update item
                    ExampleItem item = (ExampleItem)Data;
                    item.DrivingLicense = ChkDrivingLicense.Selected;
                };
                _hgroup.AddChild(ChkDrivingLicense);
            }

            _hgroup.AddChild(new Spacer { PercentWidth = 100 });

            if (null == _buttonInfo)
            {
                _buttonInfo = new Button { 
                    Text = "Info", 
                    FocusEnabled = false,
                    SkinClass = typeof(ImageButtonSkin),
                    Icon = ImageLoader.Instance.Load("Icons/information")
                };
                _buttonInfo.Click += delegate
                {
                    DispatchEvent(new Event(ADD_BUTTON_CLICKED, true)); // bubbles
                };
                _hgroup.AddChild(_buttonInfo);
            }

            if (null == _buttonEdit)
            {
                _buttonEdit = new Button
                {
                    Text = "Edit",
                    FocusEnabled = false,
                    SkinClass = typeof(ImageButtonSkin),
                    Icon = ImageLoader.Instance.Load("Icons/edit")
                };
                _buttonEdit.Click += delegate
                {
                    DispatchEvent(new Event(EDIT_BUTTON_CLICKED, true)); // bubbles
                };
                _hgroup.AddChild(_buttonEdit);
            }

            if (null == _buttonRemove)
            {
                _buttonRemove = new Button
                {
                    Text = "Remove",
                    FocusEnabled = false,
                    SkinClass = typeof(ImageButtonSkin),
                    Icon = ImageLoader.Instance.Load("Icons/cancel")
                };
                _buttonRemove.ButtonDown += delegate
                                                {
                                                    var parentList = Owner as List;
                                                    if (null != parentList)
                                                    {
                                                        //Debug.Log("Removing at " + parentList.DataProvider.GetItemIndex(Data));
                                                        parentList.DataProvider.RemoveItemAt(parentList.DataProvider.GetItemIndex(Data));
                                                    }
                                                    else
                                                        Debug.LogError("Owner of item renderer is not a list");
                                                };
                _hgroup.AddChild(_buttonRemove);
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_dataChanged)
            {
                _dataChanged = false;

                if (null != _data)
                {
                    var dto = (ExampleItem)_data; // <- casting to data type

                    //_buttonAdd.Text = "Button " + dto.Age;
                    //_buttonAdd.Text = null == _data ? string.Empty : _data.ToString();

                    FirstNameLabel.Text = dto.FirstName;
                    LastNameLabel.Text = dto.LastName;
                    AgeLabel.Text = dto.Age + "";
                    NsAge.Value = dto.Age;
                    ChkDrivingLicense.Selected = dto.DrivingLicense;
                }
            }   
        }

        protected override void Measure()
        {
            base.Measure();

            MeasuredWidth = _hgroup.GetExplicitOrMeasuredWidth();
            MeasuredHeight = _hgroup.GetExplicitOrMeasuredHeight();

            MeasuredMinWidth = LayoutUtil.GetMinBoundsWidth(_hgroup);
            MeasuredMinHeight = LayoutUtil.GetMinBoundsHeight(_hgroup);
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            FirstNameLabel.Color = (Color) GetStyle("textColor");
            LastNameLabel.Color = (Color)GetStyle("textColor");
            AgeLabel.Color = (Color)GetStyle("textColor");
            
            if (_selected)
                _rect.BackgroundColor = (Color)GetStyle("selectionColor");
            else if (ShowsCaret)
                _rect.BackgroundColor = (Color)GetStyle("caretColor");
            else if (hovered)
                _rect.BackgroundColor = (Color)GetStyle("rollOverColor");
            else
                _rect.BackgroundColor = (Color)GetStyle("backgroundColor");
            
            _rect.SetActualSize(width, height); // (this is a component - no layout, so...)
            _hgroup.SetActualSize(width, height); // (this is a component - no layout, so...)
        }

        /**
         *  Mouse rollOver event handler.
         */
        // ReSharper disable MemberCanBePrivate.Global
        protected void ItemRendererRollOverHandler(Event e)
        // ReSharper restore MemberCanBePrivate.Global
        {
            //Debug.Log("Roll over");
            hovered = true;
            InvalidateDisplayList();
        }

        /**
         *  Mouse rollOut event handler.
         */
        // ReSharper disable MemberCanBePrivate.Global
        protected void ItemRendererRollOutHandler(Event e)
        // ReSharper restore MemberCanBePrivate.Global
        {
            hovered = false;
            InvalidateDisplayList();
        }
    }
}