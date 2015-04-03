using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Managers;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event = eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Components
{
    [Style(Name = "disabledOverlayStyle", Type = typeof(GUIStyle), ProxyType = typeof(DisabledOverlayStyle))] //ProxyType = typeof(ComponentStyleProxy), ProxyMemberName = "DisabledOverlayStyle")]

    public abstract class InteractiveComponent : InvalidationManagerClient, IFocusComponent, IFocusManagerClient
    {
#if DEBUG 
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode = false;
        // ReSharper restore UnassignedField.Global
#endif

        #region Static

// ReSharper disable InconsistentNaming
        public static string FOCUS_OUT_UID = "_________eDriven GUI Copyright 2012 by Danko Kozar";
// ReSharper restore InconsistentNaming

        #endregion
        
        #region Properties

        //private bool _idChanged;
        //private string _hotControlId;
        /// <summary>
        /// Component ID used by developer for locating the component via ComponentManager<br/>
        /// Must be unique in the application
        ///</summary>
        ///<exception cref="ComponentException"></exception>
        public override string Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                //if (null != base.Id)
                //    throw new ComponentException(string.Format(ComponentException.CannotChangeId, base.Id, value));

                base.Id = value;
                //_idChanged = true;
                //InvalidateProperties();
                RegisterId();
            }
        }

        /// <summary>
        /// Finds a child component by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>Walks up the hierarchy -> Recursive!!!</remarks>
        public InteractiveComponent GetChildComponent(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("id not defined");

            return FindRecursive(Children, id);
        }

        private static InteractiveComponent FindRecursive(IEnumerable<DisplayListMember> children, string id)
        {
            InteractiveComponent result = null;

            foreach (var child in children)
            {
                //Debug.Log("child.Id: " + child.Id);
                var ic = child as InteractiveComponent;
                if (null == ic)
                    continue;

                if (child.Id == id)
                {
                    return ic;
                }

                result = FindRecursive(ic.Children, id);
                if (null != result)
                    break;
            }
            return result;
        }

        //private bool _uidChanged;
        private string _uid;
        /// <summary>
        /// Component ID used internally by the application for locating the component via ComponentManager<br/>
        /// Must be unique in the application
        ///</summary>
        ///<exception cref="ComponentException"></exception>
        public string Uid
        {
            get
            {
                return _uid;
            }
            set
            {
                if (null != _uid)
                    throw new ComponentException(string.Format(ComponentException.CannotChangeUid, _uid, value));

                _uid = value;
                RegisterComponent();
            }
        }

        /// <summary>
        /// Is component created
        ///</summary>
        public virtual bool Created { get; protected set; }

        ///// <summary>
        ///// Does this component get registered by the component manager?
        ///// Set this to true for components which have tooltips
        ///// btw setting the tooltip itself sets this to true
        ///// </summary>
        ////protected bool RegisterComponent { get; set;}

        public override string Tooltip
        {
            get
            {
                return base.Tooltip;
            }
            set
            {
                if (value == base.Tooltip)
                    return;
                
                base.Tooltip = value;
                RegisterComponent();
            }
        }

        //private GUIStyle _disabledOverlay;

        // TODO: make the enabled invalidation to actually change the value of the variable _disabledOverlay
        // and not to be checked by the render routine!
        private bool _enabledChanged;
        private bool _enabled = true;
        /// <summary>
        /// The component enabled/disabled state
        /// </summary>
        public virtual bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled) // register Enabled CHANGE only
                {
                    _enabled = value;
                    _enabledChanged = true;
                    InvalidateProperties();
                }
            }
        }

        //private bool _processClicksChanged;
        private bool _processClicks = false; // changed on 5.1.2012.

        //private bool _processMouseOversChanged;
        //private bool _processMouseOvers = true;
        ///// <summary>
        ///// Is mouse position processed with rollover & rollout
        ///// Use wisely - performance costing
        ///// </summary>
        //public virtual bool ProcessMouseOvers
        //{
        //    get
        //    {
        //        return _processMouseOvers;
        //    }
        //    set
        //    {
        //        if (value != _processMouseOvers)
        //        {
        //            _processMouseOvers = value;
        //            _processMouseOversChanged = true;
        //            InvalidateProperties();
        //        }
        //    }
        //}

        //private bool _mouseEnabledChanged;
        private bool _mouseEnabled = true;
        /// <summary>
        /// Is mouse position processed when moved
        /// Use wisely - performance costing
        /// </summary>
        public bool MouseEnabled
        {
            get
            {
                return _mouseEnabled;
            }
            set
            {
                if (value != _mouseEnabled)
                {
                    _mouseEnabled = value;
                    //_mouseEnabledChanged = true;
                    //InvalidateProperties();
                }
            }
        }

        ///<summary>
        /// Whether the component can receive focus when clicked on
        ///</summary>
// ReSharper disable UnusedMember.Global
        public bool MouseFocusEnabled { get; set; }
// ReSharper restore UnusedMember.Global

        private bool _tabFocusEnabled;
        ///<summary>
        /// Whether the component can receive focus via the TAB key
        ///</summary>
        public bool TabFocusEnabled
        {
            get { 
                return _tabFocusEnabled;
            }
            set
            {
                if (value == _tabFocusEnabled)
                    return;

                _tabFocusEnabled = value;
                DispatchEvent(new Event("tabFocusEnabledChange"));
            }
        }

        ///<summary>
        ///</summary>
        public int TabIndex { get; set; }

        //private bool _processMouseWheelChanged;
        //private bool _processMouseWheel = true;
        ///// <summary>
        ///// Is mouse position processed when moved
        ///// Use wisely - performance costing
        ///// </summary>
        //public virtual bool ProcessMouseWheel
        //{
        //    get
        //    {
        //        return _processMouseWheel;
        //    }
        //    set
        //    {
        //        if (value != _processMouseWheel)
        //        {
        //            _processMouseWheel = value;
        //            _processMouseWheelChanged = true;
        //            InvalidateProperties();
        //        }
        //    }
        //}

        private bool _processKeysChanged;
        private bool _processKeys = true;
        /// <summary>
        /// Processes key up/downs
        /// Use wisely - performance costing
        /// </summary>
        public virtual bool ProcessKeys
        {
            get
            {
                return _processKeys;
            }
            set
            {
                if (value != _processKeys)
                {
                    _processKeys = value;
                    _processKeysChanged = true;
                    InvalidateProperties();
                }
            }
        }

        /// <summary>
        /// Does this suppress event propagation to background (other stages or 3D)
        /// </summary>
        public virtual bool SupressEventPropagation {get; set;}

        /// <summary>
        /// Does a component highlight when focused
        ///</summary>
        public virtual bool HighlightOnFocus {get; set;}

        private bool _focusEnabledChanged;
        private bool _focusEnabled; // false by default
        /// <summary>
        /// Is the component is set to focus via FocusManager
        /// Handled by Component in click processing
        /// if ProcessClicks is true
        /// </summary>
        public virtual bool FocusEnabled
        {
            get
            {
                return _focusEnabled;
            }
            set
            {
                if (value != _focusEnabled)
                {
                    _focusEnabled = value;
                    _focusEnabledChanged = true;
                    // important - component which has to be focused should be registered with component manager
                    if (_focusEnabled)
                        RegisterComponent();
                }
            }
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;

                if (HasExplicitFocus && !value)
                    FocusManager.Instance.Blur(this);
            }
        }

        /// <summary>
        /// The collection of KeyCode which should fire the KeyDown
        /// If the collection is empty, no filter is applied
        /// </summary>
        public List<KeyCode> KeyDownFilter = new List<KeyCode>();

        /// <summary>
        /// The collection of KeyCode which should fire the KeyUp
        /// If the collection is empty, no filter is applied
        /// </summary>
        public List<KeyCode> KeyUpFilter = new List<KeyCode>();

        /// <summary>
        /// Check if supplied key event passes key down filter
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool PassesKeyDownFilter(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent)e;
            return (KeyDownFilter.Count == 0 || KeyDownFilter.Contains(ke.KeyCode));
        }

        /// <summary>
        /// Check if supplied key event passes key up filter
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool PassesKeyUpFilter(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent)e;
            return (KeyUpFilter.Count == 0 ||  KeyUpFilter.Contains(ke.KeyCode));
        }

        /// <summary>
        /// The collection of KeyCode which should fire action events 
        /// (like PRESS event of button when in focus)
        /// </summary>
        public List<KeyCode> ActionKeys = new List<KeyCode>();

        /// <summary>
        /// Check if supplied key is registered as action key
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public bool IsActionKey(KeyCode keyCode)
        {
            return (ActionKeys.Count == 0 || ActionKeys.Contains(keyCode));
        }

        #region Event Handlers

        /// <summary>
        /// The event that fires when the key is down, if processKeys is enabled
        ///</summary>
        private MulticastDelegate _keyDown;
        [Event(Name = KeyboardEvent.KEY_DOWN, Type = typeof(KeyboardEvent), Bubbles = true)]
        public MulticastDelegate KeyDown
        {
            get
            {
                if (null == _keyDown)
                    _keyDown = new MulticastDelegate(this, KeyboardEvent.KEY_DOWN);
                return _keyDown;
            }
            set
            {
                _keyDown = value;
            }
        }

        /// <summary>
        /// The event that fires when the key is up, if processKeys is enabled
        ///</summary>
        private MulticastDelegate _keyUp;
        [Event(Name = KeyboardEvent.KEY_UP, Type = typeof(KeyboardEvent), Bubbles = true)]
        public MulticastDelegate KeyUp
        {
            get
            {
                if (null == _keyUp)
                    _keyUp = new MulticastDelegate(this, KeyboardEvent.KEY_UP);
                return _keyUp;
            }
            set
            {
                _keyUp = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives focus
        ///</summary>
        private MulticastDelegate _focusIn;
        [Event(Name = GuiEvent.FOCUS_IN, Type = typeof(GuiEvent), Bubbles = true)]
        public MulticastDelegate FocusIn
        {
            get
            {
                if (null == _focusIn)
                    _focusIn = new MulticastDelegate(this, GuiEvent.FOCUS_IN);
                return _focusIn;
            }
            set
            {
                _focusIn = value;
            }
        }

        /// <summary>
        /// The event that fires when the component looses focus
        ///</summary>
        private MulticastDelegate _focusOut;
        [Event(Name = GuiEvent.FOCUS_OUT, Type = typeof(GuiEvent), Bubbles = true)]
        public MulticastDelegate FocusOut
        {
            get
            {
                if (null == _focusOut)
                    _focusOut = new MulticastDelegate(this, GuiEvent.FOCUS_OUT);
                return _focusOut;
            }
            set
            {
                _focusOut = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse down
        ///</summary>
        private MulticastDelegate _mouseDown;
        [Event(Name = MouseEvent.MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseDown
        {
            get
            {
                if (null == _mouseDown)
                    _mouseDown = new MulticastDelegate(this, MouseEvent.MOUSE_DOWN);
                return _mouseDown;
            }
            set
            {
                _mouseDown = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse up
        ///</summary>
        private MulticastDelegate _mouseUp;
        [Event(Name = MouseEvent.MOUSE_UP, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseUp
        {
            get
            {
                if (null == _mouseUp)
                    _mouseUp = new MulticastDelegate(this, MouseEvent.MOUSE_UP);
                return _mouseUp;
            }
            set
            {
                _mouseUp = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse click
        ///</summary>
        private MulticastDelegate _click;
        [Event(Name = MouseEvent.CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate Click
        {
            get
            {
                if (null == _click)
                    _click = new MulticastDelegate(this, MouseEvent.CLICK);
                return _click;
            }
            set
            {
                _click = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives double click
        ///</summary>
        private MulticastDelegate _doubleClick;
        [Event(Name = MouseEvent.DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate DoubleClick
        {
            get
            {
                if (null == _doubleClick)
                    _doubleClick = new MulticastDelegate(this, MouseEvent.DOUBLE_CLICK);
                return _doubleClick;
            }
            set
            {
                _doubleClick = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives right mouse down
        ///</summary>
        private MulticastDelegate _rightMouseDown;
        [Event(Name = MouseEvent.RIGHT_MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RightMouseDown
        {
            get
            {
                if (null == _rightMouseDown)
                    _rightMouseDown = new MulticastDelegate(this, MouseEvent.RIGHT_MOUSE_DOWN);
                return _rightMouseDown;
            }
            set
            {
                _rightMouseDown = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives right mouse up
        ///</summary>
        private MulticastDelegate _rightMouseUp;
        [Event(Name = MouseEvent.RIGHT_MOUSE_UP, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RightMouseUp
        {
            get
            {
                if (null == _rightMouseUp)
                    _rightMouseUp = new MulticastDelegate(this, MouseEvent.RIGHT_MOUSE_UP);
                return _rightMouseUp;
            }
            set
            {
                _rightMouseUp = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives right click
        ///</summary>
        private MulticastDelegate _rightClick;
        [Event(Name = MouseEvent.RIGHT_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RightClick
        {
            get
            {
                if (null == _rightClick)
                    _rightClick = new MulticastDelegate(this, MouseEvent.RIGHT_CLICK);
                return _rightClick;
            }
            set
            {
                _rightClick = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives middle mouse click
        ///</summary>
        private MulticastDelegate _rightDoubleClick;
        [Event(Name = MouseEvent.RIGHT_DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RightDoubleClick
        {
            get
            {
                if (null == _rightDoubleClick)
                    _rightDoubleClick = new MulticastDelegate(this, MouseEvent.RIGHT_DOUBLE_CLICK);
                return _rightDoubleClick;
            }
            set
            {
                _rightDoubleClick = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives middle mouse down
        ///</summary>
        private MulticastDelegate _middleMouseDown;
        [Event(Name = MouseEvent.MIDDLE_MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MiddleMouseDown
        {
            get
            {
                if (null == _middleMouseDown)
                    _middleMouseDown = new MulticastDelegate(this, MouseEvent.MIDDLE_MOUSE_DOWN);
                return _middleMouseDown;
            }
            set
            {
                _middleMouseDown = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives middle mouse up
        ///</summary>
        private MulticastDelegate _middleMouseUp;
        [Event(Name = MouseEvent.MIDDLE_MOUSE_UP, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MiddleMouseUp
        {
            get
            {
                if (null == _middleMouseUp)
                    _middleMouseUp = new MulticastDelegate(this, MouseEvent.MIDDLE_MOUSE_UP);
                return _middleMouseUp;
            }
            set
            {
                _middleMouseUp = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives middle mouse click
        ///</summary>
        private MulticastDelegate _middleClick;
        [Event(Name = MouseEvent.MIDDLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MiddleClick
        {
            get
            {
                if (null == _middleClick)
                    _middleClick = new MulticastDelegate(this, MouseEvent.MIDDLE_CLICK);
                return _middleClick;
            }
            set
            {
                _middleClick = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives middle mouse click
        ///</summary>
        private MulticastDelegate _middleDoubleClick;
        [Event(Name = MouseEvent.MIDDLE_DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MiddleDoubleClick
        {
            get
            {
                if (null == _middleDoubleClick)
                    _middleDoubleClick = new MulticastDelegate(this, MouseEvent.MIDDLE_DOUBLE_CLICK);
                return _middleDoubleClick;
            }
            set
            {
                _middleDoubleClick = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse over
        ///</summary>
        private MulticastDelegate _mouseOver;
        [Event(Name = MouseEvent.MOUSE_OVER, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseOver
        {
            get
            {
                if (null == _mouseOver)
                    _mouseOver = new MulticastDelegate(this, MouseEvent.MOUSE_OVER);
                return _mouseOver;
            }
            set
            {
                _mouseOver = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse out
        ///</summary>
        private MulticastDelegate _mouseOut;
        [Event(Name = MouseEvent.MOUSE_OUT, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseOut
        {
            get
            {
                if (null == _mouseOut)
                    _mouseOut = new MulticastDelegate(this, MouseEvent.MOUSE_OUT);
                return _mouseOut;
            }
            set
            {
                _mouseOut = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse over
        ///</summary>
        private MulticastDelegate _rollOver;
        [Event(Name = MouseEvent.ROLL_OVER, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RollOver
        {
            get
            {
                if (null == _rollOver)
                    _rollOver = new MulticastDelegate(this, MouseEvent.ROLL_OVER);
                return _rollOver;
            }
            set
            {
                _rollOver = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse out
        ///</summary>
        private MulticastDelegate _rollOut;
        [Event(Name = MouseEvent.ROLL_OUT, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate RollOut
        {
            get
            {
                if (null == _rollOut)
                    _rollOut = new MulticastDelegate(this, MouseEvent.ROLL_OUT);
                return _rollOut;
            }
            set
            {
                _rollOut = value;
            }
        }

        /// <summary>
        /// The event that fires when the component receives mouse move
        /// Note: This works in editor only!
        ///</summary>
        private MulticastDelegate _mouseMove;
        [Event(Name = MouseEvent.MOUSE_MOVE, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseMove
        {
            get
            {
                if (null == _mouseMove)
                    _mouseMove = new MulticastDelegate(this, MouseEvent.MOUSE_MOVE);
                return _mouseMove;
            }
            set
            {
                _mouseMove = value;
            }
        }
        
        /// <summary>
        /// The event that fires when the component receives mouse wheel
        ///</summary>
        private MulticastDelegate _mouseWheel;
        [Event(Name = MouseEvent.MOUSE_WHEEL, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseWheel
        {
            get
            {
                if (null == _mouseWheel)
                    _mouseWheel = new MulticastDelegate(this, MouseEvent.MOUSE_WHEEL);
                return _mouseWheel;
            }
            set
            {
                _mouseWheel = value;
            }
        }

        /// <summary>
        /// The event that fires when the component is popup and user mouse-clicked outside the popup
        /// NOTE: Works with popups only
        ///</summary>
        private MulticastDelegate _mouseDownOutside;
        [Event(Name = MouseEvent.MOUSE_DOWN_OUTSIDE, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseDownOutside
        {
            get
            {
                if (null == _mouseDownOutside)
                    _mouseDownOutside = new MulticastDelegate(this, MouseEvent.MOUSE_DOWN_OUTSIDE);
                return _mouseDownOutside;
            }
            set
            {
                _mouseDownOutside = value;
            }
        }

        /// <summary>
        /// The event that fires when the component is popup and user mouse-wheeled outside the popup
        /// NOTE: Works with popups only
        ///</summary>
        private MulticastDelegate _mouseWheelOutside;
        [Event(Name = MouseEvent.MOUSE_WHEEL_OUTSIDE, Type = typeof(MouseEvent), Bubbles = true)]
        public MulticastDelegate MouseWheelOutside
        {
            get
            {
                if (null == _mouseWheelOutside)
                    _mouseWheelOutside = new MulticastDelegate(this, MouseEvent.MOUSE_WHEEL_OUTSIDE);
                return _mouseWheelOutside;
            }
            set
            {
                _mouseWheelOutside = value;
            }
        }

        #endregion

        #endregion

        #region Members

        //private bool _rolledOver = false;

        #endregion

        #region Constructor

        protected InteractiveComponent()
        {
            //SetStyle("disabledOverlay", DisabledOverlayStyle.Instance);

            // invalidation flags
            //_processClicksChanged = true;
            //_processMouseOversChanged = true;
            //_mouseEnabledChanged = true;
            _processKeysChanged = true;
            _focusEnabledChanged = true;
            //_processMouseWheelChanged = true;
        }

        #endregion

        #region Styles

        ///<summary>
        /// Should this component dispatch style changes events (optimization)
        ///</summary>
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global
        public bool DispatchStyleChanges;
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global

        /// <summary>
        /// This method is run each time that any style has been changed
        /// </summary>
        /// <param name="styleProp">Style property</param>
        /// <param name="s">Style value</param>
        //public override void StyleChanged(string styleProp, object s /*, object style*/)
        //{
        //    StyleProtoChain.StyleChanged(this, styleProp);

        //    if (DispatchStyleChanges)
        //    {
        //        if (null != styleProp && (styleProp != "styleName"))
        //        {
        //            if (HasEventListener(styleProp + "Changed"))
        //                DispatchEvent(new Event(styleProp + "Changed"));
        //        }
        //        else
        //        {
        //            if (HasEventListener("allStylesChanged"))
        //                DispatchEvent(new Event("allStylesChanged"));
        //        }
        //    }

        //    switch (styleProp)
        //    {
        //        case "disabledOverlayStyle":
        //            _disabledOverlay = (GUIStyle)s;
        //            break;
        //    }
        //}

        #endregion

        public int HotControlId { get; internal set; }

        protected override void PreRender()
        {
            base.PreRender();

            /**
             * Naming the control (for focusing purposes, etc,)
             * Note: the post-render focusing is implemented in PostRender method
             * */
            if (this == FocusManager.Instance.FocusedComponent && null != Uid) {
                //Debug.Log(string.Format("GUI.SetNextControlName({0})", Uid));
                GUI.SetNextControlName(Uid);
            }
        }

        //protected override void Render()
        //{
        //    base.Render();

        //    HandleFocusIn();
        //}

        //protected void HandleFocusIn()
        //{
        //    //if (_focusInRequested)
        //    //{
        //    //    //Defer(delegate(object[] args)
        //    //    //{
        //    //        Debug.Log("_focusInRequested: " + this);
        //    //        //Debug.Log("    HotControlId: " + HotControlId);

        //    //        /**
        //    //         * Note: Although we have set the hot control ID on mouse down (which was needed to immediatelly have text selection)
        //    //         * we have to do it here too, because of tab focus
        //    //         * */
        //    //        if (0 != HotControlId) // && GUIUtility.keyboardControl != HotControlId)
        //    //        {
        //    //            //Debug.Log("* " + GUIUtility.keyboardControl);

        //    //            if (this is TextField)
        //    //            {
        //    //                //GUIUtility.keyboardControl = HotControlId;
        //    //                //Debug.Log("GUIUtility.keyboardControl: " + GUIUtility.keyboardControl);
        //    //                GUI.FocusControl(Uid);
        //    //            }
        //    //            else
        //    //            {
        //    //                // for all other controls
        //    //                //GUIUtility.keyboardControl = 0;
        //    //                //if (FocusManager.AutoCorrectUnityFocus)
        //    //                //    TextFieldFocusHelper.AutoCorrectUnityFocus();
        //    //            }
        //    //        }
        //    //    //}, 2);
        //    //}
        //}

        //protected override void OverlayRender()
        //{
        //    base.OverlayRender();

        //    if (UnityEngine.Event.current.type == EventType.Repaint)
        //    {
        //        /**
        //         * 1) draw overlay for if in disabled state
        //         * */
        //        if (!Enabled){
                
        //            if (null != _disabledOverlay)
        //            {
        //                _disabledOverlay.Draw(RenderingRect,
        //                       this == MouseEventDispatcher.MouseTarget,
        //                       this == MouseEventDispatcher.MouseDownComponent,
        //                       false,
        //                       this == FocusManager.Instance.FocusedComponent); // 20130704
        //                       //true);
        //            }
        //        }
        //    }
        //}

        private GUIStyle _focusIndicatorStyle;

        protected virtual bool IsOurFocus(DisplayObject target)
        {
            return target == this;
        }

        protected override void FocusRender()
        {
            base.FocusRender();

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                /**
                 * 2) draw focus indicator if in focus
                 * */
                //if (IsInFocus && HighlightOnFocus)
                if (IsOurFocus(FocusManager.Instance.FocusedComponent) && HighlightOnFocus)
                {
                    //Debug.Log("IsOurFocus: " + this);
                    //FocusIndicatorStyle.Instance.Draw(RenderingRect,
                    //       this == MouseEventDispatcher.MouseTarget,
                    //       this == MouseEventDispatcher.MouseDownComponent,
                    //       false,
                    //       true);

                    if (null == _focusIndicatorStyle)
                    {
                        _focusIndicatorStyle = FocusIndicatorStyle.Instance;// FocusIndicatorStyleProxy.Instance.Default.FocusIndicatorStyle;
                    }

                    _focusIndicatorStyle.Draw(RenderingRect,
                           this == MouseEventDispatcher.MouseTarget,
                           this == MouseEventDispatcher.MouseDownComponent,
                           false,
                           this == FocusManager.Instance.FocusedComponent); // 20130704 // true);
                }
            }

            if (_focusOutRequested)
            {
                _focusOutRequested = false;

                // NOTE: FOCUS_OUT on some component doesn't have to fire BEFORE FOCUS_IN on another component
                // in this implementation
                // that is because those events happen in rendering cycle, thus being dependant on display list location
                DispatchEvent(new GuiEvent(GuiEvent.FOCUS_OUT)); // bubbling event! (for dialogs etc)
            }

            if (_focusInRequested)
            {
                _focusInRequested = false;
                DispatchEvent(new GuiEvent(GuiEvent.FOCUS_IN)); // bubbling event! (for dialogs etc)
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_focusEnabledChanged)
            {
                _focusEnabledChanged = false;
                // check if _focusEnabled property changed while this component is in focus
                // if so - blur it
                if (!_focusEnabled && FocusManager.Instance.HasFocusedComponent(this))
                    FocusManager.Instance.Blur(this);
            }

            if (_processKeysChanged || _isInFocusChanged) // TODO: remove this inline handlers stuf, let the MouseEventDispatcher call it directly and dispatch event from here
            {
                //Debug.Log("_processKeysChanged || _isInFocusChanged: " + this);
                _processKeysChanged = false;
                _isInFocusChanged = false;
                if (_processKeys && _isInFocus)
                {
                    // subscribe to SystemManager
                    //SystemManager.Instance.KeyDown += new EventHandler(OnKeyDown);
                    //SystemManager.Instance.KeyUp += new EventHandler(OnKeyUp);

                    //KeyDown += KeyDownHandler;
                    //KeyUp += KeyUpHandler;

                    /**
                     * NOTE: This handlers fire automatically ONLY when component is in focus
                     * In any other case it does not!
                     * For example, this works for datagrid, list, combobox, because they dont have any focusable children
                     * and they are constantly in focus
                     * This doesn't fire for components alowing children to have focus, for instance a NumericStepper alows its textbox to be in focus, when directly clicked
                     * */
                    AddEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler);
                    AddEventListener(KeyboardEvent.KEY_UP, KeyUpHandler);
                }
                else
                {
                    // unsubscribe from SystemManager
                    //SystemManager.Instance.KeyDown -= new EventHandler(OnKeyDown);
                    //SystemManager.Instance.KeyUp -= new EventHandler(OnKeyUp);
                    
                    //KeyDown -= KeyDownHandler;
                    //KeyUp -= KeyUpHandler;
                    RemoveEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler);
                    RemoveEventListener(KeyboardEvent.KEY_UP, KeyUpHandler);
                }
            }

            if (_enabledChanged)
            {
                _enabledChanged = false;
            }
        }

        #region INTERNAL EVENT HANDLERS

//        protected virtual void MouseDownHandler(Event e)
//        {
//#if DEBUG
//            if (DebugMode)
//            {
//                Debug.Log(string.Format("InteractiveComponent: MouseDownHandler [CurrentTarget:{0}, Target: {1}]", e.CurrentTarget, e.Target));
//            }
//#endif
//        }

//        /// <summary>
//        /// Focus manager fires after the mouse-up, not click!
//        /// NOTE: This is a Windows-like behaviour
//        /// </summary>
//        /// <param name="e"></param>
//        protected virtual void MouseUpHandler(Event e)
//        {
//#if DEBUG
//            if (DebugMode)
//            {
//                Debug.Log(string.Format("InteractiveComponent: MouseUpHandler [CurrentTarget:{0}, Target: {1}]", e.CurrentTarget, e.Target));    
//            }
//#endif
            
//            if (this == e.Target)
//            {
//                InteractiveComponent ic = FocusManager.FindFocusableAncestor(this);
//                if (null != ic){
//#if DEBUG
//                    if (DebugMode)
//                    {
//                        Debug.Log(string.Format("InteractiveComponent: Setting focus on [{0}]", ic));
//                    }
//#endif
//                    ic.SetFocus();
//                }
//            }
//        }

        private bool _isInFocusChanged;
        private bool _isInFocus;
        protected bool IsInFocus
        {
            get { return _isInFocus; }
            set
            {
                if (value == _isInFocus)
                    return;

                _isInFocus = value;
                _isInFocusChanged = true;
                InvalidateProperties();
            }
        }

        #region IFocusManagerClient

        private bool _focusInRequested; // (on the next OnGUI call)
        /// <summary>
        /// Executes when the component focus in is requested
        /// </summary>
        public virtual void FocusInHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("FocusIn [{0}]", this));
            }
#endif
            //Debug.Log(string.Format("FocusIn [{0}]", this));
            IsInFocus = true;
            _focusInRequested = true;
            //Debug.Log(string.Format("FocusManager.Instance.FocusedComponent [{0}]", FocusManager.Instance.FocusedComponent));
        }

        private bool _focusOutRequested; // (on the next OnGUI call)
        /// <summary>
        /// Executes when the component focus out is requested
        /// </summary>
        public virtual void FocusOutHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("FocusOut [{0}]", this));
            }
#endif
            //Debug.Log(string.Format("FocusOut [{0}]", this));
            IsInFocus = false;

            _focusOutRequested = true;
        }

        #endregion

        /// <summary>
        /// KeyDown handler
        /// NOTE: We do the filtering here (in the local handler), 
        /// but dispatching happens always (when component in focus) - no metter of filtering
        /// This is needed for event bubbling (because perhaps parent component wants to react on key that is filtered by this component)
        /// </summary>
        /// <param name="e">An event</param>
        protected virtual void KeyDownHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("InteractiveComponent:OnKeyDown: " + this);
#endif
        }

        /// <summary>
        /// KeyUp handler
        /// NOTE: We do the filtering here (in the local handler), 
        /// but dispatching happens always (when component in focus) - no metter of filtering
        /// This is needed for event bubbling (because perhaps parent component wants to react on key that is filtered by this component)
        /// </summary>
        /// <param name="e">An event</param>
        protected virtual void KeyUpHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("InteractiveComponent:KeyUp " + this);
#endif
        }

        #endregion

        protected override void InitializeContent()
        {
            base.InitializeContent();

            // Tooltip

            // Not an error!
            // We use Ids as tooltips, because we register the component with the compoenent manager 
            // and get tooltips for roll-overed components by ID-s 
            //if (!string.IsNullOrEmpty(Tooltip) && null != Id)
            //    Content.tooltip = Id;

            if (null != Uid)
                Content.tooltip = Uid;
        }

        public void RegisterComponent()
        {
            //_uidChanged = true;
            //InvalidateProperties();

            if (!ComponentManager.Instance.IsRegisteredInternal(this))
                ComponentManager.Instance.RegisterInternal(ref _uid, this);
        }

        private void RegisterId()
        {
            //if (!ComponentManager.Instance.IsRegistered(this))
                ComponentManager.Instance.Register(/*ref*/ Id, this);
        }

        #region Implementation of IFocusComponent

        private bool _hasFocusableChildren;
        ///<summary>
        ///</summary>
        public virtual bool HasFocusableChildren
        {
            get { return _hasFocusableChildren; }
            set
            {
                if (value == _hasFocusableChildren)
                    return;

                _hasFocusableChildren = value;
                DispatchEvent(new Event("hasFocusableChildrenChange"));
            }
        }

        /// <summary>
        /// Sets component to focus
        /// </summary>
        public virtual void SetFocus()
        {
            // the call of the internal function of focus manager
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("InteractiveComponent: SetFocus [{0}]", this));
            }
#endif
            /**
             * Check if the component is created
             * (if it's hierarchy is set etc.)
             * This should't necessary be true for some components (popups etc.)
             * If not completely created yet, wait for a creation complete event
             * */
            //if (Initialized) {
                FocusManager.Instance.TabbedToFocus = true;
                //Debug.Log("Setting focus: " + this);
                FocusManager.Instance.SetFocus(this);
            /*}
            else
            {
                AddEventListener(FrameworkEvent.CREATION_COMPLETE, SetFocusDelayed);
            }*/
        }

        ///<summary>
        ///</summary>
        ///<param name="isFocused"></param>
        public virtual void DrawFocus(bool isFocused)
        {
            
        }

        /*private void SetFocusDelayed(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("InteractiveComponent: SetFocusDelayed [{0}]", this));
            }
#endif

            RemoveEventListener(FrameworkEvent.CREATION_COMPLETE, SetFocusDelayed);
            FocusManager.Instance.TabbedToFocus = true;
            FocusManager.Instance.SetFocus(this);
        }*/

        /// <summary>
        /// Internal method which is being called from FocusManager
        /// used for dispatching event after the focus has been received
        /// </summary>
//        internal void ExecuteFocus()
//        {
//#if DEBUG
//            if (DebugMode)
//            {
//                Debug.Log(string.Format("InteractiveComponent: ExecuteFocus [{0}]", this));
//            }
//#endif

//            // dispatch event which indicates to the outside world that focus is received
//            DispatchEvent(new GuiEvent(GuiEvent.FOCUS_IN)); // bubbling event! (for dialogs etc)
//      }

        /// <summary>
        /// Does a component have focus (including its children)
        /// </summary>
        public virtual bool HasFocus
        {
            get
            {
                return HasExplicitFocus;
            }
        }

        /// <summary>
        /// Does a component have an explicit focus
        /// </summary>
        public bool HasExplicitFocus
        {
            get
            {
                InteractiveComponent fc = FocusManager.Instance.FocusedComponent;
                if (null != fc)
                    return this == fc;

                return false;
            }
        }

        public bool IsFocusable
        {
            get
            {
                return IsFocusableRecursive(this);
            }
        }

        ///<summary>
        /// Focus routing signature
        ///</summary>
        public delegate void RocusRoutingSignature();

        ///<summary>
        /// Focus routing
        ///</summary>
        public RocusRoutingSignature FocusRouting;

        /// <summary>
        /// NOTE: Recursive!!!
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private static bool IsFocusableRecursive(InteractiveComponent child)
        {
            if (child.FocusEnabled)
                return true;

            foreach (Component component in child.Children)
            {
                if (component.FocusEnabled)
                    return true;

                return IsFocusableRecursive(component);
            }

            //if (child is Group)
            //{
            //    Container container = (Container)child;
            //    foreach (Component component in container.Children)
            //    {
            //        if (component.FocusEnabled)
            //            return true;

            //        return IsFocusableRecursive(component);
            //    }
            //}

            return false;
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            RemoveEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler);
            RemoveEventListener(KeyboardEvent.KEY_UP, KeyUpHandler);
        }
    }
}