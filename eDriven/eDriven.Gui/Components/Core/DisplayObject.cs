using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Signals;
using eDriven.Gui.Managers;
using eDriven.Gui.Reflection;
using UnityEngine;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// The object that can be drawn on screen<br/>
    /// It can be positioned on screen (using X and Y), sized (using Width and Height) and rendered (using the Draw() method)<br/>
    /// </summary>
    [Event(Name = FrameworkEvent.SHOW, Type = typeof(FrameworkEvent))]
    [Event(Name = FrameworkEvent.HIDE, Type = typeof(FrameworkEvent))]
    [Event(Name = FrameworkEvent.FIRST_SHOW, Type = typeof(FrameworkEvent))]
    
    public abstract class DisplayObject : EventDispatcher, IDrawable, /*ISkinnable, */IDataHost
    {
        #region Signals

        private Signal _preRenderSignal;
        /// <summary>
        /// The signal that emits before before render
        /// </summary>
        public Signal PreRenderSignal
        {
            get
            {
                if (null == _preRenderSignal)
                    _preRenderSignal = new Signal();
                return _preRenderSignal;
            }
            set
            {
                _preRenderSignal = value;
            }
        }

        private Signal _renderSignal;
        /// <summary>
        /// The signal that emits before render
        /// </summary>
        public Signal RenderSignal
        {
            get
            {
                if (null == _renderSignal)
                    _renderSignal = new Signal();
                return _renderSignal;
            }
            set
            {
                _renderSignal = value;
            }
        }

        private Signal _postRenderSignal;
        /// <summary>
        /// The signal that emits after render
        /// </summary>
        public Signal PostRenderSignal
        {
            get
            {
                if (null == _postRenderSignal)
                    _postRenderSignal = new Signal();
                return _postRenderSignal;
            }
            set
            {
                _postRenderSignal = value;
            }
        }

        private Signal _overlayRenderSignal;
        ///<summary>
        /// The signal that emits just before the overlay render
        ///</summary>
        public Signal OverlayRenderSignal
        {
            get
            {
                if (null == _overlayRenderSignal)
                    _overlayRenderSignal = new Signal();
                return _overlayRenderSignal;
            }
            set
            {
                _overlayRenderSignal = value;
            }
        }

        private Signal _focusRenderSignal;
        /// <summary>
        /// The signal that emits just before the focus render
        /// </summary>
        public Signal FocusRenderSignal
        {
            get
            {
                if (null == _focusRenderSignal)
                    _focusRenderSignal = new Signal();
                return _focusRenderSignal;
            }
            set
            {
                _focusRenderSignal = value;
            }
        }

        private Signal _renderDoneSignal;
        /// <summary>
        /// The signal that emits after the render is complete
        /// </summary>
        public Signal RenderDoneSignal
        {
            get
            {
                if (null == _renderDoneSignal)
                    _renderDoneSignal = new Signal();
                return _renderDoneSignal;
            }
            set
            {
                _renderDoneSignal = value;
            }
        }

        #endregion

        #region Properties

        #region Styles

        private GUIStyle _activeStyle;
        /// <summary>
        /// The default style that component uses for measuring and rendering
        /// This is done in the InitializeStyle() override
        /// </summary>
        internal virtual GUIStyle ActiveStyle
        {
            get { return _activeStyle; }
            set
            {
                _activeStyle = value;

                if (null != value)
                    ActiveStyleName = value.ToString();
            }
        }

        /// <summary>
        /// Used by GUI inspector exclusively
        /// </summary>
        internal string ActiveStyleName { get; private set; }

        #endregion

        #region Visibility

        private bool _visible;

        /// <summary>
        /// Is component Visible
        /// If not, isn't processed by draw routine
        /// </summary>
        public virtual bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Note: never make this property virtual and override it
        /// This property reflects the true nature of component visibility
        /// This is important because the components are being hidden during the instantiation, and shown on creation complete
        /// This is the way to find out the desired visibility state (some of them might want to be hidden)
        /// </summary>
// ReSharper disable InconsistentNaming
        public bool QVisible
// ReSharper restore InconsistentNaming
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        #endregion

        #region Alpha

        private float _alpha = 1;
        /// <summary>
        /// Alpha
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        #endregion

        /// <summary>
        /// Tooltip
        /// </summary>
        public virtual string Tooltip { get; set; }

        public bool AutoDisposeData;

        protected DisplayObject()
        {
            InitTransform();
        }

        protected virtual void InitTransform()
        {
            _transform = new DisplayObjectTransform((DisplayListMember)this);
        }

        private GuiTransformBase _transform;
        public GuiTransformBase Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        /// <summary>
        /// A lifecycle method
        /// Invalidates position
        /// </summary>
        public virtual void InvalidateTransform()
        {
            Transform.Invalidate();
        }

        #region Bounds

        //private Rectangle _bounds = new Rectangle();
        /// <summary>
        /// Bounds relative to Owner (bakes rect)
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                return _transform.Bounds;
            }
        }

        /// <summary>
        /// Works with global coordinates (*RARE*)<br/>
        /// Returns true if bounds of this display object (or it's children if recursive) contains point<br/>
        /// This could be overriden and for instance checked against circle bounds, or by GlobalBounds but slightly expanded (for mobile)
        /// e.g. return GlobalBounds.Expand(20).Contains(point);
        /// </summary>
        /// <param name="point"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual bool ContainsPoint(Point point, bool recursive)
        {
            return _transform.ContainsPoint(point);
        }

        #endregion

        #region Position

        /// <summary>
        /// X coordinate
        /// </summary>
        public virtual float X { get; set; }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public virtual float Y { get; set; }

        #region Position

        /// <summary>
        /// Position
        /// </summary>
// ReSharper disable UnusedMember.Global
        public Point Position
// ReSharper restore UnusedMember.Global
        {
            get
            {
                return _transform.Position;
            }
        }

        /// <summary>
        /// The rendering Rect
        /// This is the optimization technique: since we are using Rectangle class for internal calculations, and Unity uses Rects to render components, 
        /// To avoid constant conversions from Rectangle to Rect, we are converting only when needed (when coordinates or size change)
        /// We update this variable on change
        /// </summary>
        public Rect RenderingRect { get; internal set; }

        /// <summary>
        /// The rendering Rect
        /// This is the optimization technique: since we are using Rectangle class for internal calculations, and Unity uses Rects to render components, 
        /// To avoid constant conversions from Rectangle to Rect, we are converting only when needed (when coordinates or size change)
        /// We update this variable on change
        /// </summary>
        public Rect LocalRenderingRect { get; internal set; }

        #endregion

        #endregion

        #region Size

        /// <summary>
        /// The Width of the component
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// The Height of the component
        /// </summary>
        public virtual float Height { get; set; }

        #endregion

        #region Event Handlers

        private MulticastDelegate _showHandler;
        /// <summary>
        /// The event that fires when the component is shown
        ///</summary>
        [Event(Name = FrameworkEvent.SHOW, Type = typeof(FrameworkEvent))]
        public MulticastDelegate ShowHandler
        {
            get
            {
                if (null == _showHandler)
                    _showHandler = new MulticastDelegate(this, FrameworkEvent.SHOW);
                return _showHandler;
            }
            set
            {
                _showHandler = value;
            }
        }

        private MulticastDelegate _hideHandler;
        /// <summary>
        /// The event that fires when the component is hidden
        ///</summary>
        [Event(Name = FrameworkEvent.HIDE, Type = typeof(FrameworkEvent))]
        public MulticastDelegate HideHandler
        {
            get
            {
                if (null == _hideHandler)
                    _hideHandler = new MulticastDelegate(this, FrameworkEvent.HIDE);
                return _hideHandler;
            }
            set
            {
                _hideHandler = value;
            }
        }

        private MulticastDelegate _firstShowHandler;
        /// <summary>
        /// The event that fires when the component is shown for the first time
        ///</summary>
        [Event(Name = FrameworkEvent.FIRST_SHOW, Type = typeof(FrameworkEvent))]
        public MulticastDelegate FirstShow
        {
            get
            {
                if (null == _firstShowHandler)
                    _firstShowHandler = new MulticastDelegate(this, FrameworkEvent.FIRST_SHOW);
                return _firstShowHandler;
            }
            set
            {
                _firstShowHandler = value;
            }
        }
        
        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            ShowHandler.Dispose();
            HideHandler.Dispose();
            FirstShow.Dispose();

            if (AutoDisposeData)
            {
                var d = Data as IDisposable;
                if (null != d)
                    d.Dispose();
            }

            Data = null;
        }

        #endregion

        #region Implementation of IDataHost

        private object _data;
        /// <summary>
        /// The arbitrary data attached to this piece of GUI
        /// </summary>
// ReSharper disable ConvertToAutoProperty
        public virtual object Data
// ReSharper restore ConvertToAutoProperty
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #endregion

        #region Members

        /// <summary>
        /// GUIContent
        /// This is the content used by Unity when rendering pieces of GUI
        /// GUIContent consists of icon and label
        /// </summary>
        protected GUIContent Content { get; set; }

        #endregion

        #region Implementation of IDrawable

        // TODO: Remove PreRender and PostRender from Draw because of the proper Alpha propagation
        virtual public void Draw()
        {
            // defered calls
            if (_hasDeferredCalls)
            {
                ProcessDeferredCalls();
            }

            if (_skipRender)
            {
                DecreaseSkipFrameCount();
                return;
            }

            RotationStart();

            ScaleStart();

            ColorStart();

            //if (_shouldFirePreRender)
            //    DispatchEvent(new Event(PRE_RENDER), true);
            if (null != _preRenderSignal)
                _preRenderSignal.Emit(this);

            PreRender();

            //if (_shouldFireRender)
            //    DispatchEvent(new Event(RENDER), true); 
            
            if (null != _renderSignal)
                _renderSignal.Emit(this);

            Render();

            if (null != _postRenderSignal)
                _postRenderSignal.Emit(this);
            
            PostRender();
            if (null != _overlayRenderSignal)
                _overlayRenderSignal.Emit(this);

            // NOTE: focus cannot be run from PostRender
            // That is because InteractiveComponent is superclass to Container (which introduces scrollview)
            // Focus rect has to be drawn last, after the scrollbars
            OverlayRender();

            ColorEnd();

            if (null != _focusRenderSignal)
                _focusRenderSignal.Emit(this);

            FocusRender();

            if (null != _renderDoneSignal)
                _renderDoneSignal.Emit(this);

            ScaleEnd();

            RotationEnd();
        }

        #region Rotation

        /// <summary>
        /// Rotation
        /// The pivot point is the center of the component
        /// </summary>
        public float Rotation;

        private Vector2 _rotationPivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Rotation pivot
        /// </summary>
        public Vector2 RotationPivot
        {
            get
            {
                return _rotationPivot;
            }
            set
            {
                _rotationPivot = new Vector2(
                    Mathf.Clamp(value.x, 0f, 1f),
                    Mathf.Clamp(value.y, 0f, 1f)
                );
            }
        }

        private Matrix4x4 _rotationMatrixBackup;

        protected virtual void RotationStart()
        {
            if (0 == Rotation || 0 == Rotation % 360)
                return;

            _rotationMatrixBackup = GUI.matrix;
            //Debug.Log(RenderingRect.CenterAsVector2);
            GUIUtility.RotateAroundPivot(Rotation, 
                new Vector2(
                    RenderingRect.x + RenderingRect.width * _rotationPivot.x,
                    RenderingRect.y + RenderingRect.height * _rotationPivot.y
                )
            );
        }

        protected virtual void RotationEnd()
        {
            if (0 == Rotation || 0 == Rotation % 360)
                return;
            
            GUI.matrix = _rotationMatrixBackup;
        }

        #endregion

        #region Scale

        /// <summary>
        /// Scale
        /// The pivot point is the center of the component
        /// </summary>
        public Vector2 Scale = Vector2.one;

        private Matrix4x4 _scaleMatrixBackup;

        /// <summary>
        /// A minimal X scale to fight the "Ignoring invalid matrix assinged to GUI.matrix - the matrix needs to be invertible. Did you scale by 0 on Z-axis" bug
        /// </summary>
        public static float MinScaleX = 0.0001f;

        /// <summary>
        /// A minimal Y scale to fight the "Ignoring invalid matrix assinged to GUI.matrix - the matrix needs to be invertible. Did you scale by 0 on Z-axis" bug
        /// </summary>
        public static float MinScaleY = 0.0001f;

        protected virtual void ScaleStart()
        {
            if (Vector2.one == Scale)
                return;

            //Debug.Log("GUI.matrix: " + GUI.matrix);
            _scaleMatrixBackup = GUI.matrix;
            
            // Note: ScaleAroundPivot sometimes introduces a bug:
            // "Ignoring invalid matrix assinged to GUI.matrix - the matrix needs to be invertible. Did you scale by 0 on Z-axis"
            // I found out that this happensif Scale x or y is 0
            // this is the fix:
            if (Scale.x == 0)
                Scale.x = MinScaleX;
            if (Scale.y == 0)
                Scale.y = MinScaleY;

            GUIUtility.ScaleAroundPivot(Scale, new Vector2(RenderingRect.x + RenderingRect.width / 2, RenderingRect.y + RenderingRect.height / 2)); //Bounds.CenterAsVector2);
        }

        protected virtual void ScaleEnd()
        {
            if (Vector2.one == Scale)
                return;

            GUI.matrix = _scaleMatrixBackup;
        }

        #endregion

        protected virtual void ColorStart() {}
        protected virtual void ColorEnd() { }

        /// <summary>
        /// Pre-draw logic
        /// Switching on Alpha, scrollbars and clipping
        /// </summary>
        virtual protected void PreRender()
        {

        }

        /// <summary>
        /// Actual draw logic
        /// </summary>
        virtual protected void Render()
        {
            //if (this is Button)
            //{
            //    //if (((Button)this).Id == "btn")
            //    Debug.Log("* btn: " + RenderingRect);
            //}
            //Debug.Log("* Render: " + RenderingRect);
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (null == ActiveStyle)
                    return; // throw new Exception(string.Format("ActiveStyle is null on component [{0}]", this));

                //Debug.Log("* Render: " + RenderingRect);
                //if (this is Button)
                //{
                //    //if (((Button)this).Id == "btn")
                //        Debug.Log("* btn: " + RenderingRect);
                //}

                if (DragManager.IsDragging)

                    ActiveStyle.Draw(RenderingRect, Content,
                       false,
                       false,
                       false,
                       false);

                else

                    /**
                     * 20131210
                     * Zna se pojavit sljedeći warning:
                     * "GUIContent is null. Use GUIContent.none.
                     * UnityEngine.GUIStyle:Draw(Rect, GUIContent, Boolean, Boolean, Boolean, Boolean)
                     * eDriven.Gui.Components.DisplayObject:Render() (at c:/PROJECTS/Unity/eDriven/eDriven.Gui/Components/Core/DisplayObject.cs:640)"
                     * (tako da sam umjesto "Content" ubacio "Content ?? GUIContent.none"
                     * */

                    ActiveStyle.Draw(RenderingRect, Content ?? GUIContent.none,
                       this == MouseEventDispatcher.MouseTarget,
                       this == MouseEventDispatcher.MouseDownComponent,
                       false, // this should be overriden in components that have a selected state (Button)
                       this == FocusManager.Instance.FocusedComponent);
            }
        }

        /// <summary>
        /// Post-draw logic
        /// Switching off Alpha, scrollbars and clipping
        /// </summary>
        virtual protected void PostRender()
        {
            
        }

        /// <summary>
        /// Post-draw logic
        /// Renders component overlay layer
        /// </summary>
        virtual protected void OverlayRender()
        {
            
        }

        /// <summary>
        /// Post-draw logic
        /// Renders focus rectangle
        /// </summary>
        virtual protected void FocusRender()
        {
            
        }

        #endregion

        #region Skip render

        private int _skipFrameCount;
        private bool _skipRender;
        /// <summary>
        /// This is the approach of skipping the rendering process for N frames
        /// Needed by effects which start tweening one frame too late
        /// TODO: Investigate!
        /// </summary>
        /// <param name="numberOfFrames"></param>
        public void SkipRender(int numberOfFrames)
        {
            if (numberOfFrames > 0)
            {
                _skipFrameCount = numberOfFrames;
                _skipRender = true;
            }
        }

        protected void DecreaseSkipFrameCount()
        {
            _skipFrameCount--;
            if (_skipFrameCount <= 0)
                _skipRender = false;
        }

        #endregion

        #region Deffered calls

        /// <summary>
        /// The signature of a method that should be called within a delay of N frames
        /// </summary>
        /// <param name="args"></param>
        public delegate void DeferedCall(params object[] args);

        private class DeferedCallDescriptor
        {
            internal readonly DeferedCall Callback;
            internal int NumberOfFrames;
            internal readonly object[] Args;

            internal DeferedCallDescriptor(DeferedCall callback, int numberOfFrames, params object[] args)
            {
                Callback = callback;
                NumberOfFrames = numberOfFrames;
                Args = args;
            }
        }

        private List<DeferedCallDescriptor> _deferredCalls;

        private bool _hasDeferredCalls;

        /// <summary>
        /// Defers the function call for a given number of frames
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="numberOfFrames"></param>
        /// <param name="args">The arguments that should be applied to a defered function</param>
        public void Defer(DeferedCall callback, int numberOfFrames, params object[] args)
        {
            if (numberOfFrames <= 0)
            {
                callback();
                return;
            }

            if (null == _deferredCalls) // go lazy
                _deferredCalls = new List<DeferedCallDescriptor>();

            _deferredCalls.Add(new DeferedCallDescriptor(callback, numberOfFrames, args));

            _hasDeferredCalls = true;
        }

        private List<DeferedCallDescriptor> _toRemove;
        private void ProcessDeferredCalls()
        {
            if (null == _toRemove)
                _toRemove = new List<DeferedCallDescriptor>();

            foreach (DeferedCallDescriptor callback in _deferredCalls)
            {
                if (callback.NumberOfFrames <= 0)
                {
                    callback.Callback(callback.Args); // execute
                    _toRemove.Add(callback);
                }
                else
                {
                    callback.NumberOfFrames--;
                }
            }

            _toRemove.ForEach(delegate(DeferedCallDescriptor callback)
                                  {
                                      _deferredCalls.Remove(callback);
                                  });

            _toRemove.Clear();
            
            if (_deferredCalls.Count <= 0)
                _hasDeferredCalls = false;
        }

        #endregion

        //#region Implementation of ISkinnable

        //private bool _renderSkin; // does a Draw method contain line 'GUI.Skin = Skin;'

        //private bool _skinChanged;
        //private GUISkin _skin;

        ///// <summary>
        ///// Sets skin to this component<br/>
        ///// and also its children in the propagation phase,<br/>
        ///// but only if skin is not explicitely defined on a child
        ///// </summary>
        //public virtual GUISkin Skin
        //{
        //    get
        //    {
        //        return _skin;
        //    }
        //    set
        //    {
        //        if (value != _skin)
        //        {
        //            _skin = value;

        //            // Note: Render skin flag IS set here
        //            _renderSkin = null != _skin;
        //        }
        //    }
        //}

        //#endregion

        #region Initialization

        /// <summary>
        /// Initializes the component<br/>
        /// Called by the parent upon first addition<br/>
        /// Sets child's Owner and reference to Stage<br/>
        /// Sets Skin<br/>
        /// </summary>
        public virtual void Initialize()
        {
            /**
             * 1) Initialize Style
             * */
            InitializeStyle();

            /**
             * 2) Initialize content
             * */
            InitializeContent();
        }

        /// <summary>
        /// Initializes Style
        /// </summary>
        protected virtual void InitializeStyle()
        {
            //Debug.Log("*** InitializeStyle ***");
            //StyleDeclarationPopulator.PopulateStyleDeclarations(this, false); // do not overwrite existing
        }

        /// <summary>
        /// Initializes GUI content
        /// </summary>
        protected virtual void InitializeContent()
        {
            Content = new GUIContent();

            // implement in subclass
            // do not initialize content for non-base components
        }

        #endregion

        #region Optimized

        //private Signal _renderSignal;
        //public Signal PreRenderSignal
        //{
        //    get
        //    {
        //        if (null == _renderSignal)
        //            _renderSignal = new Signal();

        //        return _renderSignal;
        //    }
        //}

        //private int _preRenderListenerCount;
        //private bool _shouldFirePreRender;

        //private int _renderListenerCount;
        //private bool _shouldFireRender;

        //private int _postRenderListenerCount;
        //private bool _shouldFirePostRender;

        //private int _overlayRenderListenerCount;
        //private bool _shouldFireOverlayRender;

        //private int _focusRenderListenerCount;
        //private bool _shouldFireFocusRender;

        //private int _renderDoneListenerCount;
        //private bool _shouldFireRenderDone;

        //public override void AddEventListener(string eventType, EventHandler handler, EventPhase phases)
        //{
        //    if (MappedToAnyPhase(eventType, handler, phases))
        //        return;

        //    base.AddEventListener(eventType, handler, phases);

        //    switch (eventType)
        //    {
        //        case PRE_RENDER:
        //            _preRenderListenerCount++;
        //            break;
        //        case RENDER:
        //            _renderListenerCount++;
        //            break;
        //        case POST_RENDER:
        //            _postRenderListenerCount++;
        //            break;
        //        case OVERLAY_RENDER:
        //            _overlayRenderListenerCount++;
        //            break;
        //        case FOCUS_RENDER:
        //            _focusRenderListenerCount++;
        //            break;
        //        //case RENDER_DONE:
        //        //    _renderDoneListenerCount++;
        //        //    break;
        //    }

        //    HandleFlags();
        //}

        //public override void RemoveEventListener(string eventType, EventHandler handler, EventPhase phases)
        //{
        //    base.RemoveEventListener(eventType, handler, phases);

        //    switch (eventType)
        //    {
        //        case PRE_RENDER:
        //            if (_preRenderListenerCount > 0)
        //                _preRenderListenerCount--;
        //            break;
        //        case RENDER:
        //            if (_renderListenerCount > 0)
        //                _renderListenerCount--;
        //            break;
        //        case POST_RENDER:
        //            if (_postRenderListenerCount > 0)
        //                _postRenderListenerCount--;
        //            break;
        //        case OVERLAY_RENDER:
        //            if (_overlayRenderListenerCount > 0)
        //                _overlayRenderListenerCount--;
        //            break;
        //        case FOCUS_RENDER:
        //            if (_focusRenderListenerCount > 0)
        //                _focusRenderListenerCount--;
        //            break;
        //        //case RENDER_DONE:
        //        //    if (_renderDoneListenerCount > 0)
        //        //        _renderDoneListenerCount--;
        //        //    break;
        //    }

        //    HandleFlags();
        //}

        //private void HandleFlags()
        //{
        //    _shouldFirePreRender = _preRenderListenerCount > 0;
        //    _shouldFireRender = _renderListenerCount > 0;
        //    _shouldFirePostRender = _postRenderListenerCount > 0;
        //    _shouldFireOverlayRender = _overlayRenderListenerCount > 0;
        //    _shouldFireFocusRender = _focusRenderListenerCount > 0;
        //    //_shouldFireRenderDone = _renderDoneListenerCount > 0;
        //}

        #endregion

        //#region NestLevel

        //private int _hDepth = -1;
        ///// <summary>
        ///// NestLevel<br/>
        ///// Used for invalidation: this is a property of the DisplayObject class because DisplayObjects<br/>
        ///// could also have InvalidationManagerClients as children, so they must know how to pass the NestLevel to these children
        ///// </summary>
        //public virtual int NestLevel
        //{
        //    get
        //    {
        //        return _hDepth;
        //    }
        //    set
        //    {
        //        _hDepth = value;
        //    }
        //}

        //#endregion

    }
}