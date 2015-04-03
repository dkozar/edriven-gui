using System;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    [Style(Name = "skinClass", Default = typeof(HScrollBarSkin))]

    ///<summary>
    ///</summary>
    public class HScrollBar : ScrollBarBase
    {
        private void UpdateMaximumAndPageSize()
        {
            var hsp = Viewport.HorizontalScrollPosition;
            var viewportWidth = /*null == Viewport.Width ? 0 : */Viewport.Width;
            // Special case: if contentWidth is 0, assume that it hasn't been 
            // updated yet.  Making the maximum==hsp here avoids trouble later
            // when Range constrains value
            var cWidth = Viewport.ContentWidth;
            Maximum = (cWidth == 0) ? hsp : cWidth - viewportWidth;
            PageSize = viewportWidth;
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
                    RemoveEventListener(MouseEvent.MOUSE_WHEEL, HSBMouseWheelHandler, EventPhase.CaptureAndTarget);
                }
                
                base.Viewport = value;
                
                if (null != value)
                {
                    UpdateMaximumAndPageSize();
                    Value = value.HorizontalScrollPosition;
                    
                    // The HSB viewport mouse wheel listener is added at a low priority so that 
                    // if a VSB installs a listener it will run first and cancel the event.
                    value.AddEventListener(MouseEvent.MOUSE_WHEEL, MouseWheelHandler, -50); // lower priority

                    // The HSB mouse wheel listener stops propagation and redispatches its event, 
                    // so we listen during the capture phase.
                    AddEventListener(MouseEvent.MOUSE_WHEEL, HSBMouseWheelHandler, EventPhase.CaptureAndTarget); 
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

            float r = Track.Width /*getLayoutBoundsWidth()*/ - Thumb.Width/*getLayoutBoundsWidth()*/;
            return Minimum + ((r != 0) ? (x / r) * (Maximum - Minimum) : 0); 
        }

        /**
         *  
         */
        override protected void UpdateSkinDisplayList()
        {
            if (null == Thumb || null == Track)
                return;
            
            float trackSize = Track.Width; //getLayoutBoundsWidth();
            float range = Maximum - Minimum;
            
            Point thumbPos;
            float thumbPosTrackX = 0;
            float thumbPosParentX;
            float thumbSize = trackSize;
            if (range > 0)
            {
                if (!(bool) GetStyle("fixedThumbSize"))
                {
                    thumbSize = Mathf.Min((PageSize/(range + PageSize))*trackSize, trackSize);
                    thumbSize = Mathf.Max(Thumb.MinWidth, thumbSize);
                }
                else
                {
                    thumbSize = Thumb.Width;
                }
                
                // calculate new thumb position.
                thumbPosTrackX = (Value - Minimum) * ((trackSize - thumbSize) / range);
            }
            
            if (!(bool) GetStyle("fixedThumbSize"))
                Thumb.SetActualSize(thumbSize, Thumb.GetExplicitOrMeasuredHeight()); /*setLayoutBoundsSize*/
            if ((bool) GetStyle("autoThumbVisibility"))
                Thumb.Visible = thumbSize < trackSize;
            
            // convert thumb position to parent's coordinates.
            thumbPos = Track.LocalToGlobal(new Point(thumbPosTrackX, 0));
            thumbPosParentX = Thumb.Parent.GlobalToLocal(thumbPos).X;
            
            //Thumb.setLayoutBoundsPosition(Mathf.Round(thumbPosParentX), Thumb.getLayoutBoundsY());
            //Debug.Log("Moving thumb to " + thumbPosParentX);

            Thumb.Move(Mathf.Round(thumbPosParentX), Thumb.Y);
        }
        
        protected override void SetValue(float value)
        {
            base.SetValue(value);

            if (null != Viewport)
                Viewport.HorizontalScrollPosition = value;
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
                PageSize = Math.Abs(Viewport.GetHorizontalScrollPositionDelta(
                    (increase) ? NavigationUnit.PageRight : NavigationUnit.PageLeft));
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
                StepSize = Math.Abs(Viewport.GetHorizontalScrollPositionDelta(
                    (increase) ? NavigationUnit.Right : NavigationUnit.Left));
            }
            base.ChangeValueByStep(increase);
            if (null != Viewport)
                StepSize = oldStepSize;
        }

        protected override void PartAdded(string partName, object instance)
        {
            if (instance == Thumb)
            {
                Thumb.Left = null;
                Thumb.Right = null;
                Thumb.HorizontalCenter = null;
            }      

            base.PartAdded(partName, instance);
        }

        internal override void ViewportHorizontalScrollPositionChangeHandler(Event e)
        {
            if (null != Viewport)
                Value = Viewport.HorizontalScrollPosition;
        }

        internal override void ViewportResizeHandler(Event e)
        {
            //Debug.Log("ViewportResizeHandler");
            if (null != Viewport)
                UpdateMaximumAndPageSize();
        }

        internal override void ViewportContentWidthChangeHandler(Event e)
        {
            if (null != Viewport)
            {
                //var viewportHeight = /*null == Viewport.Height ? 0 : */Viewport.Height;
                Maximum = Viewport.ContentWidth - Viewport.Width;
            }
        }

        private void MouseWheelHandler(Event e)
        {
            IViewport vp = Viewport;
            if (e.DefaultPrevented || null == vp || !vp.Visible)
                return;

            //Debug.Log("HScrollBar MouseWheelHandler");
            
            MouseEvent me = (MouseEvent)e;
            var delta = me.CurrentEvent.delta.y;
            int nSteps = (int)Math.Abs(delta);

            nSteps = 1; // TEMP

            // Scroll event.delta "steps".  
            //Debug.Log("delta: " + delta);
            NavigationUnit navigationUnit = (delta > 0) ? NavigationUnit.Right : NavigationUnit.Left;
            for (int hStep = 0; hStep < nSteps; hStep++)
            {
                float hspDelta = vp.GetHorizontalScrollPositionDelta(navigationUnit);
                //if (null != hspDelta)
                vp.HorizontalScrollPosition += hspDelta;
            }

            e.PreventDefault();
        }

        /// <summary>
        /// This setting is specific to horizontal scroll bar<br/>
        /// When set to true, the scrollbar will scroll horizontally<br/>
        /// </summary>
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
        public bool MouseWheelScrollsHorizontally = true;
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnassignedField.Global

        /**
         *  
         *  Redispatch HSB mouse wheel events to the viewport to give the VSB's listener, if any,
         *  an opportunity to handle/cancel them.  If no VSB exists, mouseWheelHandler (see above)
         *  will process the event.
         */
        private void HSBMouseWheelHandler(Event e)
        {
            //Debug.Log("HSBMouseWheelHandler");
            if (MouseWheelScrollsHorizontally) {
                MouseWheelHandler(e);
                return;
            }
            
            IViewport vp = Viewport;
            if (e.DefaultPrevented || null == vp || !vp.Visible) {
                return;
            }
            
            // redispatch as viewport's event
            e.StopPropagation(); // note: do not cancel!
            vp.DispatchEvent(e);
        }
    }
}
