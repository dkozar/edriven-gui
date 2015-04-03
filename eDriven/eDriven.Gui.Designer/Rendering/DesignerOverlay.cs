#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Reflection;
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Core.Signals;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Managers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Designer.Rendering
{
    public class DesignerOverlay : MonoBehaviour, IDisposable
    {
#if DEBUG
    /// <summary>
    /// Debug mode
    /// </summary>
        public static bool DebugMode;
#endif

        #region Properties

        /// <summary>
        /// Disables the overlay in editor
        /// </summary>
        public static bool DisableOverlayOnInspector = true;

        /// <summary>
        /// The signal emmiting when component clicked in the game view
        /// </summary>
        public static Signal ClickSignal = new Signal();

        ///// <summary>
        ///// The signal emmiting when component double clicked in the game view
        ///// </summary>
        //public static Signal DoubleClickSignal = new Signal();

        #endregion

        #region Singleton

        private static DesignerOverlay _instance;
        /// <summary>
        /// The instance of DesignerOverlay
        /// </summary>
        public static DesignerOverlay Instance
        {
            get
            {
                if (_instance == null)
                {
                    UnityEngine.Object[] retValue = FindObjectsOfType(typeof(DesignerOverlay));
                    if (retValue == null || retValue.Length == 0)
                        return null;
                    if (retValue.Length > 1)
                        throw new ApplicationException("More than one DesignerOverlay object exists on the scene!");

                    _instance = ((DesignerOverlay)retValue[0]);
                    _instance.InitStage();
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor

        protected DesignerOverlay() // constructor is protected
        {
            //_inputSlot = new RenderSlot(this);
            //_keyDownSlot = new KeyDownSlot(this);
            //_keyUpSlot = new KeyUpSlot(this);
        }

        #endregion

        #region Members

        private DisplayListMember _hoveredComponent;
        private DisplayListMember _selectedComponent;
        
        private Rectangle _previousHoveredBounds = new Rectangle();
        private Rectangle _previousSelectedBounds = new Rectangle();
        
        #endregion

        #region Methods

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void Start()
            // ReSharper restore UnusedMember.Local
        {
            InitStage();
        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay enabled");
#endif
            TurnOn();
            SystemManager.Instance.DisposingSignal.Connect(DisposingSlot, true);
        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnDisable()
// ReSharper restore UnusedMember.Local
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay disabled");
#endif
            TurnOff();

            _instance = null;
            SystemManager.Instance.DisposingSignal.Disconnect(DisposingSlot);
        }

        private void DisposingSlot(object[] parameters)
        {
            _instance = null;
            TurnOff();
        }

        private void TurnOn()
        {
            HandleEventListeners();

            MouseEventDispatcher.PlayModeInspect = true; // !DesignerMode;

            if (null != _selectedComponent && IsInspectable(_selectedComponent))
                Select(_selectedComponent);
        }

        private void TurnOff()
        {
            HandleEventListeners();

            MouseEventDispatcher.PlayModeInspect = false;

            Unhover();

            //SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
        }

        /// <summary>
        /// Returns true if the component is inspectable
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        internal static bool IsInspectable(DisplayListMember component)
        {
            return component.Stage != DesignerOverlayStage.Instance || !DisableOverlayOnInspector;
        }

        private DesignerOverlayStage _stage;

        private void InitStage()
        {
            //if (null != _stage || null == Font)
            //    return; // init only once
            //Debug.Log("Initializing Stage");
            _stage = DesignerOverlayStage.Instance;
        }

        #endregion

        #region Mouse

        private void HandleEventListeners()
        {
            if (enabled)
            {
                //MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_OVER, MouseOverSlot);
                //MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_OUT, MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseOverSignal.Connect(MouseOverSlot);
                MouseEventDispatcher.Instance.InspectorMouseOutSignal.Connect(MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseLeaveSignal.Connect(MouseLeaveSlot);

                // Capture phase. Disabling the actual click inside of the handler.
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.DOUBLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.RIGHT_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.RIGHT_DOUBLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MIDDLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);

                SystemManager.Instance.RenderSignal.Connect(RenderSlot); // get update signals for checking GUI state
                //SystemManager.Instance.KeyDownSignal.Connect(KeyDownSlot);
                //SystemManager.Instance.KeyUpSignal.Connect(KeyUpSlot);
            }
            else
            {
                //MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OVER, MouseOverSlot);
                //MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OUT, MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseOverSignal.Disconnect(MouseOverSlot);
                MouseEventDispatcher.Instance.InspectorMouseOutSignal.Disconnect(MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseLeaveSignal.Disconnect(MouseLeaveSlot);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.DOUBLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.RIGHT_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.RIGHT_DOUBLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MIDDLE_CLICK, SpecialClickHandler, EventPhase.Capture | EventPhase.Target);

                SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
                //SystemManager.Instance.KeyDownSignal.Disconnect(KeyDownSlot);
                //SystemManager.Instance.KeyUpSignal.Disconnect(KeyUpSlot);
            }
        }

        /// <summary>
        /// Fires on mouse-overed component change<br/>
        /// Since in editor overlay we are dealing with adapters, we need to examine if the mouse-overed component is related to any adapter<br/>
        /// We do that by looking for the adapter via GuiLookup, which finds the adapter in component parent<br/>
        /// We then examine if the current selection changed. If it did, we are calling the Select method
        /// </summary>
        private void MouseOverSlot(params object[] parameters)
        {
            Component comp = (Component)parameters[0];
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay->MouseOverSlot: " + comp);
#endif
            if (null == comp)
                return;

            GameObject go = GuiLookup.GetGameObject(comp);
            //Debug.Log("     go: " + go);

            if (null == go)
                return;

            ComponentAdapter adapter = (ComponentAdapter) go.GetComponent(typeof (ComponentAdapter));
            if (null == adapter)
                return;

            //Debug.Log("     adapter: " + adapter);

            if (null == adapter.Component)
                return;

            //if (_hoveredComponent == adapter.Component) // commented out 20130216 because of the stage leave bug
            //    return;

            _hoveredComponent = adapter.Component;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Hover: " + _hoveredComponent);
            }
#endif
            Hover(_hoveredComponent);
        }

        //internal void MouseOutSlot(Event e)
        internal void MouseOutSlot(params object[] parameters)
        {
            Component target = (Component)parameters[0];
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay->MouseOutSlot: " + target);
#endif
            if (target is Stage)
                Unhover();
        }

        //internal void MouseOutSlot(Event e)
        internal void MouseLeaveSlot(params object[] parameters)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay->MouseLeaveSlot");
#endif
            Unhover();
        }

        internal void ClickHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay->ClickHandler: " + e.Target);
#endif
            MouseEvent me = (MouseEvent)e;
            if (me.CurrentEvent.control || me.CurrentEvent.shift)
            {
                //Debug.Log("Click");
                e.CancelAndStopPropagation(); // disable the actual button click in the capture phase. Sweet ^_^
            }
            //_stack.Clear();

            GameObject go = GuiLookup.GetGameObject((Component) e.Target);

            if (null == go)
            {
                _hoveredComponent = null;
                ClickSignal.Emit();
                return;
            }
            
            ComponentAdapter adapter = (ComponentAdapter)go.GetComponent(typeof(ComponentAdapter));
            if (null == adapter)
            {
                _hoveredComponent = null;
                ClickSignal.Emit();
                return;
            }
            
            if (null == adapter.Component)
            {
                _hoveredComponent = null;
                ClickSignal.Emit();
                return;
            }
            
            _hoveredComponent = adapter.Component;

            ClickSignal.Emit(_hoveredComponent);
            //Debug.Log("ClickSignal.NumSlots: " + ClickSignal.NumSlots);
        }

        internal void SpecialClickHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DesignerOverlay->SpecialClickHandler: " + e.Target);
#endif
            MouseEvent me = (MouseEvent)e;
            if (me.CurrentEvent.control || me.CurrentEvent.shift)
            {
                //Debug.Log("Click");
                e.CancelAndStopPropagation(); // disable the actual button click in the capture phase. Sweet ^_^
            }
            //_stack.Clear();

            GameObject go = GuiLookup.GetGameObject((Component) e.Target);

            if (null == go)
            {
                _hoveredComponent = null;
                ClickSignal.Emit(null, e);
                return;
            }
            
            ComponentAdapter adapter = (ComponentAdapter)go.GetComponent(typeof(ComponentAdapter));
            if (null == adapter)
            {
                _hoveredComponent = null;
                ClickSignal.Emit(null, e);
                return;
            }
            
            if (null == adapter.Component)
            {
                _hoveredComponent = null;
                ClickSignal.Emit(null, e);
                return;
            }
            
            _hoveredComponent = adapter.Component;

            ClickSignal.Emit(_hoveredComponent, e);
            //Debug.Log("DoubleClickSignal.NumSlots: " + DoubleClickSignal.NumSlots);
        }

        #endregion

        #region Render

        /// <summary>
        /// Render slot
        /// </summary>
        /// <param name="parameters"></param>
        public void RenderSlot(params object[] parameters)
        {
            // Checking if the current component moved and/or resized
            // NOTE: Using brute force checking here (instead of listening to move and resize events)
            // because the component may choose not to dispatch any events
            // However - no harm done - the DesignerOverlay should be off in the production
            if (null == _stage)
                return;

            if (null != _hoveredComponent && !_previousHoveredBounds.Equals(_hoveredComponent.Transform.GlobalBounds) && IsInspectable(_hoveredComponent))
            {
                _stage.Hover(_hoveredComponent);
                _previousHoveredBounds = _hoveredComponent.Transform.GlobalBounds;
            }

            if (null != _selectedComponent && !_previousSelectedBounds.Equals(_selectedComponent.Transform.GlobalBounds) && IsInspectable(_selectedComponent))
            {
                _stage.Select(_selectedComponent);
                _previousSelectedBounds = _selectedComponent.Transform.GlobalBounds;
            }
        }

        #endregion

        #region Overlay

        public void Hover(DisplayListMember component)
        {
            _hoveredComponent = component;
            //Debug.Log("Hover: " + component + "; Stage:" + _stage);
            if (null != _stage)
                _stage.Hover(component);
        }

        public void Unhover()
        {
            if (null != _stage)
                _stage.Unhover();
        }

        /// <summary>
        /// Selects the component from the outside
        /// </summary>
        /// <param name="component"></param>
        public void Select(DisplayListMember component)
        {
            //Debug.Log("Select: " + component);
            _selectedComponent = component;
            if (null != _stage)
                _stage.Select(component);
        }

        public void Deselect()
        {
            if (null != _stage)
                _stage.Deselect();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            TurnOff();
        }

        #endregion

        /// <summary>
        /// Attaches the designer overlay component to eDriven framework object
        /// </summary>
        public static void Attach()
        {
// ReSharper disable once UnusedVariable
            DesignerOverlay overlay = (DesignerOverlay)Framework.GetComponent<DesignerOverlay>(true); // add if non-existing
        }
    }
}