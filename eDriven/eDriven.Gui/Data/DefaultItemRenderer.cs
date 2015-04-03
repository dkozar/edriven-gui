using System;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using eDriven.Gui.Shapes;
using eDriven.Gui.Styles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Data
{
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xfffffe)]
    [Style(Name = "rollOverColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "caretColor", Type = typeof(Color), Default = 0xdeeeff)]
    [Style(Name = "selectionColor", Type = typeof(Color), Default = 0xaaccee)]

    ///<summary>
    ///</summary>
    public class DefaultItemRenderer : Component, IItemRenderer 
    {
        public DefaultItemRenderer()
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

        //----------------------------------
        //  labelDisplay
        //----------------------------------

        public TextBase LabelDisplay;

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
                if (value == _data)
                    return;

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

            if (null == LabelDisplay)
            {
                LabelDisplay = new Label();
                AddChild(LabelDisplay);
                if (_text != string.Empty)
                    LabelDisplay.Text = _text;
            }
        }

        protected override void Measure()
        {
            base.Measure();

            //Debug.Log("Measuring... LabelDisplay: " + LabelDisplay);

            // label has padding of 3 on left and right and padding of 5 on top and bottom.
            MeasuredWidth = LayoutUtil.GetPreferredBoundsWidth(LabelDisplay) + 6;
            MeasuredHeight = LayoutUtil.GetPreferredBoundsHeight(LabelDisplay) + 10;

            MeasuredMinWidth = LayoutUtil.GetMinBoundsWidth(LabelDisplay) + 6;
            MeasuredMinHeight = LayoutUtil.GetMinBoundsHeight(LabelDisplay) + 10;
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            LabelDisplay.Color = (Color) GetStyle("textColor");

            //hovered = Contains(Gui.MouseTarget, true);

            //Color backgroundColor;
            //var drawBackground = true;
            if (Selected)
                _rect.BackgroundColor = (Color)GetStyle("selectionColor");
            else if (ShowsCaret)
                _rect.BackgroundColor = (Color)GetStyle("caretColor");
            else if (hovered)
                _rect.BackgroundColor = (Color)GetStyle("rollOverColor");
            else
                _rect.BackgroundColor = (Color) GetStyle("backgroundColor");

            //else
            //{
            //    var alternatingColors:Array = getStyle("alternatingItemColors");
                
            //    if (alternatingColors && alternatingColors.length > 0)
            //    {
            //        // translate these colors into uints
            //        styleManager.getColorNames(alternatingColors);
                    
            //        backgroundColor = alternatingColors[itemIndex % alternatingColors.length];
            //    }
            //    else
            //    {
            //        // don't draw background if it is the contentBackgroundColor. The
            //        // list skin handles the background drawing for us.
            //        drawBackground = false;
            //    }
            //}
            
            //if (ShowsCaret)
            //{
            //    //graphics.lineStyle(1, getStyle("selectionColor"));
            //    //graphics.drawRect(0.5, 0.5, unscaledWidth-1, unscaledHeight-1);
            //}
            //else 
            //{
            //    //graphics.lineStyle();
            //    //graphics.drawRect(0, 0, unscaledWidth, unscaledHeight);
            //}
                
            // make sure our width/height is in the min/max for the label
            float childWidth = width - 6;
            childWidth = Math.Max(LayoutUtil.GetMinBoundsWidth(LabelDisplay), Math.Min(LayoutUtil.GetMaxBoundsWidth(LabelDisplay), childWidth));
            
            float childHeight = height - 10;
            childHeight = Math.Max(LayoutUtil.GetMinBoundsHeight(LabelDisplay), Math.Min(LayoutUtil.GetMaxBoundsHeight(LabelDisplay), childHeight));
            
            // set the label's position and size
            LabelDisplay.SetLayoutBoundsSize(childWidth, childHeight);
            LabelDisplay.SetLayoutBoundsPosition(6, 5);

            _rect.SetActualSize(width, height); // (this is a component - no layout, so...)
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
