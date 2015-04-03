using eDriven.Animation;
using eDriven.Core.Events;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;
using EventHandler=eDriven.Core.Events.EventHandler;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Base component class
    /// </summary>
    
    #region Event metadata

    // framework events
    [Event(Name = FrameworkEvent.PREINITIALIZE, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.INITIALIZE, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.CREATION_COMPLETE, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.SHOWING, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.HIDING, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.SHOW, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.HIDE, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.ADDING, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.ADD, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.REMOVE, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.X_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.Y_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.HEIGHT_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_HEIGHT_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.MIN_WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.MIN_HEIGHT_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_MIN_WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_MIN_HEIGHT_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.MAX_WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.MAX_HEIGHT_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_MAX_WIDTH_CHANGED, Type = typeof(Event))]
    [Event(Name = FrameworkEvent.EXPLICIT_MAX_HEIGHT_CHANGED, Type = typeof(Event))]

    // mouse events
    [Event(Name = MouseEvent.MOUSE_MOVE, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_OVER, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_OUT, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_UP, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MIDDLE_MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MIDDLE_MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MIDDLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MIDDLE_DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.RIGHT_MOUSE_DOWN, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.RIGHT_MOUSE_UP, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.RIGHT_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.RIGHT_DOUBLE_CLICK, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_DOWN_OUTSIDE, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_DRAG, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_LEAVE, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_WHEEL, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.MOUSE_WHEEL_OUTSIDE, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.ROLL_OVER, Type = typeof(MouseEvent), Bubbles = true)]
    [Event(Name = MouseEvent.ROLL_OUT, Type = typeof(MouseEvent), Bubbles = true)]
    
    #endregion

    #region Style metadata

    [Styleable(Reader = typeof(eDrivenGuiComponentReader), Mode = StyleableProxyMode.Reflection/*, ReadIdMethod = "ReadId", ReadClassnameMethod = "ReadClassname"*/)]

    // constrains
    [Style(Name = "left", Type = typeof(float?), Default = null) ] //, Value = null !!!
    [Style(Name = "right", Type = typeof(float?), Default = null)]
    [Style(Name = "top", Type = typeof(float?), Default = null)]
    [Style(Name = "bottom", Type = typeof(float?), Default = null)]

    // paddings
    [Style(Name = "paddingLeft", Type = typeof(int), Default = 0)]
    [Style(Name = "paddingRight", Type = typeof(int), Default = 0)]
    [Style(Name = "paddingTop", Type = typeof(int), Default = 0)]
    [Style(Name = "paddingBottom", Type = typeof(int), Default = 0)]

    // margins
    /*[Style(Name = "marginLeft", Type = typeof(int), Default = 0)]
    [Style(Name = "marginRight", Type = typeof(int), Default = 0)]
    [Style(Name = "marginTop", Type = typeof(int), Default = 0)]
    [Style(Name = "marginBottom", Type = typeof(int), Default = 0)]*/

    [Style(Name = "color", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "contentColor", Type = typeof(Color), Default = 0xffffff)]

    [Style(Name = "overlayStyle", Type = typeof(GUIStyle), ProxyType = (typeof(ContainerOverlayStyle)))] // ProxyType = (typeof(ComponentStyleProxy)), ProxyMemberName = "OverlayStyle")]
    [Style(Name = "showOverlay", Type = typeof(bool), Default = false)]
    [Style(Name = "creationCompleteEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "showingEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "showEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "hideEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "addingEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "addedEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "removedEffect", Type = typeof(ITweenFactory))]
    [Style(Name = "usePointerCursor", Type = typeof(bool), Default = false)]

    #endregion

    public partial class Component : 
        InteractiveComponent,
        ILayoutElement, 
        IStyleClient,
        INavigatorDescriptor // can be added to tab navigator
    {

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
        ///<summary>
        ///</summary>
        public const float DEFAULT_MEASURED_WIDTH = 160;

        ///<summary>
        ///</summary>
        public const float DEFAULT_MEASURED_MIN_WIDTH = 40;

        ///<summary>
        ///</summary>
        public const float DEFAULT_MEASURED_HEIGHT = 22;

        ///<summary>
        ///</summary>
        public const float DEFAULT_MEASURED_MIN_HEIGHT = 22;
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global

        #region Properties

        /// <summary>
        /// Note: If resource is loaded via (Texture)Resources.Load("Icons/cart_product_info"); 
        /// then this parameter set to 'true' will reset all current and future instances of that resource!
        /// </summary>
        public bool AutoDisposeUnmanagedResources;

        #region Constructor

        public Component()
        {
            _shouldRenderBackground = !(this is Group);
        }

        #endregion

        #region INavigatorDescriptor

        /// <summary>
        /// A label that is displayed in TabNavigator and similar components
        /// </summary>
        public virtual string NavigatorDescriptor { get; set; }

        #endregion

        #region Size

// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual float XMin
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get { return X; }
        }

        /// <summary>
        /// Right edge of the component
        /// </summary>
        public virtual float XMax
        {
            get { return X + Width; }
        }

        /// <summary>
        /// Top edge of the component
        /// </summary>
        public virtual float YMin
        {
            get { return Y; }
        }

        /// <summary>
        /// Bottom edge of the component
        /// </summary>
        public virtual float YMax
        {
            get { return Y + Height; }
        }

        #endregion

        #region Events

        /// <summary>
        /// The event that fires when the component starts initialization
        ///</summary>
// ReSharper disable UnaccessedField.Global
        private MulticastDelegate _preinitialize;
        [Event(Name = FrameworkEvent.PREINITIALIZE, Type = typeof(Event))]
        public MulticastDelegate Preinitialize
        {
            get
            {
                if (null == _preinitialize)
                    _preinitialize = new MulticastDelegate(this, FrameworkEvent.PREINITIALIZE);
                return _preinitialize;
            }
            set
            {
                _preinitialize = value;
            }
        }
// ReSharper restore UnaccessedField.Global

        /// <summary>
        /// The event that fires when the component initializes
        ///</summary>
        //public MulticastDelegate InitializeHandler;

        private MulticastDelegate _initializeHandler;
        [Event(Name = FrameworkEvent.INITIALIZE, Type = typeof(Event))]
        public MulticastDelegate InitializeHandler
        {
            get
            {
                if (null == _initializeHandler)
                    _initializeHandler = new MulticastDelegate(this, FrameworkEvent.INITIALIZE);
                return _initializeHandler;
            }
            set
            {
                _initializeHandler = value;
            }
        }

        /// <summary>
        /// The event that fires when the component creation completes<br/>
        /// I.e. when first validation pass is finished
        ///</summary>
        //public MulticastDelegate CreationCompleteHandler;

        private MulticastDelegate _creationCompleteHandler;
        [Event(Name = FrameworkEvent.CREATION_COMPLETE, Type = typeof(Event))]
        public MulticastDelegate CreationCompleteHandler
        {
            get
            {
                if (null == _creationCompleteHandler)
                    _creationCompleteHandler = new MulticastDelegate(this, FrameworkEvent.CREATION_COMPLETE);
                return _creationCompleteHandler;
            }
            set
            {
                _creationCompleteHandler = value;
            }
        }

        /// <summary>
        /// The event that fires when the component updates
        ///</summary>
// ReSharper disable UnaccessedField.Global
        //public MulticastDelegate UpdateComplete;

        private MulticastDelegate _updateComplete;
        [Event(Name = FrameworkEvent.UPDATE_COMPLETE, Type = typeof(Event))]
        public MulticastDelegate UpdateComplete
        {
            get
            {
                if (null == _updateComplete)
                    _updateComplete = new MulticastDelegate(this, FrameworkEvent.UPDATE_COMPLETE);
                return _updateComplete;
            }
            set
            {
                _updateComplete = value;
            }
        }


// ReSharper restore UnaccessedField.Global

        //public MulticastDelegate MoveHandler;

        private MulticastDelegate _moveHandler;
        [Event(Name = MoveEvent.MOVE, Type = typeof(MoveEvent))]
        public MulticastDelegate MoveHandler
        {
            get
            {
                if (null == _moveHandler)
                    _moveHandler = new MulticastDelegate(this, MoveEvent.MOVE);
                return _moveHandler;
            }
            set
            {
                _moveHandler = value;
            }
        }

        //public MulticastDelegate ResizeHandler;

        private MulticastDelegate _resize;
        [Event(Name = ResizeEvent.RESIZE, Type = typeof(Event))]
        public MulticastDelegate Resize
        {
            get
            {
                if (null == _resize)
                    _resize = new MulticastDelegate(this, ResizeEvent.RESIZE);
                return _resize;
            }
            set
            {
                _resize = value;
            }
        }

        #endregion

        private GUIStyle _overlayStyle;
        public bool ShowOverlay;

        private GUIStyle _disabledOverlay;

        /// <summary>
        /// A flag set by popup manager to true if this is a popup
        /// </summary>
        public bool IsPopUp { get; internal set; }

        #endregion

        #region Render

        protected override void OverlayRender()
        {
            if (ShowOverlay && UnityEngine.Event.current.type == EventType.repaint) // moved to here 20120202
            {
                if (null != _overlayStyle)
                    /*_overlayStyle.Draw(RenderingRect,// Content, // ContainerStyleProxy.Instance.OverlayStyle
                                    this == MouseEventDispatcher.MouseTarget,
                                    this == FocusManager.Instance.FocusedComponent,
                                    false, false);*/

                    // 20130704
                    _overlayStyle.Draw(RenderingRect,// Content, // ContainerStyleProxy.Instance.OverlayStyle
                                       this == MouseEventDispatcher.MouseTarget,
                                       this == MouseEventDispatcher.MouseDownComponent,
                                       false,
                                       this == FocusManager.Instance.FocusedComponent);
            }

            //base.OverlayRender();

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                /**
                 * 1) draw overlay for if in disabled state
                 * */
                if (!Enabled)
                {

                    if (null != _disabledOverlay)
                    {
                        _disabledOverlay.Draw(RenderingRect,
                               this == MouseEventDispatcher.MouseTarget,
                               this == MouseEventDispatcher.MouseDownComponent,
                               false,
                               this == FocusManager.Instance.FocusedComponent); // 20130704
                        //true);
                    }
                }
            }

            //if (ShowOverlay && UnityEngine.Event.current.type == EventType.repaint) // commented out 20120202
            //{
            //    if (null != _overlayStyle)
            //        _overlayStyle.Draw(RenderingRect,// Content, // ContainerStyleProxy.Instance.OverlayStyle
            //                        this == MouseEventDispatcher.MouseTarget,
            //                        this == FocusManager.Instance.FocusedComponent,
            //                        false, false);
            //}
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            foreach (IPlugin plugin in Plugins)
            {
                plugin.Dispose();
            }
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Writes a console message if componentId corresponds to this component ID
        ///</summary>
        ///<param name="componentId">The ID of the compoennt for which the message will be logged</param>
        ///<param name="message">A message to log</param>
// ReSharper disable UnusedMember.Global
        public void LogIf(string componentId, string message)
// ReSharper restore UnusedMember.Global
        {
            if (componentId == Id)
                Debug.Log(message);
        }

        #endregion

        #region Drawing order

        //public bool AutoBringToFront;
        
        public bool StopMouseWheelPropagation;

        //private static void PushBranchToBack(Component leaf)
        //{
        //    if (null != leaf.Parent) //  && !(leaf.Owner is Stage)
        //    {
        //        //Debug.Log("Bringing to front: " + leaf);

        //        Container c = (Container)leaf.Parent;

        //        // If parent defined, trigger bringing to front this child
        //        c.PushChildToBack(leaf);
        //        // Do it recursivelly al the way up to Stage (which has no parent)
        //        //PushBranchToBack(leaf.Owner); // NONO
        //    }
        //}
        
        #endregion

        #region Parent - child

        ///<summary>
        /// Running when the parent is changed (used for setup the new parent and the stage)
        ///</summary>
        ///<param name="newParent"></param>
        public void ParentChanged(DisplayObjectContainer newParent)
        {
            //Debug.LogWarning("ParentChanged: " + this + "; Parent: " + newParent);

            if (null == newParent)
            {
                Parent = null;
                //Stage = null;
                NestLevel = 0;
            }
            else
            {
                Parent = newParent;
                Stage = newParent.Stage; // 20130921 -> this line is very important, because this is the place where the Stage reference is added to a component
            }
//            Debug.Log(string.Format(@"Parent changed. Component: {0}
//Parent: {1}", this, newParent));
        }

        public override DisplayListMember AddChild(DisplayListMember child)
        {
            DisplayObjectContainer formerParent = child.Parent;
            if (null != formerParent)
                formerParent.RemoveChild(child);

            // If there is an overlay, place the child underneath it.
            int index = base.NumberOfChildren;

            // Do anything that needs to be done before the child is added.
            // When adding a child to Component, this will set the child's
            // virtual parent, its nestLevel, its document, etc.
            // When adding a child to a Container, the override will also
            // invalidate the container, adjust its content/chrome partitions,
            // etc.
            AddingChild(child);

            // Call a low-level player method in DisplayObjectContainer which
            // actually attaches the child to this component.
            // The player dispatches an "added" event from the child just after
            // it is attached, so all "added" handlers execute during this call.
            // Component registers an addedHandler() in its constructor,
            // which makes it runs before any other "added" handlers except
            // capture-phase ones; it sets up the child's styles.
            QAddChildAt(child, index);

            // Do anything that needs to be done after the child is added
            // and after all "added" handlers have executed.
            // This is where
            ChildAdded(child);

            return child;
        }

        public override DisplayListMember AddChildAt(DisplayListMember child, int index)
        {
            DisplayObjectContainer formerParent = child.Parent;
            if (null != formerParent)
                formerParent.RemoveChild(child);

            AddingChild(child);

            QAddChildAt(child, index);

            ChildAdded(child);

            return child;
        }

        override public DisplayListMember RemoveChild(DisplayListMember child)
        {
            RemovingChild(child);

            QRemoveChild(child);

            ChildRemoved(child);

            return child;
        }

        override public DisplayListMember RemoveChildAt(int index) // TODO: Do delayed version
        {
            //DisplayListMember child = QGetChildAt(index);
            //return QRemoveChild(child);

            DisplayListMember child = GetChildAt(index);

            RemovingChild(child);

            QRemoveChild(child);

            ChildRemoved(child);

            return child;
        }

        #endregion

        #region Visibility

        private VisibilityPhase _visibilityPhase = VisibilityPhase.Idle;

        private enum VisibilityPhase
        {
            Showing, Hiding, Idle
        }

        public override void SetVisible(bool value, bool dispatchEvent)
        {
            // prevent showing
            if (value && VisibilityPhase.Showing != _visibilityPhase && IsDefaultPrevented(FrameworkEvent.SHOWING))
            {
                _visibilityPhase = VisibilityPhase.Showing;
                return;
            }

            // prevent hiding
            if (!value && VisibilityPhase.Hiding != _visibilityPhase && IsDefaultPrevented(FrameworkEvent.HIDING))
            {
                _visibilityPhase = VisibilityPhase.Hiding;
                return;
            }

            //switch (_visibilityPhase)
            //{
            //    case VisibilityPhase.Showing:
            //        IAnimation showEffect = GetStyle("showEffect") as IAnimation;
            //        Debug.Log("showEffect: " + showEffect);
            //        if (null != showEffect)
            //        {
            //            var tween = showEffect.Produce();
            //            tween.Play(this);
            //            return;
            //        }
            //        break;

            //    case VisibilityPhase.Hiding:
            //        IAnimation hideEffect = GetStyle("hideEffect") as IAnimation;
            //        Debug.Log("hideEffect: " + hideEffect);
            //        if (null != hideEffect)
            //        {
            //            var tween = hideEffect.Produce();
            //            tween.Callback = delegate { Visible = false; };
            //            tween.Play(this);
            //            return;
            //        }
            //        break;
            //}
            
            _visibilityPhase = VisibilityPhase.Idle;

            base.SetVisible(value, dispatchEvent);
        }

        #endregion

        #region Rotation

        //private Matrix4x4 _matrixBackup;

        //override protected void RotationStart()
        //{
        //    if (0 != Rotation)
        //    {
        //        _matrixBackup = GUI.matrix;
        //        GUIUtility.RotateAroundPivot(Rotation, Bounds.CenterAsVector2);
        //    }
        //}

        //override protected void RotationEnd()
        //{
        //    if (0 != Rotation)
        //    {
        //        GUI.matrix = _matrixBackup;
        //    }
        //}

        #endregion

        #region Invalidate/update display list overrides

        private bool _drawingListInvalidated;
        internal override void InvalidateDrawingList()
        {
            _drawingListInvalidated = true;
            //InvalidateDisplayList();
            InvalidateProperties();
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            // Handle a deferred state change request.
            if (_currentStateDeferred != null)
            {
                var newState = _currentStateDeferred;
                _currentStateDeferred = null;
                CurrentState = newState;
            }

            // Typically state changes occur immediately, but during
            // component initialization we defer until commitProperties to 
            // reduce a bit of the startup noise.
            if (_currentStateChanged && !Initialized)
            {
                _currentStateChanged = false;
                CommitCurrentState();
            }

            if (_drawingListInvalidated)
            {
                //Debug.Log("Calling DepthUtil.UpdateDrawingList on " + this);
                _drawingListInvalidated = false;
                DepthUtil.UpdateDrawingList(this);
            }
        }

        #endregion

        #region Child adding/added/removing/removed processing methods

        protected virtual void AddingChild(DisplayListMember child)
        {
            if (child is Component)
                ((Component)child).ParentChanged(this);

            if (child is IInvalidationManagerClient)
                ((IInvalidationManagerClient)child).NestLevel = /*NestLevel == -1 ? -1 : */NestLevel + 1;

            if (child is IStyleClient) // TEMP: TURNED OFF BECAUSE OF THE FREEZING BUG! TODO
                ((IStyleClient)child).RegenerateStyleCache(true);

            if (child is ISimpleStyleClient)
                ((ISimpleStyleClient)child).StyleChanged(null);

            if (child is IStyleClient)
                ((IStyleClient)child).NotifyStyleChangeInChildren(null, null, true);

            if (child is Component)
                ((Component)child).InitializeEffects();

            // Inform the component that it's style properties
            // have been fully initialized. Most components won't care,
            // but some need to react to even this early change.
            if (child is Component)
                ((Component)child).StylesInitialized();
        }

        protected virtual void ChildAdded(DisplayListMember child)
        {
            InvalidationManagerClient imc = child as InvalidationManagerClient;
            if (null == imc)
                return;

            if (!imc.Initialized)
                child.Initialize();
            else // already created, but changed parent
                child.PropagateStage();
        }

        // ReSharper disable MemberCanBeMadeStatic.Global
        protected virtual void RemovingChild(DisplayListMember child)
            // ReSharper restore MemberCanBeMadeStatic.Global
        {

        }

        protected virtual void ChildRemoved(DisplayListMember child)
        {
            if (child is Component)
            {
                ((Component)child).ParentChanged(null);
            }
        }

        #endregion

        #region ROLL_OVER / ROLL_OUT

        /// <summary>
        /// Overriding AddEventListener to test for mouse overs / mouse outs
        /// When listening for ROLL_OVER / ROLL_OUT on THIS, ignore the mouse overs on children
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        /// <param name="phases"></param>
        public override void AddEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            if (MouseEvent.ROLL_OVER == eventType)
            {
                MouseEventDispatcher.Instance.RegisterRollOverComponent(this);
            }
            else if (MouseEvent.ROLL_OUT == eventType)
            {
                MouseEventDispatcher.Instance.RegisterRollOverComponent(this);
            }

            base.AddEventListener(eventType, handler, phases);
        }

        public override void RemoveEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            if (MouseEvent.ROLL_OVER == eventType)
            {
                MouseEventDispatcher.Instance.UnregisterRollOverComponent(this);
            }
            else if (MouseEvent.ROLL_OUT == eventType)
            {
                MouseEventDispatcher.Instance.UnregisterRollOverComponent(this);
            }
            
            base.RemoveEventListener(eventType, handler, phases);
        }

        #endregion

        ///<summary>
        /// Called after the component styles are first initialized<br/>
        /// This happens upont the addition to the display list, and befor measuring<br/>
        /// You can setup values needed for measuring component here
        ///</summary>
// ReSharper disable MemberCanBeProtected.Global
        public virtual void StylesInitialized()
// ReSharper restore MemberCanBeProtected.Global
        {
        }

        #region IConstraintClient

        /// <summary>
        /// Returns the specified constraint value
        /// </summary>
        /// <param name="constraintName"></param>
        /// <returns></returns>
        public object GetConstraintValue(string constraintName)
        {
            return GetStyle(constraintName);
        }

        /// <summary>
        /// Sets the specified constraint value
        /// </summary>
        /// <param name="constraintName"></param>
        /// <param name="value"></param>
        public void SetConstraintValue(string constraintName, object value)
        {
            SetStyle(constraintName, value);
        }

        #endregion

        #region ILayoutElement

        //private float? _left;
        /// <summary>
        /// Horizontal distance to the left edge of the parent container
        /// </summary>
        public object Left
        {
            get
            {
                return GetConstraintValue("left");
            }
            set
            {
                SetConstraintValue("left", value);
            }
        }

        //private float? _right;
        /// <summary>
        /// Horizontal distance to the right edge of the parent container
        /// </summary>
        public object Right
        {
            get
            {
                return GetConstraintValue("right");
            }
            set
            {
                SetConstraintValue("right", value);
                //if (value == _right)
                //    return;

                //_right = value;
                //InvalidateTransform();
                //InvalidateParentSizeAndDisplayList();
            }
        }

        //private float? _top;
        /// <summary>
        /// Vertical distance to the top edge of the parent container
        /// </summary>
        public object Top
        {
            get
            {
                return GetConstraintValue("top");
            }
            set
            {
                SetConstraintValue("top", value);
            }
        }

        //private float? _bottom;
        /// <summary>
        /// Vertical distance to the bottom edge of the parent container
        /// </summary>
        public object Bottom
        {
            get
            {
                return GetConstraintValue("bottom");
            }
            set
            {
                SetConstraintValue("bottom", value);
            }
        }

        /// <summary>
        /// Horizontal offset of the component center to the container center
        /// </summary>
        public object HorizontalCenter
        {
            get
            {
                return GetConstraintValue("horizontalCenter");
            }
            set
            {
                SetConstraintValue("horizontalCenter", value);
            }
        }

        /// <summary>
        /// Vertical offset of the component center to the container center
        /// </summary>
        public object VerticalCenter
        {
            get
            {
                return GetConstraintValue("verticalCenter");
            }
            set
            {
                SetConstraintValue("verticalCenter", value);
            }
        }

        #endregion

        #region Color

        private Color _color = Color.white;
        /// <summary>
        /// Global tinting color for the GUI.
        /// This will affect both backgrounds & text colors. 
        /// <see cref="http://unity3d.com/support/documentation/ScriptReference/GUI-color.html"/>
        /// </summary>
        public Color Color
        {
            get { return (Color)GetStyle("color"); }
            set { SetStyle("color", value); }
        }

        private Color _backgroundColor = Color.white;
        /// <summary>
        /// Global tinting color for all background elements rendered by the GUI.
        /// This gets multiplied by color. 
        /// <see cref="http://unity3d.com/support/documentation/ScriptReference/GUI-backgroundColor.html"/>
        /// </summary>
        public Color BackgroundColor
        {
            get { return (Color)GetStyle("backgroundColor"); }
            set { SetStyle("backgroundColor", value); }
        }

        private Color _contentColor = Color.white;
        /// <summary>
        /// Tinting color for all text rendered by the GUI.
        /// This gets multiplied by color.  
        /// <see cref="http://unity3d.com/support/documentation/ScriptReference/GUI-contentColor.html"/>
        /// </summary>
        public Color ContentColor
        {
            get { return (Color)GetStyle("contentColor"); }
            set { SetStyle("contentColor", value); }
        }

        private Color _tempColor;
        private float _tempAlpha;

        private bool _shouldRenderBackground;

        private Color _preRenderColor;
        protected Color PreRenderColor
        {
            get
            {
                return _preRenderColor;
            }
        }

        private Color _preRenderBackgroundColor;
        protected Color PreRenderBackgroundColor
        {
            get
            {
                return _preRenderBackgroundColor;
            }
        }

        private Color _preRenderContentColor;

        protected Color PreRenderContentColor
        {
            get
            {
                return _preRenderContentColor;
            }
        }

        /// <summary>
        /// Process color
        /// </summary>
        protected override void ColorStart()
        {
            /**
             * 2) Process color
             * */
            if (Alpha < 1 || Color.white != _color) // set Alpha and/or color
            {
                // alpha multiplies with color alpha
                //float alpha = _alpha < 1 ? _alpha : _preRenderColor.a;
                _tempAlpha = Alpha * _preRenderColor.a;

                _preRenderColor = GUI.color;

                if (Color.white != _color)
                {
                    _tempColor = _color;
                    GUI.color = new Color(_tempColor.r, _tempColor.g, _tempColor.b, _tempAlpha);
                }
                else
                    GUI.color = new Color(1, 1, 1, _tempAlpha);
            }

            if (_shouldRenderBackground) // _shouldRenderBackground 20131231
            {
                if (Color.white != _backgroundColor)
                {
                    _preRenderBackgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = _backgroundColor;
                }

                if (Color.white != _contentColor)
                {
                    _preRenderContentColor = GUI.contentColor;
                    GUI.contentColor = _contentColor;
                }
            }
        }

        /// <summary>
        /// Reset color
        /// </summary>
        protected override void ColorEnd()
        {
            /**
             * Reset color
             * */
            if (Alpha < 1 || Color.white != _color) // set Alpha and/or color
            {
                GUI.color = _preRenderColor;
            }

            if (_shouldRenderBackground) // _shouldRenderBackground 20131231
            {
                if (Color.white != _backgroundColor)
                {
                    GUI.backgroundColor = _preRenderBackgroundColor;
                }

                if (Color.white != _contentColor)
                {
                    GUI.contentColor = _preRenderContentColor;
                }
            }

            ///**
            // * Dispatch event on first render
            // * */
            //if (!_wasVisible)
            //{
            //    _wasVisible = true;
            //    if (HasEventListener(FrameworkEvent.FIRST_SHOW))
            //        DispatchEvent(new FrameworkEvent(FrameworkEvent.FIRST_SHOW), false);
            //}
        }

        #endregion

        #region Effects

        /*public override void Initialize()
        {
            // initialize only once
            if (Initialized)
                return;

            // note: initialize effects firs, then the superclass stuff (adding children etc.)
            InitializeEffects();

            base.Initialize();
        }*/

        private void InitializeEffects()
        {
            var showEffect = GetStyle("showEffect") as ITweenFactory;
            if (null != showEffect)
                AddEventListener(FrameworkEvent.SHOW, delegate { showEffect.Play(this); });

            var hideEffect = GetStyle("hideEffect") as ITweenFactory;
            if (null != hideEffect)
                AddEventListener(FrameworkEvent.HIDE, delegate { hideEffect.Play(this); });

            var addedEffect = GetStyle("addedEffect") as ITweenFactory;
            if (null != addedEffect)
                AddEventListener(FrameworkEvent.ADD, delegate { addedEffect.Play(this); });

            var removedEffect = GetStyle("removedEffect") as ITweenFactory;
            if (null != removedEffect)
                AddEventListener(FrameworkEvent.REMOVE, delegate { removedEffect.Play(this); });
        }

        #endregion
    }
}