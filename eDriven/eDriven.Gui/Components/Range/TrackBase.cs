using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Managers;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    #region Event metadata

    ///<summary>
    /// A base track class
    ///</summary>
    [DefaultEvent(Event.CHANGE)]

    [Event(Name = Event.CHANGE, Type = typeof(Event), Bubbles = false)]
    [Event(Name = FrameworkEvent.CHANGE_START, Type = typeof(FrameworkEvent), Bubbles = false)]
    [Event(Name = FrameworkEvent.CHANGE_END, Type = typeof(FrameworkEvent), Bubbles = false)]
    [Event(Name = TrackBaseEvent.THUMB_DRAG, Type = typeof(TrackBaseEvent), Bubbles = false)]
    [Event(Name = TrackBaseEvent.THUMB_PRESS, Type = typeof(TrackBaseEvent), Bubbles = false)]
    [Event(Name = TrackBaseEvent.THUMB_RELEASE, Type = typeof(TrackBaseEvent), Bubbles = false)]

    #endregion

    [Style(Name = "slideDuration", Type = typeof(float), Default = 0.3f)]

    public class TrackBase : Range
    {
        /// <summary>
        /// Track base
        /// </summary>
        public TrackBase()
        {
            AddEventListener(FrameworkEvent.ADD, AddedToStageHandler);
            AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
        }

        public override void Dispose()
        {
            base.Dispose();

            RemoveEventListener(FrameworkEvent.ADD, AddedToStageHandler);
            RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);

            var sm = SystemEventDispatcher.Instance;
            sm.RemoveEventListener(MouseEvent.MOUSE_MOVE,
                SystemMouseMoveHandler);
            sm.RemoveEventListener(MouseEvent.MOUSE_UP,
                SystemMouseUpHandler);
        }

        /**
         * change handler
         * */
        private MulticastDelegate _change;

        /// <summary>
        /// Scroll handler
        /// </summary>
        public MulticastDelegate Change
        {
            get
            {
                if (null == _change)
                    _change = new MulticastDelegate(this, Event.CHANGE);
                return _change;
            }
            set
            {
                _change = value;
            }
        }

        // ReSharper disable MemberCanBeProtected.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global

        ///<summary>
        /// Thumb button
        ///</summary>
        [SkinPart(Required = false)]

        public Button Thumb;


        ///<summary>
        /// Track button
        ///</summary>
        [SkinPart(Required = false)]
        public Button Track;

        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore MemberCanBeProtected.Global

        private DisplayObject _mouseDownTarget;

        public override float Maximum
        {
            get
            {
                return base.Maximum;
            }
            set
            {
                if (value == base.Maximum)
                    return;

                base.Maximum = value;
                InvalidateDisplayList();
            }
        }

        public override float Minimum
        {
            get
            {
                return base.Minimum;
            }
            set
            {
                if (value == base.Minimum)
                    return;

                base.Minimum = value;
                InvalidateDisplayList();
            }
        }

        public override float Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (value == base.Value)
                    return;

                base.Value = value;
                InvalidateDisplayList();
            }
        }

        protected override void SetValue(float value)
        {
            //Debug.Log("SetValue: " + value);
            base.SetValue(value);

            InvalidateDisplayList();
        }

// ReSharper disable UnusedParameter.Global
        protected virtual float PointToValue(float x, float y)
// ReSharper restore UnusedParameter.Global
        {
            return Minimum;
        }

        ///<summary>
        /// Changes value by step
        ///</summary>
        ///<param name="increase"></param>
        public override void ChangeValueByStep(bool increase)
        {
            float prevValue = Value;
        
            base.ChangeValueByStep(increase);
            
            if (Value != prevValue)
                DispatchEvent(new Event(Event.CHANGE));
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == Thumb)
            {
                //Debug.Log("Thumb added: " + this);
                Thumb.AddEventListener(MouseEvent.MOUSE_DOWN, ThumbMouseDownHandler);
                Thumb.AddEventListener(ResizeEvent.RESIZE, ThumbResizeHandler);
                Thumb.AddEventListener(FrameworkEvent.UPDATE_COMPLETE, ThumbUpdateCompleteHandler);
                Thumb.StickyHighlighting = true; // let the thumb stay in down state even if we move the mouse away
                Thumb.Enabled = Enabled;
            }
            else if (instance == Track)
            {
                Track.AddEventListener(MouseEvent.MOUSE_DOWN, TrackMouseDownHandler);
                Track.AddEventListener(ResizeEvent.RESIZE, TrackResizeHandler);
                Track.Enabled = Enabled;
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == Thumb)
            {
                //Debug.Log("Thumb removed");
                Thumb.RemoveEventListener(MouseEvent.MOUSE_DOWN, ThumbMouseDownHandler);
                Thumb.RemoveEventListener(ResizeEvent.RESIZE, ThumbResizeHandler);
                Thumb.RemoveEventListener(FrameworkEvent.UPDATE_COMPLETE, ThumbUpdateCompleteHandler);
            }
            else if (instance == Track)
            {
                Track.RemoveEventListener(MouseEvent.MOUSE_DOWN, TrackMouseDownHandler);
                Track.RemoveEventListener(ResizeEvent.RESIZE, TrackResizeHandler);
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);
            UpdateSkinDisplayList();
        }

        private Point _clickOffset;

// ReSharper disable VirtualMemberNeverOverriden.Global
        protected virtual void UpdateSkinDisplayList() { }
// ReSharper restore VirtualMemberNeverOverriden.Global
        
        /**
         *  
         *  Redraw whenever added to the stage to ensure the calculations
         *  in updateSkinDisplayList() are correct.
         */
// ReSharper disable UnusedMember.Local
        private void AddedToStageHandler(Event e)
// ReSharper restore UnusedMember.Local
        {
            UpdateSkinDisplayList();
        }
        
        /**
         *  
         */
        private void TrackResizeHandler(Event e)
        {
            UpdateSkinDisplayList();
        }

        /**
         *  
         */
        private void ThumbResizeHandler(Event e)
        {
            UpdateSkinDisplayList();
        }
        
        /**
         *  
         */
        private void ThumbUpdateCompleteHandler(Event e)
        {
            //Debug.Log("ThumbUpdateCompleteHandler");
            UpdateSkinDisplayList();
            Thumb.RemoveEventListener(FrameworkEvent.UPDATE_COMPLETE, ThumbUpdateCompleteHandler);
        }

        /**
         *  
         *  Handle mouse-down events on the scroll thumb. Records 
         *  the mouse down point in _clickOffset.
         */
        protected virtual void ThumbMouseDownHandler(Event e)
        {
            e.CancelAndStopPropagation();

            DragManager.IsDragging = true;

            //Debug.Log("ThumbMouseDownHandler: " + e);
            MouseEvent me = (MouseEvent) e;
            var sm = SystemEventDispatcher.Instance;
            sm.AddEventListener(MouseEvent.MOUSE_MOVE, 
                SystemMouseMoveHandler);
            sm.AddEventListener(MouseEvent.MOUSE_UP,
                SystemMouseUpHandler);

            _clickOffset = Thumb.GlobalToLocal(me.GlobalPosition);

            DispatchEvent(new TrackBaseEvent(TrackBaseEvent.THUMB_PRESS));
            DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
        }

        /**
         *  
         *  Capture mouse-move events anywhere on or off the stage.
         *  First, we calculate the new value based on the new position
         *  using calculateNewValue(). Then, we move the thumb to 
         *  the new value's position. Last, we set the value and
         *  dispatch a "change" event if the value changes. 
         */
        protected virtual void SystemMouseMoveHandler(Event e)
        {
            if (null == Track)
                return;

            MouseEvent me = (MouseEvent) e;

            Point p = Track.GlobalToLocal(me.GlobalPosition);
            float newValue = PointToValue(p.X - _clickOffset.X, p.Y - _clickOffset.Y);
            newValue = NearestValidValue(newValue, SnapInterval);

            if (newValue != Value)
            {
                SetValue(newValue);
                DispatchEvent(new TrackBaseEvent(TrackBaseEvent.THUMB_DRAG));
                DispatchEvent(new Event(Event.CHANGE));
            }
        }

        protected virtual void SystemMouseUpHandler(Event e)
        {
            //Debug.Log("SystemMouseUpHandler");

            DragManager.IsDragging = false;

            var sm = SystemEventDispatcher.Instance;
            sm.RemoveEventListener(MouseEvent.MOUSE_MOVE,
                SystemMouseMoveHandler);
            sm.RemoveEventListener(MouseEvent.MOUSE_UP,
                SystemMouseUpHandler);
            
            DispatchEvent(new TrackBaseEvent(TrackBaseEvent.THUMB_RELEASE));
            DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_END));
        }

        protected virtual void TrackMouseDownHandler(Event e) { }

        /**
         *  
         *  Capture any mouse down event and listen for a mouse up event
         */  
        private void MouseDownHandler(Event e)
        {
            //var sm = SystemEventDispatcher.Instance;
            var dispatcher = MouseEventDispatcher.Instance;
            dispatcher.AddEventListener(MouseEvent.MOUSE_UP, SystemMouseUpSomewhereHandler, EventPhase.CaptureAndTarget);
            _mouseDownTarget = (DisplayObject) e.Target;
        }

        private void SystemMouseUpSomewhereHandler(Event e)
        {
            //Debug.Log("SystemMouseUpSomewhereHandler");
            //var sm = SystemEventDispatcher.Instance;
            var dispatcher = MouseEventDispatcher.Instance;
            dispatcher.RemoveEventListener(MouseEvent.MOUSE_UP, SystemMouseUpSomewhereHandler, EventPhase.CaptureAndTarget);

            //Debug.Log("e.Target: " + e.Target);
            var dlm = (DisplayListMember)e.Target;
            // If we got a mouse down followed by a mouse up on a different target in the skin, 
            // we want to dispatch a click event. 
            if (_mouseDownTarget != e.Target && e is MouseEvent && Contains(dlm))
            {
                MouseEvent mEvent = e as MouseEvent;
                // Convert the mouse coordinates from the target to the TrackBase
                Point mousePoint = new Point(mEvent.LocalPosition.X, mEvent.LocalPosition.Y);
                mousePoint = GlobalToLocal(dlm.LocalToGlobal(mousePoint));

                var me2 = new MouseEvent(MouseEvent.CLICK, mEvent.Bubbles, mEvent.Cancelable)
                {
                    LocalPosition = new Point(mousePoint.X, mousePoint.Y),
                    RelatedObject = null,
                    Control = mEvent.Control,
                    Shift = mEvent.Shift,
                    Alt = mEvent.Alt,
                    ButtonDown = mEvent.ButtonDown,
                    CurrentEvent = mEvent.CurrentEvent
                };
                DispatchEvent(me2);
            }

            _mouseDownTarget = null;
        }
    }
}