using System.Collections;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.Shapes;
using eDriven.Gui.Styles;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Demo.Gui.Code.LoadData
{
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "rollOverColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "caretColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "selectionColor", Type = typeof(Color), Default = 0xaaccee)]

    public class LabelItemRenderer : Component, IItemRenderer 
    {
        private static readonly Hashtable ButtonStyles = new Hashtable { { "cursor", "pointer" } };

        public LabelItemRenderer()
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
                if (null != LabelDisplay)
                    LabelDisplay.Text = _text;
            }
        }

        private HGroup _hgroup;

        //----------------------------------
        //  labelDisplay
        //----------------------------------
        
        /**
         *  Optional item renderer label component. 
         *  This component is used to determine the value of the 
         *  <code>baselinePosition</code> property in the host component of 
         *  the item renderer.
         */
        public TextBase LabelDisplay;

        private Button _buttonShow;
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
         *  
         *  Flag that is set when the mouse is hovered over the item renderer.
         */
        private bool hovered;

        #endregion

        private object _data;
        public override object Data
        {
            get { 
                return _data;
            }
            set
            {
                _data = value;
                DispatchEvent(new FrameworkEvent(FrameworkEvent.DATA_CHANGE));
            }
        }

        private RectShape _rect;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _rect = new RectShape();
            //{
            //    Left = 0, Right = 0, Top = 0, Bottom = 0
            //};
            AddChild(_rect);

            _hgroup = new HGroup { VerticalAlign = VerticalAlign.Middle, PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10 };
            AddChild(_hgroup);

            if (null == LabelDisplay)
            {
                LabelDisplay = new Label
                {
                    PercentWidth = 100,
                    MinWidth = 100 // ?
                };
                _hgroup.AddChild(LabelDisplay);
                if (_text != string.Empty) {
                    LabelDisplay.Text = _text;
                }
            }

            _hgroup.AddChild(new Spacer { PercentWidth = 100 });

            if (null == _buttonShow)
            {
                _buttonShow = new Button { 
                    Text = "Show", 
                    FocusEnabled = false,
                    SkinClass = typeof(ImageButtonSkin),
                    Icon = ImageLoader.Instance.Load("Icons/accept"),
                    Styles = ButtonStyles
                };
                _buttonShow.ButtonDown += delegate
                {
                    DispatchEvent(new Event("showImage", true)); // bubbling!
                };
                _hgroup.AddChild(_buttonShow);
            }

            if (null == _buttonRemove)
            {
                _buttonRemove = new Button
                {
                    Text = "Remove",
                    FocusEnabled = false,
                    SkinClass = typeof(ImageButtonSkin),
                    Icon = ImageLoader.Instance.Load("Icons/cancel"),
                    Styles = ButtonStyles
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

        /*protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_dataChanged)
            {
                _dataChanged = false;
                if (null != _buttonAdd)
                    _buttonAdd.Text = null == _data ? string.Empty : _data.ToString();
            }   
        }*/

        protected override void Measure()
        {
            base.Measure();

            //Debug.Log("Measuring... LabelDisplay: " + LabelDisplay);

            // label has padding of 3 on left and right and padding of 5 on top and bottom.
            MeasuredWidth = _hgroup.GetExplicitOrMeasuredWidth(); // +6;
            MeasuredHeight = _hgroup.GetExplicitOrMeasuredHeight(); // +10;

            MeasuredMinWidth = LayoutUtil.GetMinBoundsWidth(_hgroup); // + 6;
            MeasuredMinHeight = LayoutUtil.GetMinBoundsHeight(_hgroup); // + 10;
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            LabelDisplay.Color = (Color) GetStyle("textColor");

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
         *  
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
         *  
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