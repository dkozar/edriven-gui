using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Components
{
    #region Event metadata

    /// <summary>
    /// Button component
    /// </summary>
    [DefaultEvent(MouseEvent.CLICK)]

    [Event(Name = ButtonEvent.BUTTON_DOWN, Type = typeof(ButtonEvent), Bubbles = true)]
    [Event(Name = ButtonEvent.BUTTON_UP, Type = typeof(ButtonEvent), Bubbles = true)]
    [Event(Name = ButtonEvent.PRESS, Type = typeof(ButtonEvent), Bubbles = true)]
    [Event(Name = ButtonEvent.CHANGE, Type = typeof(ButtonEvent), Bubbles = true)]

    #endregion

    #region Style metadata

    [Style(Name = "skinClass", Default = typeof(ButtonSkin))]
    [Style(Name = "repeatDelay", Type = typeof(float), Default = 0.5f)]
    [Style(Name = "repeatInterval", Type = typeof(float), Default = 0.035f)]

    #endregion

    [SkinStates("up", "over", "down", "disabled", "upAndSelected", "overAndSelected", "downAndSelected", "disabledAndSelected")]

    public class Button : SkinnableComponent
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Properties

        #region Event handlers

        private MulticastDelegate _press;
        /// <summary>
        /// The event that fires when the is clicked/keypressed
        ///</summary>
        [Event(Name = ButtonEvent.PRESS, Type = typeof(ButtonEvent), Bubbles = true)]
        public MulticastDelegate Press
        {
            get
            {
                if (null == _press)
                    _press = new MulticastDelegate(this, ButtonEvent.PRESS);
                return _press;
            }
            set
            {
                _press = value;
            }
        }

        private MulticastDelegate _change;
        /// <summary>
        /// The event that fires when the toggle state is changed
        ///</summary>
        [Event(Name = ButtonEvent.CHANGE, Type = typeof(ButtonEvent), Bubbles = true)]
        public MulticastDelegate Change
        {
            get
            {
                if (null == _change)
                    _change = new MulticastDelegate(this, ButtonEvent.CHANGE);
                return _change;
            }
            set
            {
                _change = value;
            }
        }

        #endregion

        private bool _toggleModeChanged;
        private bool _toggleMode;
        /// <summary>
        /// Is the button in toggle mode
        /// </summary>
        public bool ToggleMode
        {
            get { return _toggleMode; }
            set
            {
                _toggleMode = value;
                _toggleModeChanged = true;
                InvalidateProperties();
            }
        }

        //private bool _textChanged;
        private string _text = string.Empty;
        /// <summary>
        /// Button Label
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (null != LabelDisplay)
                {
                    LabelDisplay.Text = _text;
                    //LabelDisplay.Visible = LabelDisplay.IncludeInLayout = true;
                }
            }
        }

        //private bool _iconChanged;
        private Texture _icon;
        /// <summary>
        /// The icon texture
        /// </summary>
        public Texture Icon
        {
// ReSharper disable UnusedMember.Global
            get { return _icon; }
// ReSharper restore UnusedMember.Global
            set
            {
                _icon = value;

                if (null != IconDisplay) {
                    IconDisplay.Texture = _icon;
                    //IconDisplay.Visible = IconDisplay.IncludeInLayout = true;
                }
            }
        }
        
        /// <summary>
        /// A button ID needed for identification
        /// </summary>
        public string ButtonId;

        /// <summary>
        /// Should the button toggle by own clicks
        /// This is being turned off in lists, where the button shouldn't toggle by itself,
        /// instead the list takes care of the selection
        /// </summary>
        public bool ToggleByOwnEvent = true;

        private bool _selected;
        /// <summary>
        /// Toggle state
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == _selected)
                    return;

                _selected = value;
                //Debug.Log("Changed! " + _selected);
                InvalidateSkinState();

                if (HasBubblingEventListener(ButtonEvent.CHANGE))
                    DispatchEvent(new Event(ButtonEvent.CHANGE, true));
            }
        }

        #endregion

        /*protected override void Render()
        {
            base.Render();

            if (Id == "hb")
            {
                Debug.Log("RenderingRect: " + RenderingRect + "; LocalRenderingRect: " + LocalRenderingRect);   
            }
        }*/

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Button()
        {
            MouseChildren = false;
            MouseEnabled = true;

            FocusEnabled = true;
            HighlightOnFocus = true;

            ActionKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.Space });

            //AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);

            AddEventListener(MouseEvent.CLICK, OnClickOrKeyUp);

            ProcessKeys = true;
            AddEventListener(KeyboardEvent.KEY_UP, OnClickOrKeyUp);

            MinWidth = 10;
            MinHeight = 10;

            AddHandlers();
        }

        #endregion

        #region Event handlers

        private void AddHandlers()
        {
            // TODO: Examine why do we need EventPhase.CaptureAndTarget: 
            AddEventListener(MouseEvent.ROLL_OVER, MouseEventHandler, EventPhase.CaptureAndTarget);
            AddEventListener(MouseEvent.ROLL_OUT, MouseEventHandler, EventPhase.CaptureAndTarget);
            AddEventListener(MouseEvent.MOUSE_DOWN, MouseEventHandler);
            AddEventListener(MouseEvent.MOUSE_UP, MouseEventHandler);
            AddEventListener(MouseEvent.CLICK, MouseEventHandler);
        }

        private void RemoveHandlers()
        {
            RemoveEventListener(MouseEvent.ROLL_OVER, MouseEventHandler, EventPhase.CaptureAndTarget);
            RemoveEventListener(MouseEvent.ROLL_OUT, MouseEventHandler, EventPhase.CaptureAndTarget);
            RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseEventHandler);
            RemoveEventListener(MouseEvent.MOUSE_UP, MouseEventHandler);
            RemoveEventListener(MouseEvent.CLICK, MouseEventHandler);
        }

        private bool _hovered;
        public bool Hovered
        {
            get { return _hovered; }
            set
            {
                if (value == _hovered)
                    return;

                _hovered = value;
                InvalidateButtonState();
            }
        }

        private void MouseEventHandler(Event e)
        {
            //Debug.Log("MouseEventHandler: " + e);
            MouseEvent mouseEvent = (MouseEvent)e;
            //Debug.Log("e: " + e.Type + "; " + mouseEvent.ButtonDown);
            switch (e.Type)
            {
                case MouseEvent.ROLL_OVER:
                    {
                        // if the user rolls over while holding the mouse button
                        if (mouseEvent.ButtonDown && !MouseCaptured)
                            return;
                        Hovered = true;
                        break;
                    }

                case MouseEvent.ROLL_OUT:
                    {
                        Hovered = false;
                        break;
                    }

                case MouseEvent.MOUSE_DOWN:
                    {
                        // When the button is down we need to listen for mouse events outside the button so that
                        // we update the state appropriately on mouse up.  Whenever mouseCaptured changes to false,
                        // it will take care to remove those handlers.
                        //Debug.Log("mousedown");
                        SystemManager.Instance.MouseUpSignal.Connect(MouseUpSlot, true);
                        MouseCaptured = true;
                        break;
                    }

                case MouseEvent.MOUSE_UP:
                {
                    //Debug.Log("mouseup");
                    // Call buttonReleased() if we mouse up on the button and if the mouse
                    // was captured before.
                    if (e.Target == this)
                    {
                        Hovered = true;

                        if (_mouseCaptured)
                        {
                            ButtonReleased();
                            MouseCaptured = false;
                        }
                    }
                    break;
                }

                case MouseEvent.CLICK:
                {
                    if (!Enabled)
                        e.CancelAndStopPropagation();
                    else
                        ClickHandler(e);
                    return;
                }
            }
        }

        private void ClickHandler(Event e)
        {
            // override in subclass
        }

        private bool _mouseCaptured;
        protected bool MouseCaptured
        {
            get { return _mouseCaptured; }
            set
            {
                //Debug.Log("MouseCaptured: " + value);
                _mouseCaptured = value;

                InvalidateButtonState();

                // System mouse handlers are not needed when the button is not mouse captured
                if (!value)
                    SystemManager.Instance.MouseUpSignal.Disconnect(MouseUpSlot);
            }
        }

        #endregion

        // ReSharper disable UnusedMember.Global

        private MulticastDelegate _buttonDown;
        /// <summary>
        /// Fires when the button is down
        /// </summary>
        [Event(Name = ButtonEvent.BUTTON_DOWN, Type = typeof(ButtonEvent), Bubbles = true)]
        public MulticastDelegate ButtonDown
        {
            get
            {
                if (null == _buttonDown)
                    _buttonDown = new MulticastDelegate(this, ButtonEvent.BUTTON_DOWN);
                return _buttonDown;
            }
            set
            {
                _buttonDown = value;
            }
        }

        private MulticastDelegate _buttonUp;
        /// <summary>
        /// Fires when the button is up
        /// </summary>
        [Event(Name = ButtonEvent.BUTTON_UP, Type = typeof(ButtonEvent), Bubbles = true)]
        public MulticastDelegate ButtonUp
        {
            get
            {
                if (null == _buttonUp)
                    _buttonUp = new MulticastDelegate(this, ButtonEvent.BUTTON_UP);
                return _buttonUp;
            }
            set
            {
                _buttonUp = value;
            }
        }

        /*public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);

            if (styleProp == "backgroundColor")
            {
                Debug.Log("Button, backgroundColor: " + GetStyle("backgroundColor"));
                Debug.Log("InheritingStyles: " + InheritingStyles);
                Debug.Log("NonInheritingStyles: " + NonInheritingStyles);
            }
        }*/

        // ReSharper restore UnusedMember.Global
        
        #region Skin parts

        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        ///<summary>
        /// Title label
        ///</summary>
        [SkinPart(Required = false)]
        public Label LabelDisplay;

        ///<summary>
        /// Icon display
        ///</summary>
        [SkinPart(Required = false)]
        public Image IconDisplay;

// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnassignedField.Global

        #endregion

        #region Skin states

        public override string GetCurrentSkinState()
        {
            string s;

            if (!Enabled)
                s = "disabled";
            else if (IsDown())
                s = "down";
            else if (_hovered || _mouseCaptured)
                s = "over";
            else
                s = "up";

            if (ToggleMode && Selected)
                s += "AndSelected";

            return s;
        }

        #endregion

        /**
         *  
         *  Remember whether we have fired an event already,
         *  so that we don't fire a second time.
         */
        private bool _downEventFired;

        private bool _keepDown;
        public void KeepDown(bool down, bool fireEvent = true)
        {
            if (_keepDown == down)
                return;

            _keepDown = down;

            if (!fireEvent) // Don't let the ButtonDown event get fired
                _downEventFired = true;

            if (_keepDown)
                InvalidateSkinState();
            else
                InvalidateButtonState();
        }

        private bool IsDown()
        {
            if (!Enabled)
                return false;

            if (_keepDown)
                return true;

            if (_keyboardPressed)
                return true;

            if (_mouseCaptured && (_hovered || _stickyHighlighting))
                return true;
            return false;
        }

        /**
         *  
         *  Storage for the keyboardPressed property 
         */
        private bool _keyboardPressed;
        protected bool KeyboardPressed
        {
            get
            {
                return _keyboardPressed;
            }
            set
            {
                if (value == _keyboardPressed)
                    return;

                _keyboardPressed = value;
                InvalidateButtonState();
            }
        }

        /**
         *  
         *  <code>true</code> when we need to check whether to dispatch
         *  a button down event
         */
        private bool _checkForButtonDownConditions;

        private void InvalidateButtonState()
        {
            //Debug.Log("InvalidateButtonState");
            _checkForButtonDownConditions = true;
            InvalidateProperties();
            InvalidateSkinState();
        }
        
        private bool _stickyHighlighting;
        /**
         *  If <code>false</code>, the button displays its down skin
         *  when the user presses it but changes to its over skin when
         *  the user drags the mouse off of it.
         *  If <code>true</code>, the button displays its down skin
         *  when the user presses it, and continues to display this skin
         *  when the user drags the mouse off of it.
         *
         *  Default: false
         */
        public bool StickyHighlighting
        {
            get { 
                return _stickyHighlighting;
            }
            set
            {
                if (value == _stickyHighlighting)
                    return;

                _stickyHighlighting = value;
                InvalidateButtonState();
            }
        }

        protected override void KeyDownHandler(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent) e;
            if (ke.KeyCode != KeyCode.Space)
                return;
            _keyboardPressed = true;
        }

        protected override void KeyUpHandler(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent) e;
            if (ke.KeyCode != KeyCode.Space)
                return;

            if (Enabled && _keyboardPressed)
            {
                // Mimic mouse click on the button.
                ButtonReleased();
                _keyboardPressed = false;
                DispatchEvent(new MouseEvent(MouseEvent.CLICK));
            }
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == LabelDisplay)
            {
                LabelDisplay.Text = _text;
            }

            else if (instance == IconDisplay)
            {
                IconDisplay.Texture = _icon;
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);
        }

        #region Handlers

        //protected virtual void MouseDownHandler(Event e)
        //{
        //    //SystemEventDispatcher.Instance.AddEventListener(
        //    //    MouseEvent.MOUSE_UP, SystemEventDispatcherMouseUpHandler, EventPhase.Capture | EventPhase.Target);
        //    SystemManager.Instance.MouseUpSignal.Connect(MouseUpSlot, true);
        //        //MouseEvent.MOUSE_UP, SystemEventDispatcherMouseUpHandler, EventPhase.Capture | EventPhase.Target);
        //    SystemEventDispatcher.Instance.AddEventListener(
        //        MouseEvent.MOUSE_LEAVE, SystemEventDispatcherMouseLeaveHandler);

        //    ButtonPressed();
        //}

        //private void SystemEventDispatcherMouseUpHandler(Event e)

        private void MouseUpSlot(params object[] parameters)
        {
            //Debug.Log("MouseUpSlot");

            // If the target is the button, do nothing because the
            // mouseEventHandler will be handle it.
            //if (e.Target == this)
            //    return;
            
            MouseCaptured = false;
        }

        private void SystemEventDispatcherMouseLeaveHandler(Event e)
        {
            Debug.Log("SystemEventDispatcherMouseLeaveHandler");
            ButtonReleased();
        }

        InputEvent _inputEvent;

        private void OnClickOrKeyUp(Event e)
        {
            // return if component not enabled
            if (!Enabled) return;

            // return if this key is filtered by the component
            if (e is KeyboardEvent)
            {
                if (!IsActionKey(((KeyboardEvent)e).KeyCode))
                    return;

                _inputEvent = (KeyboardEvent)e;
                ButtonReleased();
            }
            else
            {
                _inputEvent = (MouseEvent)e;
            }

            if (ToggleMode && ToggleByOwnEvent)
                Selected = !Selected;

            // OLD NOTE: Always dispatching PRESS (we don't check for subscriptions) because some other events (like CHANGE) depend on it

            //ButtonReleased();

            if (HasBubblingEventListener(ButtonEvent.PRESS))
            {
                DispatchEvent(new ButtonEvent(ButtonEvent.PRESS)
                                  {
                                      Bubbles = true, // added 20121130
                                      ButtonId = ButtonId,
                                      ButtonText = Text,
                                      Shift = _inputEvent.Shift,
                                      Control = _inputEvent.Control,
                                      Alt = _inputEvent.Alt,
                                      Selected = Selected
                                  });
            }
        }

        protected virtual void ButtonPressed()
        {
            DispatchEvent(new ButtonEvent(ButtonEvent.BUTTON_DOWN));

            if (_autoRepeat)
            {
                _autoRepeatTimer.Delay = (float) GetStyle("repeatDelay");
                _autoRepeatTimer.AddEventListener(
                    Timer.TICK, AutoRepeatTimerDelayHandler);
                _autoRepeatTimer.Start();
            }
        }

        private void AutoRepeatTimerDelayHandler(Event e)
        {
            if (!Enabled)
                return;

            if (_autoRepeat)
            {
                _autoRepeatTimer.Reset();
                _autoRepeatTimer.Stop();
                _autoRepeatTimer.RemoveEventListener(
                    Timer.TICK, AutoRepeatTimerDelayHandler);
                _autoRepeatTimer.Delay = (float) GetStyle("repeatInterval");
                _autoRepeatTimer.AddEventListener(
                    Timer.TICK, AutoRepeatTimerTimerHandler);
                _autoRepeatTimer.Start();
            }
        }

        private void AutoRepeatTimerTimerHandler(Event e)
        {
            if (!Enabled)
                return;

            DispatchEvent(new ButtonEvent(ButtonEvent.BUTTON_DOWN));
        }

        protected virtual void ButtonReleased()
        {
            DispatchEvent(new ButtonEvent(ButtonEvent.BUTTON_UP));

            if (null != _autoRepeatTimer)
            {
                _autoRepeatTimer.RemoveEventListener(
                    Timer.TICK, AutoRepeatTimerDelayHandler);
                _autoRepeatTimer.RemoveEventListener(
                    Timer.TICK, AutoRepeatTimerTimerHandler);
                _autoRepeatTimer.Reset();
                _autoRepeatTimer.Stop();
            }

            ////SystemEventDispatcher.Instance.RemoveEventListener(
            ////    MouseEvent.MOUSE_UP, SystemEventDispatcherMouseUpHandler, EventPhase.Capture | EventPhase.Target);
            //SystemManager.Instance.MouseUpSignal.Disconnect(MouseUpSlot);

            //SystemEventDispatcher.Instance.RemoveEventListener(
            //    MouseEvent.MOUSE_LEAVE, SystemEventDispatcherMouseLeaveHandler);
        }

        #endregion

        #region INVALIDATION

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_toggleModeChanged)
            {
                _toggleModeChanged = false;
                InvalidateSize();
                InvalidateSkinState();
            }

            if (_checkForButtonDownConditions)
            {
                bool isCurrentlyDown = IsDown();

                // Only if down state has changed, do we need to do something
                if (_downEventFired != isCurrentlyDown)
                {
                    if (isCurrentlyDown)
                        DispatchEvent(new ButtonEvent(ButtonEvent.BUTTON_DOWN));

                    _downEventFired = isCurrentlyDown;
                    CheckAutoRepeatTimerConditions(isCurrentlyDown);
                }
                
                _checkForButtonDownConditions = false;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            RemoveHandlers();

            //RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            RemoveEventListener(MouseEvent.CLICK, OnClickOrKeyUp);
            RemoveEventListener(KeyboardEvent.KEY_UP, OnClickOrKeyUp);

            if (null != _autoRepeatTimer)
            {
                _autoRepeatTimer.RemoveEventListener(
                    Timer.TICK, AutoRepeatTimerDelayHandler);
                _autoRepeatTimer.RemoveEventListener(
                    Timer.TICK, AutoRepeatTimerTimerHandler);
            }

            //SystemEventDispatcher.Instance.RemoveEventListener(
            //    MouseEvent.MOUSE_UP, SystemEventDispatcherMouseUpHandler, EventPhase.Capture | EventPhase.Target);
            SystemManager.Instance.MouseUpSignal.Disconnect(MouseUpSlot);

            SystemEventDispatcher.Instance.RemoveEventListener(
                MouseEvent.MOUSE_LEAVE, SystemEventDispatcherMouseLeaveHandler);

            if (AutoDisposeUnmanagedResources)
                Object.DestroyImmediate(_icon, true);
        }

        #endregion

        #region AutoRepeat

        private Timer _autoRepeatTimer;

        private bool _autoRepeat;
        public bool AutoRepeat
        {
            get { 
                return _autoRepeat;
            }
            set
            {
                if (value == _autoRepeat)
                    return;
                
                _autoRepeat = value;
                //_autoRepeatTimer = value ? new Timer(1) : null;
                CheckAutoRepeatTimerConditions(IsDown());
            }
        }

        private void CheckAutoRepeatTimerConditions(bool buttonDown)
        {
            bool needsTimer = AutoRepeat && buttonDown;
            bool hasTimer = _autoRepeatTimer != null;
            
            if (needsTimer == hasTimer)
                return;

            if (needsTimer)
                StartTimer();
            else
                StopTimer();
        }

        /**
         *  
         */
        private void StartTimer()
        {
            _autoRepeatTimer = new Timer((float)GetStyle("repeatDelay"));
            _autoRepeatTimer.AddEventListener(Timer.TICK, AutoRepeatTimerDelayHandler);
            _autoRepeatTimer.Start();
        }

        /**
         *  
         */
        private void StopTimer()
        {
            _autoRepeatTimer.Stop();
            _autoRepeatTimer = null;
        }

        #endregion
    }
}