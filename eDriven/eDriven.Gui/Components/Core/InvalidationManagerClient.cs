using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Abstract class
    /// DisplayObjectContainer which interacts with InvalidationManager
    /// </summary>
    public abstract class InvalidationManagerClient : DisplayObjectContainer, IInvalidationManagerClient, IInvalidating
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region IEventQueue

        public override void EnqueueEvent(Event e)
        {
            base.EnqueueEvent(e);

            if (_eventQueueInvalidated) return; // || -1 == NestLevel) return;

            if (-1 != NestLevel)
                InvalidationManager.Instance.InvalidateEventQueue(this);

            _eventQueueInvalidated = true;
        }

        public override void ProcessQueue()
        {
            base.ProcessQueue();

            _eventQueueInvalidated = true;
        }

        #endregion

        #region Constructor

        protected InvalidationManagerClient()
        {
            // Make the component invisible until the initialization sequence is complete.
            // It will be set visible when the 'initialized' flag is set.
            QVisible = false;

            _width = base.Width; // 20130806
            _height = base.Height;
        }

        #endregion

        #region Transform

        protected override void InitTransform()
        {
            Transform = new InvalidationManagerClientTransform(this);
        }

        /// <summary>
        /// X coordinate
        /// </summary>
        public override float X
        {
            get
            {
                return base.X;
            }
            set
            {
                if (value == X)
                    return;

                base.X = value;

                InvalidateProperties();
                //InvalidateTransform(); // runs in CommitProperties()

                if (null != Parent && Parent is Component)
                    ((Component)Parent).ChildXYChanged();

                if (HasEventListener(FrameworkEvent.X_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.X_CHANGED));
            }
        }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public override float Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                if (value == Y)
                    return;

                base.Y = value;

                InvalidateProperties();
                //InvalidateTransform(); // runs in CommitProperties()

                if (null != Parent && Parent is Component)
                    ((Component)Parent).ChildXYChanged();

                if (HasEventListener(FrameworkEvent.Y_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.Y_CHANGED));
            }
        }

        internal virtual void ChildXYChanged()
        {

        }

        private float _width;
        public override float Width
        {
            get
            {
                return _width; // base.Width;
            }
            set
            {
                if (_explicitWidth != value)
                {
                    ExplicitWidth = value;

                    // We invalidate size because locking in width
                    // may change the measured height in flow-based components.
                    InvalidateSize();
                }

                //if (base.Width != value)
                if (_width != value)
                {
                    InvalidateProperties();
                    InvalidateDisplayList();
                    //InvalidateTransform(); // runs in CommitProperties()
                    InvalidateParentSizeAndDisplayList();

                    _width = value; //base.Width = value;

                    if (HasEventListener(FrameworkEvent.WIDTH_CHANGED)) // "widthChanged"
                        DispatchEvent(new Event(FrameworkEvent.WIDTH_CHANGED));
                }
            }
        }

        private float _height;
        public override float Height
        {
            get
            {
                //return base.Height;
                return _height;
            }
            set
            {
                if (_explicitHeight != value)
                {
                    ExplicitHeight = value;

                    // We invalidate size because locking in width
                    // may change the measured height in flow-based components.
                    InvalidateSize();
                }

                if (_height != value) // base.Height != value)
                {
                    InvalidateProperties();
                    InvalidateDisplayList();
                    //InvalidateTransform(); // runs in CommitProperties()
                    InvalidateParentSizeAndDisplayList();

                    _height = value;

                    if (HasEventListener(FrameworkEvent.HEIGHT_CHANGED)) // "heightChanged"
                        DispatchEvent(new Event(FrameworkEvent.HEIGHT_CHANGED));
                }
            }
        }

        #endregion

        #region Properties

        private bool _resizeWithStyleBackground;
        /// <summary>
        /// A flag indicating if component size depends of the style background size
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual bool ResizeWithStyleBackground
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                return _resizeWithStyleBackground;
            }
            set
            {
                if (value == _resizeWithStyleBackground)
                    return;
                _resizeWithStyleBackground = value;
                InvalidateSize();
            }
        }

        private bool _resizeWithContent = true; // true is the default
        /// <summary>
        /// A flag indicating if component size depends of the content size
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual bool ResizeWithContent
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                return _resizeWithContent;
            }
            set
            {
                if (value == _resizeWithContent)
                    return;
                _resizeWithContent = value;
                InvalidateSize();
            }
        }

        private float? _explicitMinWidth;

        /// <summary>
        /// Explicit min width
        /// </summary>
        public virtual float? ExplicitMinWidth
        {
            get { return _explicitMinWidth; }
            set
            {
                if (value == _explicitMinWidth)
                    return;

                _explicitMinWidth = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();

                if (HasEventListener(FrameworkEvent.EXPLICIT_MIN_WIDTH_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.EXPLICIT_MIN_WIDTH_CHANGED));
            }
        }

        private float? _explicitMinHeight;

        /// <summary>
        /// Explicit min height
        /// </summary>
        public virtual float? ExplicitMinHeight
        {
            get { return _explicitMinHeight; }
            set
            {
                if (value == _explicitMinHeight)
                    return;

                _explicitMinHeight = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();

                if (HasEventListener(FrameworkEvent.EXPLICIT_MIN_HEIGHT_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.EXPLICIT_MIN_HEIGHT_CHANGED));
            }
        }

        private float? _explicitMaxWidth;

        /// <summary>
        /// Explicit max width
        /// </summary>
        public virtual float? ExplicitMaxWidth
        {
            get { return _explicitMaxWidth; }
            set
            {
                if (value == _explicitMaxWidth)
                    return;

                _explicitMaxWidth = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();

                if (HasEventListener(FrameworkEvent.EXPLICIT_MAX_WIDTH_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.EXPLICIT_MAX_WIDTH_CHANGED));
            }
        }

        private float? _explicitMaxHeight;

        /// <summary>
        /// Explicit max height
        /// </summary>
        public virtual float? ExplicitMaxHeight
        {
            get { return _explicitMaxHeight; }
            set
            {
                if (value == _explicitMaxHeight)
                    return;

                _explicitMaxHeight = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();

                if (HasEventListener(FrameworkEvent.EXPLICIT_MAX_HEIGHT_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.EXPLICIT_MAX_HEIGHT_CHANGED));
            }
        }

        /// <summary>
        /// Minimal width
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual float MinWidth
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                if (null != _explicitMinWidth)
                    return (float) _explicitMinWidth;

                return MeasuredMinWidth;
            } 
            set { ExplicitMinWidth = value; }
        }

        /// <summary>
        /// Minimal height
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual float MinHeight
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                if (null != _explicitMinHeight)
                    return (float)_explicitMinHeight;

                return MeasuredMinHeight;
            }
            set { ExplicitMinHeight = value; }
        }

        /// <summary>
        /// Maximal Width for percentage-based component
        /// </summary>
        public virtual float MaxWidth
        {
            get
            {
                return _explicitMaxWidth ?? LayoutUtil.DEFAULT_MAX_WIDTH;
            }
            set
            {
                /*if (ExplicitMaxWidth == value)
                    return;*/

                ExplicitMaxWidth = value;
            }
        }

        /// <summary>
        /// Maximal Height for percentage-based component
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual float MaxHeight
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                return _explicitMaxHeight ?? LayoutUtil.DEFAULT_MAX_HEIGHT;
            }
            set
            {
                /*if (ExplicitMaxHeight == value)
                    return;*/

                ExplicitMaxHeight = value;
            }
        }

        private float? _explicitWidth;
        /// <summary>
        /// The explicit width of the component (in pixels)<br/>
        /// When set, it resets PercentWidth property to null
        /// </summary>
        public float? ExplicitWidth
        {
            get { return _explicitWidth; }
            set
            {
                if (value == _explicitWidth)
                    return;

                if (null != value)
                    _percentWidth = null;

                _explicitWidth = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();
            }
        }

        private float? _explicitHeight;
        /// <summary>
        /// The explicit height of the component (in pixels)<br/>
        /// When set, it resets PercentHeight property to null
        /// </summary>
        public float? ExplicitHeight
        {
            get { return _explicitHeight; }
            set
            {
                if (value == _explicitHeight)
                    return;

                if (null != value)
                    _percentHeight = null;

                _explicitHeight = value;

                InvalidateSize();
                InvalidateParentSizeAndDisplayList();
            }
        }

        #region IPercentage

        private float? _percentWidth;
        /// <summary>
        /// The percent height of the component (in pixels)<br/>
        /// When set, it resets ExplicitHeight property to null
        ///</summary>
        public virtual float? PercentWidth
        {
            get { return _percentWidth; }
            set
            {
                if (_percentWidth == value)
                    return;

                if (null != value)
                    _explicitWidth = null;

                _percentWidth = value;

                InvalidateParentSizeAndDisplayList();
            }
        }

        private float? _percentHeight;
        /// <summary>
        /// The percent height of the component (in pixels)<br/>
        /// When set, it resets ExplicitHeight property to null
        ///</summary>
        public virtual float? PercentHeight
        {
            get { return _percentHeight; }
            set
            {
                if (_percentHeight == value)
                    return;

                if (null != value)
                    _explicitHeight = null;

                _percentHeight = value;
                
                InvalidateParentSizeAndDisplayList();
            }
        }

        #endregion

        private bool _forceLayout;
        /**
         *  
         *  Remember when a child has been added or removed.
         *  When that occurs, we want to run the LayoutManager
         *  (even if autoLayout is false).
         */
        internal bool ForceLayout
        {
            get
            {
                return _forceLayout;
            }
            set
            {
                if (value == _forceLayout)
                    return;

                _forceLayout = value;

                if (_forceLayout)
                    InvalidationManager.Instance.UpdateCompleteSignal.Connect(LayoutUpdateCompleteSlot, 50, true);
            }
        }

        /// <summary>
        /// The purpose of connecting to invalidation manager is to reset the ForceLayout flag after the all layout passes are finished
        /// We are connecting in the "autoDisconnect" mode, so no need of disconnecting here
        /// </summary>
        /// <param name="parameters"></param>
        private void LayoutUpdateCompleteSlot(params object[] parameters)
        {
            _forceLayout = false;
            OnLayoutUpdateComplete();
        }

        protected virtual void OnLayoutUpdateComplete()
        {
            
        }

        #endregion
        
        #region Members

        private bool _propertiesInvalidated;
        private bool _sizeInvalidated;
        private bool _transformInvalidated;
        internal bool DisplayListInvalidated;
        private bool _eventQueueInvalidated;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the component
        /// Called by the parent upon first addition
        /// At this state all the properties have been set
        /// Sets child's Owner and reference to Stage
        /// Sets Skin
        /// </summary>
        public override void Initialize()
        {
            // initialize only once
            if (Initialized)
                return;

            // dispatch PREINITIALIZE event
            if (HasEventListener(FrameworkEvent.PREINITIALIZE))
                DispatchEvent(new FrameworkEvent(FrameworkEvent.PREINITIALIZE));

            //InitializeEffects();

            // create children now
            CreateChildren();

            // dispatch INITIALIZE event
            if (HasEventListener(FrameworkEvent.INITIALIZE))
                DispatchEvent(new FrameworkEvent(FrameworkEvent.INITIALIZE));

            // invalidate all the components that had NestLevel = -1 when tried to be invalidated upon adition
            InvalidateEverything();

            // consolidate
            ChildrenCreated();

            // content initialization
            base.Initialize();

            //Initialized = true;

            // initialization complete
            InitializationComplete();
        }

        /// <summary>
        /// Performs any final processing after child objects are created.
        /// This method is being called upon children creation and after the hierarchy is built (NestLevel is set)
        /// </summary>
        private void InvalidateEverything()
        {
            //if (this is List)
            //    Debug.Log("### InvalidateEverything: " + this + "###");
            //Debug.Log("### InvalidateEverything: " + this + "###");

            //InvalidateStyles();
            InvalidateProperties();
            InvalidateSize();
            InvalidateTransform();
            InvalidateDisplayList();
        }

        //private void InitializeEffects()
        //{
        //    var showEffect = GetStyle("showEffect") as ITween;
        //    if (null != showEffect)
        //        AddEventListener(FrameworkEvent.SHOW, delegate { showEffect.Play(this); });

        //    var hideEffect = GetStyle("hideEffect") as ITween;
        //    if (null != hideEffect)
        //        AddEventListener(FrameworkEvent.HIDE, delegate { hideEffect.Play(this); });

        //    var addedEffect = GetStyle("addedEffect") as ITween;
        //    if (null != addedEffect)
        //        AddEventListener(FrameworkEvent.ADD, delegate { addedEffect.Play(this); });

        //    var removedEffect = GetStyle("removedEffect") as ITween;
        //    if (null != removedEffect)
        //        AddEventListener(FrameworkEvent.ADD, delegate { removedEffect.Play(this); });
        //}

        /// <summary>
        /// Creates component's children
        /// </summary>
        protected virtual void CreateChildren()
        {

        }

        /// <summary>
        /// Runs after all the children have been created
        /// </summary>
// ReSharper disable once VirtualMemberNeverOverriden.Global
        protected virtual void ChildrenCreated()
        {
            //InvalidateProperties();
            //InvalidateStyles();
            //InvalidateSize();
            //InvalidatePosition();
            //InvalidateDisplayList();
        }

        // ReSharper disable VirtualMemberNeverOverriden.Global
        /// <summary>
        /// Runs after the initialization has been completed
        /// </summary>
        protected virtual void InitializationComplete()
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            InternalStructureBuilt = true;
        }

        /// <summary>
        /// Runs after the creation hase been completed
        /// </summary>
        protected virtual void CreationComplete()
        {
            //var effect = GetStyle("creationCompleteEffect") as ITween;
            //if (null != effect)
            //    effect.Play(this);
        }

        #endregion

        #region Implementation of IInvalidating

        /// <summary>
        /// A lifecycle method
        /// Invalidates properties
        /// </summary>
        public virtual void InvalidateProperties()
        {
            //Debug.Log("InvalidateProperties: " + NamingUtil.DisplayListMemberToString(this));
            if (_propertiesInvalidated) return; // || -1 == NestLevel) return;

            _propertiesInvalidated = true;

            //if (-1 != NestLevel)
            if (null != Parent)
                InvalidationManager.Instance.InvalidateProperties(this);
        }
        
        /// <summary>
        /// A lifecycle method
        /// Invalidates position
        /// </summary>
        public override void InvalidateTransform()
        {
            //Debug.Log("InvalidateTransform: " + NamingUtil.DisplayListMemberToString(this));
            if (_transformInvalidated) return; // || -1 == NestLevel) return;

            _transformInvalidated = true;

            //if (null != Parent)
            //    Parent.Transform.ValidateChild(this); // TEMP

            if (-1 != NestLevel)
                //if (null != Parent)
                //InvalidationManager.Instance.InvalidateTransform(this);
                Transform.Invalidate();
        }

        /// <summary>
        /// A lifecycle method
        /// Invalidates size
        /// </summary>
        public virtual void InvalidateSize()
        {
            //if (this is NumericStepper)
            //    Debug.LogWarning("Invalidating NumericStepper size. NestLevel: " + NestLevel);
            //if (Id == "hbox")
            //    Debug.Log("InvalidateSize: " + NamingUtil.DisplayListMemberToString(this));

            if (_sizeInvalidated) return; // || -1 == NestLevel) return;

            _sizeInvalidated = true;

            //if (-1 != NestLevel)
            if (null != Parent)
                InvalidationManager.Instance.InvalidateSize(this);
        }

        /// <summary>
        /// A lifecycle method
        /// Invalidates layout
        /// </summary>
        public virtual void InvalidateDisplayList()
        {
            //if (Id == "hbox")
            //    Debug.Log("InvalidateDisplayList: " + NamingUtil.DisplayListMemberToString(this));

            if (DisplayListInvalidated) return; // || -1 == NestLevel) return;
            
            DisplayListInvalidated = true;

            //if (Id == "hbox")
            //    Debug.Log("InvalidateDisplayList: " + NamingUtil.DisplayListMemberToString(this));

            if (-1 != NestLevel)
            //if (null != Parent) // NOTE: if using null != Parent, nothing will be displayed -> investigate
                InvalidationManager.Instance.InvalidateDisplayList(this);
        }

        ///<summary>
        /// Invalidates parent's size and layout
        /// if parent exists
        /// Should be internal, but cannot because of the interface
        ///</summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void InvalidateParentSizeAndDisplayList()
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            //if (!IncludeInLayout) // commented on 20120815 because when something excluded from layout, the parent didn't resized
            //    return;

            if (null == Parent)
                return;

            /* dirty */
            InvalidationManagerClient parent = Parent as InvalidationManagerClient ?? // container is a direct parent
                                          Parent.Parent as InvalidationManagerClient; // must be a content pane

            if (null != parent)
            {
                parent.InvalidateSize();
                parent.InvalidateDisplayList();
            }
        }

        #endregion

        #region Implementation of IInvalidationManagerClient

        //public override int NestLevel
        //{
        //    get
        //    {
        //        return base.NestLevel;
        //    }
        //    set
        //    {
        //        if (value != base.NestLevel)
        //        {
        //            base.NestLevel = value;
        //            UpdateCallbacks();
        //        }
        //    }
        //}

        //#region NestLevel

        private int _nestLevel = -1;
        /// <summary>
        /// NestLevel<br/>
        /// Used for invalidation: this is a property of the DisplayObject class because DisplayObjects<br/>
        /// could also have InvalidationManagerClients as children, so they must know how to pass the NestLevel to these children
        /// </summary>
        public virtual int NestLevel
        {
            get
            {
                return _nestLevel;
            }
            set
            {
                if (value > 1 && value != _nestLevel)
                {
                    _nestLevel = value;
                    UpdateCallbacks();

                    QChildren.ForEach(delegate(DisplayListMember child)
                    {
                        InvalidationManagerClient imc = child as InvalidationManagerClient;
                        if (null != imc)
                            imc.NestLevel = value + 1;
                    });
                }
            }
        }

        #endregion

        private bool _internalStructureBuilt;
        /// <summary>
        /// Is the component initialized (added to parent for the first time)
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        protected virtual bool InternalStructureBuilt
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get
            {
                return _internalStructureBuilt;
            }
            set
            {
                _internalStructureBuilt = value;

                // dispatch INITIALIZE event
                if (value)
                {
                    //DispatchEvent(new FrameworkEvent(FrameworkEvent.INITIALIZE)); // commented on 20120824 (noticed duplicated OnInitialize run upon initializetion)
                    //Debug.Log(string.Format("*** [{0}] INITIALIZE *** ", this));
                }
            }
        }

        private bool _initialized;
        /// <summary>
        /// A flag that determines if an object has been through all three phases
        /// of layout: commitment, measurement, and layout (provided that any were required)
        /// </summary>
        public virtual bool Initialized
        {
            get
            {
                return _initialized;
            }
            set
            {
                if (value == _initialized)
                    return;

                _initialized = value; // - 20120206

                if (value)
                {
                    //if (HasEventListener(FrameworkEvent.PRE_CREATION_COMPLETE))
                    //    DispatchEvent(new FrameworkEvent(FrameworkEvent.PRE_CREATION_COMPLETE));

                    InitPlugins();

                    SetVisible(_visible, true);

                    //_initialized = true; // + 20120206 // because of the show effect started playing on the application start

                    TriggerEffect("creationCompleteEffect");

                    CreationComplete();

                    if (HasEventListener(FrameworkEvent.CREATION_COMPLETE))
                        DispatchEvent(new FrameworkEvent(FrameworkEvent.CREATION_COMPLETE));
                }
            }
        }

        private bool _visible = true; // true -> CRUTIAL!!!
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                ////if (value == _visible)
                ////    return; // nothing changed

                //bool immediate = false;
                //if (value) // show
                //{
                //    //ITween anim = GetStyle("showEffect") as ITween;
                //    immediate = true;
                //    //if (null != anim/* && "Alpha" == anim.Property*/)
                //    //{
                //    //    //Debug.Log("showEffect: " + this);
                //    //    anim.Play(this);
                //    //}   
                //}
                //else // hide
                //{
                //    //ITween anim = GetStyle("hideEffect") as ITween;
                //    //if (null != anim && "Alpha" == anim.Property)
                //    //{
                //    //    Debug.Log("hideEffect: " + this);
                //    //    anim.Callback = delegate { SetVisible(false, true); };
                //    //    anim.Play(this);
                //    //}
                //    //else
                //        immediate = true;
                //}

                //if (immediate)
                SetVisible(value, true);
            }
        }

        /**
         *  Called when the <code>visible</code> property changes.
         *  Set the <code>visible</code> property to show or hide
         *  a component instead of calling this method directly.
         *
         *  Param: value The new value of the <code>visible</code> property.
         *  Specify <code>true</code> to show the component, and <code>false</code> to hide it.
         *
         *  Param: noEvent If <code>true</code>, do not dispatch an event.
         *  If <code>false</code>, dispatch a <code>show</code> event when
         *  the component becomes visible, and a <code>hide</code> event when
         *  the component becomes invisible.
         */
        public virtual void SetVisible(bool value, bool dispatchEvent/*=true*/)
        {
            //// prevent showing
            //if (value && VisibilityPhase.Showing != _visibilityPhase && IsDefaultPrevented(FrameworkEvent.SHOWING))
            //{
            //    _visibilityPhase = VisibilityPhase.Hiding;
            //    return;
            //}

            //// prevent hiding
            //if (!value && VisibilityPhase.Hiding != _visibilityPhase && IsDefaultPrevented(FrameworkEvent.HIDING))
            //{
            //    _visibilityPhase = VisibilityPhase.Hiding;
            //    return;
            //}

            //_visibilityPhase = VisibilityPhase.Idle;

            //HasBeenHidden = !value;

            TriggerEffect("showingEffect");

            _visible = value;

            if (!Initialized)
                return;

            if (QVisible == value)
                return;

            QVisible = value;

            if (value)
                TriggerEffect("showEffect");
            else
                TriggerEffect("hideEffect");

            if (dispatchEvent)
            {
                DispatchEvent(new FrameworkEvent(value ? FrameworkEvent.SHOW : FrameworkEvent.HIDE));
            }
        }

        /// <summary>
        /// Update flag
        /// </summary>
        internal bool UpdateFlag { get; set; }

        /// <summary>
        /// A lifecycle method
        /// Validates all (properties, position, size and layout)
        /// </summary>
        public virtual void ValidateNow()
        {
            /*if (-1 != NestLevel)
                InvalidationManager.Instance.ValidateNow(this);*/

            InvalidationManager.Instance.ValidateClient(this/*, false*/); // do not skip display list
        }

        /// <summary>
        /// A lifecycle method
        /// Validates properties
        /// Top-down
        /// </summary>
        public virtual void ValidateProperties()
        {
            _propertiesInvalidated = false;
            
#if DEBUG
            if (DebugMode)
                Debug.Log(this + " -> ValidateProperties()");
#endif

            CommitProperties();
        }

        /// <summary>
        /// Validates position
        /// A lifecycle method
        /// Top-down
        /// </summary>
        public virtual void ValidateTransform()
        {
            _transformInvalidated = false;

#if DEBUG
            if (DebugMode)
                Debug.Log(this + " -> ValidateTransform()");
#endif
            /* VALIDATING TRANSFORM HERE */
            Transform.Validate();
        }

        internal override GUIStyle ActiveStyle
        {
            get
            {
                return base.ActiveStyle;
            }
            set
            {
                base.ActiveStyle = value;

                if (ResizeWithStyleBackground) // 20120505
                    InvalidateSize();
            }
        }

//        private InvalidationManagerClient _childImc;

//        /// <summary>
//        /// Clears the altered styles, alowing the reading of the new style definition
//        /// Used by the designer for "live" changing styles using the style mappers
//        /// We are flushing the styles recursivelly, on all children
//        /// </summary>
//        public void FlushStyles()
//        {
//#if DEBUG
//            if (DebugMode)
//            {
//                Debug.Log("FlushStyles: " + this + " [initialized: " + Initialized + "]");
//            }
//#endif
//            _alteredStyles.Clear();
//            //_styles.Clear();
//            InvalidateSize();
//            foreach (DisplayListMember child in Children)
//            {
//                _childImc = child as InvalidationManagerClient;
//                if (null != _childImc) {
//                    _childImc.FlushStyles();
//                }
//            }
//        }

        /// <summary>
        /// Plays the styled effect if such style set
        /// </summary>
        /// <param name="effectStyleName"></param>
        /// <returns></returns>
        override public bool TriggerEffect(string effectStyleName)
        {
            return TriggerEffect(effectStyleName, this);
        }

        /// <summary>
        /// Validates size layout
        /// A lifecycle method
        /// <b>Bottom-up</b>
        /// </summary>
        public virtual void ValidateSize(bool recursive = false)
        {
            //if (this is Label)
            //    Debug.Log(this + " -> ValidateSize(): " + this);

#if DEBUG
            if (DebugMode)
                Debug.Log(this + " -> ValidateSize()");
#endif
            if (recursive)
            {
                for (int i = 0; i < NumberOfChildren; i++)
                {
                    DisplayObject child = GetChildAt(i);
                    if (child is IInvalidationManagerClient)
                        (child as IInvalidationManagerClient).ValidateSize(true);
                }
            }

            if (_sizeInvalidated)
            {
                /*if (Id == "viewer" || Id == "image")
                    Debug.Log(Id + " - Validating size for " + this);*/

                bool sizeChanging = MeasureSizes();

                /*if (Id == "viewer" || Id == "image")
                { 
                    Debug.Log(Id + " - sizeChanging: " + sizeChanging);
                    if (sizeChanging)
                    {
                        Debug.Log(Id + " - MeasuredWidth: " + MeasuredWidth + "; MeasuredHeight: " + MeasuredHeight);
                    }
                }*/

                if (sizeChanging && IncludeInLayout)
                {
                    InvalidateDisplayList();
                    InvalidateParentSizeAndDisplayList();
                }
            }
        }

        #region Measurement

        /// <summary>
        /// Measured width
        /// Set by the measuring process
        /// </summary>
        public float MeasuredWidth;

        /// <summary>
        /// Measured height
        /// Set by the measuring process
        /// </summary>
        public float MeasuredHeight;

        /// <summary>
        /// Measured min width
        /// The min width of the component
        /// Set by the measuring process
        /// </summary>
        public float MeasuredMinWidth;

        /// <summary>
        /// Measured min height
        /// The min height of the component
        /// Set by the measuring process
        /// </summary>
        public float MeasuredMinHeight;
        
        /**
         *  
         *  Holds the last recorded value of the x property.
         *  Used in dispatching a MoveEvent.
         */
        private float _oldX;

        /**
         *  
         *  Holds the last recorded value of the y property.
         *  Used in dispatching a MoveEvent.
         */
        private float _oldY;

        /**
         *  
         *  Holds the last recorded value of the width property.
         *  Used in dispatching a ResizeEvent.
         */
        private float _oldWidth;

        /**
         *  
         *  Holds the last recorded value of the height property.
         *  Used in dispatching a ResizeEvent.
         */
        private float _oldHeight;

        /**
         *  
         *  Holds the last recorded value of the minWidth property.
         */
        private float? _oldMinWidth;

        /**
         *  
         *  Holds the last recorded value of the minHeight property.
         */
        private float? _oldMinHeight;

        /**
         *  
         *  Holds the last recorded value of the explicitWidth property.
         */
        private float? _oldExplicitWidth;

        /**
         *  
         *  Holds the last recorded value of the explicitHeight property.
         */
        private float? _oldExplicitHeight;

        protected bool CanSkipMeasurement()
        {
            // We can skip the measure function if the object's width and height
            // have been explicitly specified (e.g.: the object's MXML tag has
            // attributes like width="50" and height="100").
            //
            // If an object's width and height have been explicitly specified,
            // then the explicitWidth and explicitHeight properties contain
            // Numbers (as opposed to NaN)

            //if (null != _explicitWidth && null != _explicitHeight)
            //{
            //    Debug.Log(string.Format("CanSkipMeasurement returns true for {0} [_explicitWidth: {1}, _explicitHeight: {2}", this, _explicitWidth, _explicitHeight));
            //}

            //if (Id == "hbox")
            //    Debug.Log("MeasureSizes.._explicitWidth: " + _explicitWidth);
            //if (Id == "hbox")
            //    Debug.Log("MeasureSizes.._explicitHeight: " + _explicitHeight);

            return null != _explicitWidth && null != _explicitHeight;
        }

        /// <summary>
        /// The flag is telling if the measurement optimization is turned on
        /// If it is, then if CanSkipMeasurement() returns true (because of the explicit sizes)
        /// the component never gets measured
        /// However, when changing the component dimensions from the outside (using Resizable for instance)
        /// we would like to know min/max dimensions to know how much we can contract/expand
        /// Thus we should switch this optimization off for this kind of components
        /// NOTE: true by default
        /// </summary>
        public bool OptimizeMeasureCalls = true;

        private bool MeasureSizes()
        {
            //Debug.Log("MeasureSizes: " + this);
            //if (Id == "hbox")
            //    Debug.Log("MeasureSizes: " + this);

            bool changed = false;

            if (!_sizeInvalidated)
                return false;

            //if (Id == "hbox")
            //    Debug.Log("MeasureSizes2: " + this);

            //if (Id == "hbox")
            //    Debug.Log("MeasureSizes..CanSkipMeasurement: " + CanSkipMeasurement());

            if (OptimizeMeasureCalls && CanSkipMeasurement())
            {
                //Debug.Log("    skip");
                //Debug.Log("miki1 " + _explicitWidth + ", " + _explicitHeight);
                _sizeInvalidated = false;
                MeasuredMinWidth = 0;
                MeasuredMinHeight = 0;
            }
            else
            {
                //if (Id == "hbox")
                //    Debug.Log("MeasureSizes3: " + this);
                //Debug.Log("About to measure: " + this);
                Measure();

                _sizeInvalidated = false;

                if (null != _explicitMinWidth && MeasuredWidth < _explicitMinWidth)
                    MeasuredWidth = (float) _explicitMinWidth;

                if (null != _explicitMaxWidth && MeasuredWidth > _explicitMaxWidth)
                    MeasuredWidth = (float) _explicitMaxWidth;

                if (null != _explicitMinHeight && MeasuredHeight < _explicitMinHeight)
                    MeasuredHeight = (float) _explicitMinHeight;

                if (null != _explicitMaxHeight && MeasuredHeight > _explicitMaxHeight)
                    MeasuredHeight = (float) _explicitMaxHeight;
            }

            if (null == _oldMinWidth)
            {
                // This branch does the same thing as the else branch,
                // but it is optimized for the first time that
                // measureSizes() is called on this object.
                _oldMinWidth = _explicitMinWidth ?? MeasuredMinWidth;

                _oldMinHeight = _explicitMinHeight ?? MeasuredMinHeight;

                _oldExplicitWidth = _explicitWidth ?? MeasuredWidth;

                _oldExplicitHeight = _explicitHeight ?? MeasuredHeight;

                changed = true;
            }
            else
            {
                float newValue = _explicitMinWidth ?? MeasuredMinWidth;

                if (newValue != _oldMinWidth)
                {
                    _oldMinWidth = newValue;
                    changed = true;
                }

                newValue = _explicitMinHeight ?? MeasuredMinHeight;
                if (newValue != _oldMinHeight)
                {
                    _oldMinHeight = newValue;
                    changed = true;
                }

                newValue = _explicitWidth ?? MeasuredWidth;
                if (newValue != _oldExplicitWidth)
                {
                    _oldExplicitWidth = newValue;
                    changed = true;
                }

                newValue = _explicitHeight ?? MeasuredHeight;

                if (newValue != _oldExplicitHeight)
                {
                    _oldExplicitHeight = newValue;
                    changed = true;
                }
            }

            //if (Id == "test")
            //{
            //    Debug.Log(string.Format("Measure sizes on 'test' measured: {0}, {1}", MeasuredWidth, MeasuredHeight));
            //}

            //if (changed)
            //    Debug.Log(string.Format("   MeasureSizes measured: {0}, {1} [{2}]", MeasuredWidth, MeasuredHeight, this));

            return changed;
        }

        // NOTE: In SetActual.. methods we are using Bounds instead of Width and Height
        // this is because Width and Height set explicit width and height
        // we do not want that if we want the component to auto-resize with it's content (for instance "Text")
        // so, these are a separate methods for setting the "temporary" dimensions, while not setting it as explicit
        // NOTE: If the component has both explicit values set, it won't re-measure itself after the initial measure
        // meaning there will be no auto-sizing

        /// <summary>
        /// Sets the temporary size
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void SetActualSize(float w, float h)
        {
            //if (Id == "hbox")
            //    Debug.Log("SetActualSize " + Id);

            bool changed = false;

            //if (Id == "horiz")
            //    Debug.Log("horiz SetActualSize: " + w + ", " + h);

            //Debug.Log("SetActualSize _width: " + _width + "; changing to: " + w + "; _height: " + _height + "; changing to: " + h);

            if (_width != w) // TODO loss of precision
            {
                _width = w;
                if (HasEventListener(FrameworkEvent.WIDTH_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.WIDTH_CHANGED));
                changed = true;
            }

            if (_height != h) // TODO loss of precision
            {
                _height = h;
                if (HasEventListener(FrameworkEvent.HEIGHT_CHANGED))
                    DispatchEvent(new Event(FrameworkEvent.HEIGHT_CHANGED));
                changed = true;
            }

            if (changed)
            {
                //if (Id == "vscroll")
                //    Debug.Log("vscroll: " + Width + ", " + Height);

                InvalidateDisplayList();
                InvalidateTransform();

                DispatchResizeEvent();
            }
        }

        #endregion

        /// <summary>
        /// Returns explicit width if set. Else returns the measured width
        /// </summary>
        /// <returns></returns>
        public float GetExplicitOrMeasuredWidth()
        {
            if (null != ExplicitWidth)
                return (float)ExplicitWidth;

            return MeasuredWidth;
        }

        /// <summary>
        /// Returns explicit height if set. Else returns the measured height<br/>
        /// </summary>
        /// <returns></returns>
        public float GetExplicitOrMeasuredHeight()
        {
            if (null != ExplicitHeight)
                return (float)ExplicitHeight;

            return MeasuredHeight;
        }

        /// <summary>
        /// Validates layout
        /// A lifecycle method
        /// Top-down
        /// </summary>
        public virtual void ValidateDisplayList()
        {
            if (!DisplayListInvalidated)
                return;

#if DEBUG
            if (DebugMode)
                Debug.Log(this + " -> ValidateDisplayList()");
#endif

            // NOTE: NONO! the following line should probably be valid for popups!!! (it messed with container scrollbars) 20130809
            // TODO!!!
            //SetActualSize(GetExplicitOrMeasuredWidth(), GetExplicitOrMeasuredHeight());

            UpdateDisplayList(Width, Height);

            DisplayListInvalidated = false;
        }

        #region Implementation of IUpdating

        /// <summary>
        /// Commits properties
        /// </summary>
        protected virtual void CommitProperties()
        {
            if (X != _oldX || X != _oldY)
            {
                InvalidateTransform();
                DispatchMoveEvent();
            }

            if (_width != _oldWidth || _height != _oldHeight)
            {
                InvalidateTransform();
                DispatchResizeEvent();
            }
        }

        protected virtual void Measure()
        {
            MeasuredWidth = 0;
            MeasuredHeight = 0;
            MeasuredMinWidth = 0;
            MeasuredMinHeight = 0;
        }

        /// <summary>
        /// A lifecycle method
        /// Updates layout
        /// Top to bottom
        /// </summary>
        protected virtual void UpdateDisplayList(float width, float height)
        {
            
        }

        #endregion

        #region Move

        /// <summary>
        /// Moves the component to a position specified with (x, y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(float x, float y)
        {
            bool changed = false;

            if (x != X)
            {
                base.X = x;

                if (HasEventListener(FrameworkEvent.X_CHANGED/*"xChanged"*/))
                    DispatchEvent(new Event(FrameworkEvent.X_CHANGED));
                changed = true;
            }

            if (y != Y)
            {
                base.Y = y;

                if (HasEventListener(FrameworkEvent.Y_CHANGED/*"yChanged"*/))
                    DispatchEvent(new Event(FrameworkEvent.Y_CHANGED));
                changed = true;
            }

            if (changed)
            {
                //if (Id == "btn")
                //    Debug.Log("     -> Changed: " + X + ", " + Y);
                
                InvalidateTransform();
                DispatchMoveEvent();
            }
        }

        /// <summary>
        /// Moves the component to a specified position
        /// </summary>
        /// <param name="position"></param>
        public void Move(Point position)
        {
            Move(position.X, position.Y);
        }

        #endregion

        /**
         *  
         */
        internal void UpdateCallbacks()
        {
            //Debug.Log("### UpdateCallbacks: " + this);
            if (_propertiesInvalidated)
            {
                //Debug.Log("  # _propertiesInvalidated: " + this);
                InvalidationManager.Instance.InvalidateProperties(this);
            }

            if (_transformInvalidated)
            {
                //Debug.Log("  # _transformInvalidated: " + this);
                InvalidationManager.Instance.InvalidateTransform(this);
            }

            if (_sizeInvalidated)
            {
                //Debug.Log("  # _sizeInvalidated: " + this);
                InvalidationManager.Instance.InvalidateSize(this);
            }

            if (DisplayListInvalidated)
            {
                //Debug.Log("  # DisplayListInvalidated: " + this);
                InvalidationManager.Instance.InvalidateDisplayList(this);
            }
        }

        #region ILayoutElement

        private bool _includeInLayout = true;
        
        /// <summary>
        /// Is component processed in LayoutChildren()?
        /// </summary>
        public bool IncludeInLayout
        {
            get { return _includeInLayout; }
            set
            {
                if (value != _includeInLayout)
                {
                    _includeInLayout = value;
                    InvalidateParentSizeAndDisplayList();
                }
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="x"></param>
        ///<param name="y"></param>
        public void SetLayoutBoundsPosition(float x, float y)
        {
            Move(x, y);
        }

        ///<summary>
        ///</summary>
        ///<param name="width"></param>
        ///<param name="height"></param>
        public void SetLayoutBoundsSize(float? width, float? height)
        {
            //SetActualSize(width ?? 0, height ?? 0); // This was surely a bug (20130824)
            LayoutUtil.SetLayoutBoundsSize(this, width, height);
        }

        #endregion

        #region Plugins

        private List<IPlugin> _plugins;
        /// <summary>
        /// The list of plugins
        /// </summary>
        public List<IPlugin> Plugins
        {
            get
            {
                if (null == _plugins)
                    _plugins = new List<IPlugin>();

                return _plugins;
            }
            set
            {
                _plugins = value;
            }
        }

        /// <summary>
        /// Initializes plugins
        /// </summary>
        private void InitPlugins()
        {
            if (null == _plugins)
                return;

            //if (this is Panel)
            //    Debug.Log("InitPlugins: " + _plugins.Count);

            foreach (IPlugin plugin in _plugins)
            {
                plugin.Initialize(this);
            }
        }

        #endregion

        #region Event dispatching

        private void DispatchMoveEvent()
        {
            if (HasEventListener(MoveEvent.MOVE))
            {
                var moveEvent = new MoveEvent(MoveEvent.MOVE) { OldX = _oldX, OldY = _oldY };
                DispatchEvent(moveEvent);
            }

            _oldX = X;
            _oldY = Y;
        }

        /// <summary>
        /// Dispatches the resize event on target
        /// </summary>
        private void DispatchResizeEvent()
        {
            if (HasEventListener(ResizeEvent.RESIZE))
            {
                var resizeEvent = new ResizeEvent(ResizeEvent.RESIZE)
                                      {
                                          Size = new Point(Width, Height),
                                          OldWidth = _oldWidth,
                                          OldHeight = _oldHeight
                                      };
                DispatchEvent(resizeEvent);
            }

            _oldWidth = Width;
            _oldHeight = Height;
        }

        protected void DispatchPropertyChangeEvent(string prop, object oldValue, object value)
        {
            if (HasEventListener("propertyChange"))
                DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, prop, oldValue, value));
        }

        #endregion
    }
}