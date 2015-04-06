using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    [Style(Name = "liveDragging", Type = typeof(bool), Default = true)]

    public class SliderBase : TrackBase
    {
        public SliderBase()
        {
            Maximum = 10;
        }
        
        [SkinPart(Required = false)]
        public IFactory DataTip;

        /**
         *  
         */
        private object dataFormatter;

        /**
         *  
         */
        private Point dataTipInitialPosition;
        
        /**
         *  
         */
        private object dataTipInstance;

        /**
         *  
         */
        private float slideToValue;
        
        /**
         *  
         */
        private bool isKeyDown;

        /**
         *  
         *  Location of the mouse down event on the thumb, relative to the thumb's origin.
         *  Used to update the value property when the mouse is dragged. 
         */
        private Point clickOffset;

        private object _dataTipFormatFunction;
        public object DataTipFormatFunction
        {
            get { return _dataTipFormatFunction; }
            set { _dataTipFormatFunction = value; }
        }

        /**
         *  Number of decimal places to use for the data tip text.
         *  A value of 0 means to round all values to an integer.
         *  This value is ignored if <code>dataTipFormatFunction</code> is defined.
         * 
         *  Default: 2
         */
        public int dataTipPrecision = 2;

        private float _pendingValue = 0;
        public float PendingValue
        {
            get { return _pendingValue; }
            set
            {
                if (value == _pendingValue)
                    return;
                _pendingValue = value;
                InvalidateDisplayList();
            }
        }

        /**
         *  If set to <code>true</code>, shows a data tip during user interaction
         *  containing the current value of the slider. In addition, the skinPart,
         *  <code>dataTip</code>, must be defined in the skin in order to 
         *  display a data tip. 
         *  Default: true
         */
        public bool showDataTip = true;

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            // Prevent focus on our children so that focus remains with the SliderBase
            if (instance == Thumb)
            {
                Thumb.Enabled = Enabled;
                Thumb.FocusEnabled = false;
            }
            else if (instance == Track) {
                Track.Enabled = Enabled;
                Track.FocusEnabled = false;
            }
        }

        protected override void SetValue(float value)
        {
            _pendingValue = value;
            base.SetValue(value);
        }

        /**
         *  Used to position the data tip when it is visible. Subclasses must implement
         *  this function. 
         *  
         *  Param: dataTipInstance The <code>dataTip</code> instance to update and position
         *  Param: initialPosition The initial position of the <code>dataTip</code> in the skin
         */
        protected virtual void UpdateDataTip(/*IDataRenderer*/ object dataTipInstance, Point initialPosition)
        {
            // Override in the subclasses
        }

        /**
         *  
         *  Returns a formatted version of the value
         */
        private object FormatDataTipText(float value)
        {
            object formattedValue = null;
                
            if (DataTipFormatFunction != null)
            {
                //formattedValue = DataTipFormatFunction(value); 
            }
            else
            {
                //if (dataFormatter == null)
                //    dataFormatter = new NumberFormatter();
                    
                //dataFormatter.Precision = dataTipPrecision;
                
                //formattedValue = dataFormatter.Format(value);
            }
            
            return formattedValue;
        }

        #region Animation

        ///**
        // *  
        // *  Handles events from the Animation that runs the animated slide.
        // *  We just call setValue() with the current animated value
        // */
        //private function animationUpdateHandler(animation:Animation):void
        //{
        //    pendingValue = animation.currentValue["value"];
        //}

        //    /**
        //     *  
        //     *  Handles end event from the Animation that runs the animated slide.
        //     *  We dispatch the "changeEnd" event at this time, after the animation
        //     *  is done since each animation occurs after a user interaction.
        //     */
        //    private function animationEndHandler(animation:Animation):void
        //{
        //    setValue(slideToValue);
                
        //    dispatchEvent(new Event(Event.CHANGE));
        //    dispatchEvent(new FlexEvent(FlexEvent.CHANGE_END));
        //}
            
        //    /**
        //     *  
        //     *  Stops a running animation prematurely and sets the value
        //     *  of the slider to the current pendingValue. We also dispatch
        //     *  a "changeEnd" event since the user has started another interaction.
        //     */
        //    private function stopAnimation():void
        //{
        //    animator.stop();
                
        //    setValue(nearestValidValue(pendingValue, snapInterval));
                
        //    dispatchEvent(new Event(Event.CHANGE));
        //    dispatchEvent(new FlexEvent(FlexEvent.CHANGE_END));
        //}

        #endregion

        protected override void ThumbMouseDownHandler(Event e)
        {
            // finish previous animation
            //if (null != animator && animator.isPlaying)
            //    stopAnimation();

            MouseEvent me = (MouseEvent) e;

            base.ThumbMouseDownHandler(e);
            clickOffset = Thumb.GlobalToLocal(me.GlobalPosition);
                    
            // Popup a dataTip only if we have a SkinPart and the boolean flag is true
            //if (dataTip && showDataTip && enabled)
            //{
            //    dataTipInstance = IDataRenderer(createDynamicPartInstance("dataTip"));
                
            //    dataTipInstance.data = FormatDataTipText(
            //        nearestValidValue(pendingValue, snapInterval));
                
            //    var tipAsUIComponent:Component = dataTipInstance as Component;
                
            //    // Allow styles to be inherited from SliderBase.
            //    if (tipAsUIComponent)
            //    {
            //        tipAsUIComponent.owner = this;
            //        tipAsUIComponent.isPopUp = true;
            //    }

            //    systemManager.toolTipChildren.addChild(DisplayObject(dataTipInstance));
                
            //    // Force the dataTip to render so that we have the correct size since
            //    // updateDataTip might need the size
            //    if (tipAsUIComponent)
            //    {
            //        tipAsUIComponent.validateNow();
            //        tipAsUIComponent.setActualSize(tipAsUIComponent.getExplicitOrMeasuredWidth(),
            //                                       tipAsUIComponent.getExplicitOrMeasuredHeight());
            //    }
                
            //    dataTipInitialPosition = new Point(DisplayObject(dataTipInstance).x, 
            //                                        DisplayObject(dataTipInstance).y);   
            //    updateDataTip(dataTipInstance, dataTipInitialPosition);
            //}
        }

        override protected void SystemMouseMoveHandler(Event e)
        {
            if (null == Track)
                return;

            MouseEvent me = (MouseEvent) e;

            Point p = Track.GlobalToLocal(me.GlobalPosition);
            float newValue = PointToValue(p.X - clickOffset.X, p.Y - clickOffset.Y);
            newValue = NearestValidValue(newValue, SnapInterval);
            
            if (newValue != PendingValue)
            {
                DispatchEvent(new TrackBaseEvent(TrackBaseEvent.THUMB_DRAG));
                if ((bool) GetStyle("liveDragging"))
                {
                    SetValue(newValue);
                    DispatchEvent(new Event(Event.CHANGE));
                }
                else
                {
                    PendingValue = newValue;
                }
            }
                      
            if (null != dataTipInstance && showDataTip)
            { 
                //dataTipInstance.data = FormatDataTipText(pendingValue);
                
                //// Force the dataTip to render so that we have the correct size since
                //// updateDataTip might need the size
                //var tipAsUIComponent:Component = dataTipInstance as Component; 
                //if (tipAsUIComponent)
                //{
                //    tipAsUIComponent.validateNow();
                //    tipAsUIComponent.setActualSize(tipAsUIComponent.getExplicitOrMeasuredWidth(),tipAsUIComponent.getExplicitOrMeasuredHeight());
                //}
                
                UpdateDataTip(dataTipInstance, dataTipInitialPosition);
            }
        }

        protected override void SystemMouseUpHandler(Event e)
        {
            if (!(bool)GetStyle("liveDragging") && (Value != PendingValue))
            {
                SetValue(PendingValue);
                DispatchEvent(new Event(Event.CHANGE));
            }

            if (null != dataTipInstance)
            {
                //removeDynamicPartInstance("dataTip", dataTipInstance);
                //systemManager.toolTipChildren.removeChild(DisplayObject(dataTipInstance));
                dataTipInstance = null;
            }
            
            base.SystemMouseUpHandler(e);
        }

        protected override void KeyDownHandler(Event e)
        {
            if (e.DefaultPrevented)
                return;

            KeyboardEvent ke = (KeyboardEvent) e;

            //if (animator && animator.isPlaying)
            //    stopAnimation();
            
            float prevValue = Value;
            float newValue;
            
            switch (ke.KeyCode)
            {
                case KeyCode.DownArrow:
                case KeyCode.LeftArrow:
                {
                    newValue = NearestValidValue(PendingValue - StepSize, SnapInterval);
                    
                    if (prevValue != newValue)
                    {
                        if (!isKeyDown)
                        {
                            DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
                            isKeyDown = true;
                        }
                        SetValue(newValue);
                        DispatchEvent(new Event(Event.CHANGE));
                    }
                    e.PreventDefault();
                    break;
                }

                case KeyCode.UpArrow:
                case KeyCode.RightArrow:
                {
                    newValue = NearestValidValue(PendingValue + StepSize, SnapInterval);
                    
                    if (prevValue != newValue)
                    {
                        if (!isKeyDown)
                        {
                            DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
                            isKeyDown = true;
                        }
                        SetValue(newValue);
                        DispatchEvent(new Event(Event.CHANGE));
                    }
                    e.PreventDefault();
                    break;
                }
                
                case KeyCode.Home:
                {
                    Value = Minimum;
                    if (Value != prevValue)
                        DispatchEvent(new Event(Event.CHANGE));
                    e.PreventDefault();
                    break;
                }

                case KeyCode.End:
                {
                    Value = Maximum;
                    if (Value != prevValue)
                        DispatchEvent(new Event(Event.CHANGE));
                    e.PreventDefault();
                    break;
                }
            }
        }

        protected override void KeyUpHandler(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent) e;

            switch (ke.KeyCode)
            {
                case KeyCode.DownArrow:
                case KeyCode.LeftArrow:
                case KeyCode.UpArrow:
                case KeyCode.RightArrow:
                {
                    if (isKeyDown)
                    {
                        // Dispatch "change" event only after a repeat occurs.
                        DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_END));
                        isKeyDown = false;
                    }
                    e.PreventDefault();
                    break;
                }
            }
        }

        protected override void TrackMouseDownHandler(Event e)
        {
            if (!Enabled)
                return;
             
            MouseEvent me = (MouseEvent) e;

            // Offset the track-relative coordinates of this event so that
            // the thumb will end up centered over the mouse down location.
            float thumbW = (null != Thumb) ? Thumb.Width : 0;
            float thumbH = (null != Thumb) ? Thumb.Height : 0;
            float offsetX = me.GlobalPosition.X - (thumbW / 2);
            float offsetY = me.GlobalPosition.Y - (thumbH / 2);
            Point p = Track.GlobalToLocal(new Point(offsetX, offsetY));

            float newValue = PointToValue(p.X, p.Y);
            newValue = NearestValidValue(newValue, SnapInterval);

            if (newValue != PendingValue)
            {
                SetValue(newValue);
                DispatchEvent(new Event(Event.CHANGE));
            }
        }

        public float Percentage
        {
            get { return Value / Maximum; }
            set
            {
                value = Mathf.Clamp(value, 0f, 1f); // 0 - 100 %
                SetValue(value * Maximum);
            }
        }

        public override bool Enabled
        { // moje, zbog skin statea
            get
            {
                return base.Enabled;
            }
            set
            {
                if (value == base.Enabled)
                    return;

                base.Enabled = value;
                if (null != Thumb)
                    Thumb.Enabled = value;
                if (null != Track)
                    Track.Enabled = value;
            }
        }
    }
}