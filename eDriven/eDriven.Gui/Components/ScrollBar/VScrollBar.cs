using System;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// Vertical scrollbar
    ///</summary>
    [Style(Name = "skinClass", Default = typeof(VScrollBarSkin))]

    public class VScrollBar : ScrollBarBase
    {
        private void UpdateMaximumAndPageSize()
        {
            var vsp = Viewport.VerticalScrollPosition;
            var viewportHeight = /*null == Viewport.Height ? 0 : */Viewport.Height;
            // Special case: if contentWidth is 0, assume that it hasn't been 
            // updated yet.  Making the maximum==hsp here avoids trouble later
            // when Range constrains value
            var cHeight = Viewport.ContentHeight;
            Maximum = (cHeight == 0) ? vsp : cHeight - viewportHeight;
            PageSize = viewportHeight;
            //Debug.Log("UpdateMaximumAndPageSize PageSize: " + PageSize);
        }

        public override IViewport Viewport
        {
            get
            {
                return base.Viewport;
            }
            set
            {
                IViewport oldViewport = base.Viewport;
                if (oldViewport == value)
                    return;
                
                if (null != oldViewport)
                {
                    oldViewport.RemoveEventListener(MouseEvent.MOUSE_WHEEL, MouseWheelHandler);
                    RemoveEventListener(MouseEvent.MOUSE_WHEEL, MouseWheelHandler);
                }
                
                base.Viewport = value;

                if (null != value)
                {
                    UpdateMaximumAndPageSize();
                    Value = value.VerticalScrollPosition;
                    value.AddEventListener(MouseEvent.MOUSE_WHEEL, MouseWheelHandler);
                    AddEventListener(MouseEvent.MOUSE_WHEEL, MouseWheelHandler);  
                }
            }
        }

        /**
         *  
         */
        override protected float PointToValue(float x, float y)
        {
            if (null == Thumb || null == Track)
                return 0;

            float r = Track.Height /*getLayoutBoundsHeight()*/ - Thumb.Height/*getLayoutBoundsHeight()*/;
            return Minimum + ((r != 0) ? (y / r) * (Maximum - Minimum) : 0); 
        }

        /**
         *  
         */
        override protected void UpdateSkinDisplayList()
        {
            if (null == Thumb || null == Track)
                return;

            //if (Id == "test")
            //    Debug.Log("UpdateSkinDisplayList: " + Width + ", " + Height);

            float trackSize = Track.Height; //getLayoutBoundsHeight();
            float range = Maximum - Minimum;

            float thumbPosTrackY = 0;
            float thumbSize = trackSize;
            if (range > 0)
            {
                if (!(bool) GetStyle("fixedThumbSize"))
                {
                    thumbSize = Mathf.Min((PageSize/(range + PageSize))*trackSize, trackSize);
                    thumbSize = Mathf.Max(Thumb.MinHeight, thumbSize);
                }
                else
                {
                    thumbSize = Thumb.Height;
                }
                
                // calculate new thumb position.
                thumbPosTrackY = (Value - Minimum) * ((trackSize - thumbSize) / range);
            }

            if (!(bool)GetStyle("fixedThumbSize"))
                Thumb.SetActualSize(Thumb.GetExplicitOrMeasuredWidth(), thumbSize); /*setLayoutBoundsSize*/
            if ((bool)GetStyle("autoThumbVisibility"))
                Thumb.Visible = thumbSize < trackSize;

            // convert thumb position to parent's coordinates.
            Point thumbPos = Track.LocalToGlobal(new Point(0, thumbPosTrackY));
            float thumbPosParentY = Thumb.Parent.GlobalToLocal(thumbPos).Y;

            //Debug.Log("Moving thumb to: " + Thumb.X + ", " +  Mathf.Round(thumbPosParentY));

            //Thumb.setLayoutBoundsPosition(Mathf.Round(thumbPosParentX), Thumb.getLayoutBoundsY());
            Thumb.Move(Thumb.X, Mathf.Round(thumbPosParentY));
        }

        protected override void SetValue(float value)
        {
            //Debug.Log("SetValue: " + value);
            base.SetValue(value);
            if (null != Viewport)
                Viewport.VerticalScrollPosition = value;
        }

        override public void ChangeValueByPage(bool increase)
        {
            float oldPageSize = 0f;
            if (null != Viewport)
            {
                // Want to use ScrollBarBase's changeValueByPage() implementation to get the same
                // animated behavior for scrollbars with and without viewports.
                // For now, just change pageSize temporarily and call the superclass
                // implementation.
                oldPageSize = PageSize;
                PageSize = Mathf.Abs(Viewport.GetVerticalScrollPositionDelta(
                    (increase) ? NavigationUnit.PageDown : NavigationUnit.PageUp));
            }
            base.ChangeValueByPage(increase);
            if (null != Viewport)
                PageSize = oldPageSize;
        }

        override public void ChangeValueByStep(bool increase)
        {
            float oldStepSize = 0f;
            if (null != Viewport)
            {
                // Want to use ScrollBarBase's changeValueByStep() implementation to get the same
                // animated behavior for scrollbars with and without viewports.
                // For now, just change pageSize temporarily and call the superclass
                // implementation.
                oldStepSize = StepSize;
                StepSize = Math.Abs(Viewport.GetVerticalScrollPositionDelta(
                    (increase) ? NavigationUnit.Down : NavigationUnit.Up));
            }
            base.ChangeValueByStep(increase);
            if (null != Viewport)
                StepSize = oldStepSize;
        }

        protected override void PartAdded(string partName, object instance)
        {
            if (instance == Thumb)
            {
                Thumb.Top = null;
                Thumb.Bottom = null;
                Thumb.VerticalCenter = null;
            }      

            base.PartAdded(partName, instance);
        }

        internal override void ViewportVerticalScrollPositionChangeHandler(Event e)
        {
            //Debug.Log("ViewportVerticalScrollPositionChangeHandler: " + Viewport.VerticalScrollPosition);
            if (null != Viewport)
                Value = Viewport.VerticalScrollPosition;
        }

        internal override void ViewportResizeHandler(Event e)
        {
            //Debug.Log("ViewportResizeHandler");
            if (null != Viewport)
                UpdateMaximumAndPageSize();
        }

        internal override void ViewportContentHeightChangeHandler(Event e)
        {
            if (null != Viewport)
            {
                //var viewportHeight = /*null == Viewport.Height ? 0 : */Viewport.Height;
                Maximum = Viewport.ContentHeight - Viewport.Height;
            }
        }

        private void MouseWheelHandler(Event e)
        {
            //Debug.Log("VScrollBar->MouseWheelHandler: " + e.Target);
            IViewport vp = Viewport;
            if (e.DefaultPrevented || null == vp || !vp.Visible)
                return;

            /* If the scrollbar is a part of the scroller, but is not visible, do not process mouse wheel
             * this way we are giving a chance for the horizontal scroll to work */
            if (!Visible)
                return;

            MouseEvent me = (MouseEvent) e;
            //Debug.Log(me.CurrentEvent.delta.y);
            var delta = me.CurrentEvent.delta.y;
            int nSteps = (int)Math.Abs(delta);
            //Debug.Log("MouseWheelHandler. delta: " + delta);

            // Scroll event.delta "steps".  
            nSteps = 1; // TEMP
            
            NavigationUnit navigationUnit = (delta > 0) ? NavigationUnit.Down : NavigationUnit.Up;
            for (int vStep = 0; vStep < nSteps; vStep++)
            {
                //Debug.Log("vStep: " + vStep);
                float vspDelta = vp.GetVerticalScrollPositionDelta(navigationUnit);
                //Debug.Log("  vspDelta: " + vspDelta);
                //if (null != vspDelta)
                vp.VerticalScrollPosition += vspDelta;
            }

            e.PreventDefault();
        }
    }
}
