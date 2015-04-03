using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Core.Geom;
using eDriven.Gui.Managers;
using eDriven.Gui.Styles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;
using EventHandler = eDriven.Core.Events.EventHandler;
using LayoutBase=eDriven.Gui.Layout.LayoutBase;

namespace eDriven.Gui
{
    /// <summary>
    /// Gui class inherits UnityEngine.MonoBehaviour
    /// Normally it should be extended and put into the hierarchy manually
    /// Alternatively it could be created dinamically
    /// Upon starting or enabling, it registers itself to the StageManager instance
    /// The Depth property is used for layering the multiple GUIs in the application
    /// Internally it creates the Stage instance
    /// It listens for the changes in the inspector and propagates them to the Stage instance
    /// </summary>
    /// <remarks>Author: Danko Kozar</remarks>
    [Obfuscation(Exclude = true)]
    public class Gui : MonoBehaviour, IChildList, IEventDispatcher, IDisposable
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Properties

        ///// <summary>
        ///// Gui ID
        ///// </summary>
        //public string Id;

        /// <summary>
        /// GUI depth applied to this stage
        /// </summary>
        public int ZIndex;
        private int _zIndex;

        private Matrix4x4 _matrix;
        /// <summary>
        /// Matrix
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                return _matrix;
            }
            set
            {
                _matrix = value;
                if (null != Stage)
                    Stage.Matrix = _matrix;
            }
        }

        /// <summary>
        /// Stage ID
        /// </summary>
        public string Id;

        /// <summary>
        /// Does this stage receive focus
        /// </summary>
        public bool FocusEnabled;

        /// <summary>
        /// Does stage react on mouse events, or suppresses mouse event propagation to background (other stages or 3D)
        /// </summary>
        public bool MouseEnabled;
        private bool _mouseEnabled;

        ///// <summary>
        ///// Does stage scroll it's content
        ///// </summary>
        //public bool ClipContent = true; // default
        //private bool _clipContent = true; // default

        ///// <summary>
        ///// Does stage scroll it's content
        ///// </summary>
        //public ScrollPolicy HorizontalScrollPolicy = ScrollPolicy.Auto; // default
        //private ScrollPolicy _horizontalScrollPolicy = ScrollPolicy.Auto; // default

        ///// <summary>
        ///// Does stage scroll it's content
        ///// </summary>
        //public ScrollPolicy VerticalScrollPolicy = ScrollPolicy.Auto; // default
        //private ScrollPolicy _verticalScrollPolicy = ScrollPolicy.Auto; // default

        ///// <summary>
        ///// Does stage always show a horizontal scrollbar
        ///// </summary>
        //public bool AlwaysShowHorizontalScrollbar;
        //private bool _alwaysShowHorizontalScrollbar;

        ///// <summary>
        ///// Does stage always show a vertical scrollbar
        ///// </summary>
        //public bool AlwaysShowVerticalScrollbar;
        //private bool _alwaysShowVerticalScrollbar;

        /// <summary>
        /// Stage visibility
        /// </summary>
        public bool Visible = true;
        private bool _visible = true;

        private bool _initialized;
        /// <summary>
        /// Returns true if the Stage is initialized
        /// </summary>
        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        /// <summary>
        /// Exposes Stage instance to the script
        /// </summary>
        public Stage Stage { get; private set; }

        #endregion

        #region Unity Messages
        
        // ReSharper disable UnusedMember.Local
        void OnEnable()
        // ReSharper restore UnusedMember.Local
        {
            //Debug.Log("Gui started.");
            if (!enabled)
                return; // do not register this stage
#if DEBUG
            if (DebugMode)
                Debug.Log("Gui started.");
#endif
            Stage = new Stage // moved before OnStart() on 20120414
            {
                MouseEnabled = MouseEnabled,
                Enabled = enabled,
                ZIndex = ZIndex,
                Matrix = Matrix,
                Id = Id
            };

            // NOTE: calling OnStart() here
            OnStart();

            if (null != _layout)
                Stage.Layout = _layout;
            
            // TODO: use AddEventListener notation internally!
            Stage.AddEventListener(FrameworkEvent.PREINITIALIZE, PreinitializeHandler, EventPhase.Target);
            Stage.AddEventListener(FrameworkEvent.INITIALIZE, InitializeHandler, EventPhase.Target);
            Stage.AddEventListener(FrameworkEvent.CREATION_COMPLETE, CreationCompleteHandler, EventPhase.Target);

            // subscribe to when ready to have children :)
            Stage.AddEventListener(FrameworkEvent.PREINITIALIZE, delegate
            {
                //Debug.Log("*preinitialize");
                CreateChildren(); // run CreateChildren method from Gui
            }, EventPhase.Target);

            Stage.Register();

            // if in editor, monitor the GUI state  (this is an optimization) 
            if (Application.isEditor)
                SystemManager.Instance.UpdateSignal.Connect(InEditorUpdateSlot);
        }

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnDisable()
        // ReSharper restore UnusedMember.Local
        {
            if (null != Stage)
            {
                Stage.Enabled = false;

                Stage.Unregister();
            }

            if (Application.isEditor)
                SystemManager.Instance.UpdateSignal.Disconnect(InEditorUpdateSlot);
        }

        #endregion

        #region Handlers

        private void PreinitializeHandler(Event e)
        {
            OnPreinitialize();
            //Stage.RemoveEventListener(FrameworkEvent.PREINITIALIZE, PreinitializeHandler, EventPhase.Target);
        }

        /**
         * TODO: Bug: InitializeHandler fires multiple times!!! Investigate!
         * */
        private void InitializeHandler(Event e)
        {
            OnInitialize();
            //Stage.RemoveEventListener(FrameworkEvent.INITIALIZE, InitializeHandler, EventPhase.Target);
        }

        private void CreationCompleteHandler(Event e)
        {
            _initialized = true;

            //Debug.Log("Stage creation complete");
            OnCreationComplete();
            //Stage.RemoveEventListener(FrameworkEvent.CREATION_COMPLETE, CreationCompleteHandler, EventPhase.Target);
        }

        #endregion

        #region New methods

        /// <summary>
        /// This method should be used for creating children of the corresponding Stage
        /// </summary>
        protected virtual void CreateChildren()
        {

        }

        /// <summary>
        /// Run after the Stage is initialized, and before the layout is initialized
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// Handler that fires on Stage preinitialize
        /// </summary>
        protected virtual void OnPreinitialize()
        {
            //Debug.Log("OnPreinitialize ******");
        }

        /// <summary>
        /// Handler that fires on Stage initialize
        /// </summary>
        protected virtual void OnInitialize()
        {
            //Debug.Log("OnInitialize ******");
        }

        /// <summary>
        /// Handler that fires on Stage creation complete
        /// </summary>
        protected virtual void OnCreationComplete()
        {
            //Debug.Log("OnCreationComplete ******");
        }

        //protected virtual void OnUpdate()
        //{
        //    //Debug.Log("OnUpdate ******");
        //}

        /// <summary>
        /// Handler that fires on Stage (screen) resize
        /// </summary>
        /// <param name="size"></param>
        protected virtual void OnResize(Point size)
        {

        }

        #endregion

        #region IEventDispatcher

        /// <summary>
        /// Adds event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler (function)</param>
        public void AddEventListener(string eventType, EventHandler handler)
        {
            Stage.AddEventListener(eventType, handler);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="priority">Event priority</param>
        public void AddEventListener(string eventType, EventHandler handler, int priority)
        {
            Stage.AddEventListener(eventType, handler, priority);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="phases">Event phases</param>
        public void AddEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            Stage.AddEventListener(eventType, handler, phases);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="phases">Event bubbling phases that we listen to</param>
        /// <param name="priority">Event priority</param>
        public void AddEventListener(string eventType, EventHandler handler, EventPhase phases, int priority)
        {
            Stage.AddEventListener(eventType, handler, phases, priority);
        }

        /// <summary>
        /// Removes an event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler (function)</param>
        public void RemoveEventListener(string eventType, EventHandler handler)
        {
            Stage.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        /// Removes an event listener
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        /// <param name="phases"></param>
        public void RemoveEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            Stage.RemoveEventListener(eventType, handler, phases);
        }

        /// <summary>
        /// Removes all stage event listeners
        /// </summary>
        /// <param name="eventType"></param>
        public void RemoveAllListeners(string eventType)
        {
            Stage.RemoveAllListeners(eventType);
        }

        /// <summary>
        /// Checks whether an event listener of specified type is registered with stage
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool HasEventListener(string eventType)
        {
            return Stage.HasEventListener(eventType);
        }

        /// <summary>
        /// Checks whether an event listener of specified type is registered with stage
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool HasBubblingEventListener(string eventType)
        {
            return Stage.HasBubblingEventListener(eventType);
        }

        /// <summary>
        /// Dispatches an event with stage as target
        /// </summary>
        /// <param name="e"></param>
        public void DispatchEvent(Event e)
        {
            Stage.DispatchEvent(e);
        }

        /// <summary>
        /// Dispatches an event with stage as target
        /// </summary>
        /// <param name="e"></param>
        /// <param name="immediate"></param>
        public void DispatchEvent(Event e, bool immediate)
        {
            Stage.DispatchEvent(e, immediate);
        }

        #endregion

        #region IChildList

        /* NOTE: 20131026 Stage.X methods changed to Stage.ContentX methods, because of skinning (Container = SkinnableContainer). Bug (works with Group though) */

        /// <summary>
        /// The child components of the container
        /// </summary>
        public List<DisplayListMember> Children
        {
            get { return Stage.ContentChildren; }
        }

        /// <summary>
        /// Number of children
        /// </summary>
        public int NumberOfChildren
        {
            get { return Stage.NumberOfContentChildren; }
        }

        /// <summary>
        /// Returns true if the stage contains the specified child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool HasChild(DisplayListMember child)
        {
            return Stage.HasContentChild(child);
        }

        /// <summary>
        /// Returns true if the stage contains the specified child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool Contains(DisplayListMember child)
        {
            return Stage.ContentContains(child);
        }

        /// <summary>
        /// Returns true if the stage contains the specified child
        /// </summary>
        /// <param name="child"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        public bool Contains(DisplayListMember child, bool exclusive)
        {
            return Stage.ContentContains(child, exclusive);
        }

        /// <summary>
        /// Adds the child to a stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public DisplayListMember AddChild(DisplayListMember child)
        {
            return Stage.AddContentChild(child);
        }

        /// <summary>
        /// Adds a child to the container to the specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Index</param>
        public DisplayListMember AddChildAt(DisplayListMember child, int index)
        {
            return Stage.AddContentChildAt(child, index);
        }

        /// <summary>
        /// Removes a chold from the stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public DisplayListMember RemoveChild(DisplayListMember child)
        {
            return Stage.RemoveContentChild(child);
        }

        /// <summary>
        /// Adds a child from the container at specified index
        /// </summary>
        public DisplayListMember RemoveChildAt(int index)
        {
            return Stage.RemoveContentChildAt(index);
        }

        /// <summary>
        /// Removes all children from the stage
        /// </summary>
        public void RemoveAllChildren()
        {
            Stage.RemoveAllContentChildren();
        }

        ///<summary>
        /// Swaps two children of the stage
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        public void SwapChildren(DisplayListMember firstChild, DisplayListMember secondChild)
        {
            Stage.SwapContentChildren(firstChild, secondChild);
        }

        /// <summary>
        /// Gets stage child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        public DisplayListMember GetChildAt(int index)
        {
            return Stage.GetContentChildAt(index);
        }

        /// <summary>
        /// Gets stage child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        public int GetChildIndex(DisplayListMember child)
        {
            return Stage.GetContentChildIndex(child);
        }

        ///<summary>
        /// Sets stage child index
        ///</summary>
        ///<param name="child"></param>
        ///<param name="index"></param>
        public void SetChildIndex(DisplayListMember child, int index)
        {
            Stage.SetChildIndex(child, index);
        }

        #endregion

        #region ILayout
        
        private LayoutBase _layout; // NOTE: not set to anything
        /// <summary>
        /// Gets or sets the stage layout
        /// </summary>
        public LayoutBase Layout
        {
            get
            {
                if (null != Stage)
                    _layout = Stage.Layout;

                return _layout;
            }
            set
            {
                if (value == _layout)
                    return;

                _layout = value;

                // layout set directly on stage wins
                if (null != Stage)
                    Stage.Layout = _layout;
            }
        }

        #endregion

        #region Slots

        /// <summary>
        /// Receives the update signal when in editor
        /// </summary>
        /// <param name="parameters"></param>
        private void InEditorUpdateSlot(params object[] parameters)
        {
            if (_visible != Visible)
            {
                _visible = Visible;
                Stage.Visible = _visible;
            }

            if (_zIndex != ZIndex) // depth changed
            {
                if (ZIndex < 0)
                    ZIndex = 0; // Gui cannot set the negative depth, it has been reserved for system stages

                if (_zIndex != ZIndex)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Stage [{0}] changing depth from {1} to {2}", Stage, _zIndex, ZIndex));
#endif
                    _zIndex = ZIndex;
                    Stage.ZIndex = ZIndex;
                }
            }

            if (_mouseEnabled != MouseEnabled)
            {
                _mouseEnabled = MouseEnabled;
                Stage.MouseEnabled = _mouseEnabled;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            if (null != Stage) {
                Stage.Unregister();
                Stage.RemoveEventListener(FrameworkEvent.PREINITIALIZE, PreinitializeHandler, EventPhase.Target); // 20130813
                Stage.RemoveEventListener(FrameworkEvent.INITIALIZE, InitializeHandler, EventPhase.Target); // 20130813
                Stage.RemoveEventListener(FrameworkEvent.CREATION_COMPLETE, CreationCompleteHandler, EventPhase.Target); // 20130813
            }
        }

        #endregion

        #region Static helpers

        /// <summary>
        /// The reference to component under the mouse<br/>
        /// This component is looked upon after each mouse move on all stages
        /// </summary>
        public static Component MouseTarget
        {
            get { return MouseEventDispatcher.MouseTarget; }
        }

        /// <summary>
        /// The reference to a component stack that will receive mouse wheel events<br/>
        /// The stuck is built after each mouse move (contains components from multiple stages)
        /// </summary>
        public static List<Component> MouseWheelTargets
        {
            get { return MouseEventDispatcher.MouseWheelTargets; }
        }

        /// <summary>
        /// The reference to the mouse-downed component
        /// </summary>
        public static Component MouseDownComponent
        {
            get
            {
                return MouseEventDispatcher.MouseDownComponent;
            }
        }

        /// <summary>
        /// The reference to the focused component
        /// </summary>
        public static Component FocusedComponent
        {
            get
            {
                return (Component) FocusManager.Instance.FocusedComponent;
            }
        }

        #endregion

        #region Styles

        /// <summary>
        /// Called from the editor when we want the total reprocessing of styles
        /// </summary>
        public static void ProcessStyles()
        {
            //Debug.Log("ProcessStyles");
            StyleCacheDirty = true;
            StyleInitializer.Run();
        }

        #endregion

        /// <summary>
        /// Media queries are being run by default
        /// </summary>
        public static bool LiveMediaQueries = true; // TRUE!!!!

        /// <summary>
        /// Set this flag to true if in need to reset the declaration cache
        /// For instance, the editor should set this flag to true if any of style declarations added/removed/changed,
        /// just before reloading the style sheet collection (calling the Load() method)
        /// </summary>
        public static bool StyleCacheDirty;
    }
}