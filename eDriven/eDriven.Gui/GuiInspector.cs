using System;
using System.Reflection;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;
using eDriven.Gui.Stages;
using eDriven.Gui.Styles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui
{
    [AddComponentMenu("eDriven/Gui/GuiInspector")]

    public class GuiInspector : MonoBehaviour, IDisposable
    {
#if DEBUG
        /// <summary>
        /// Debug is on
        /// </summary>
        public static bool DebugMode;
#endif

        /// <summary>
        /// The key for toggling the overlay
        /// </summary>
        //public static KeyCode ToggleKey = KeyCode.Delete;

        public static bool DisableOverlayOnInspector = true;

        //public static Signal ClickSignal = new Signal();

        #region Singleton

        private static GuiInspector _instance;
        /// <summary>
        /// The instance of GuiInspector
        /// </summary>
        public static GuiInspector Instance
        {
            get
            {
                if (_instance == null)
                {
                    UnityEngine.Object[] retValue = FindObjectsOfType(typeof(GuiInspector));
                    if (retValue == null || retValue.Length == 0)
                        throw new ApplicationException("GuiInspector object doesn't exist on the scene!");
                    if (retValue.Length > 1)
                        throw new ApplicationException("More than one GuiInspector object exists on the scene!");

                    _instance = ((GuiInspector)retValue[0]);
                    _instance.InitStage();
                }
                return _instance;
            }
        }

        #endregion

        #region Slots

        /// <summary>
        /// Render slot
        /// </summary>
        /// <param name="parameters"></param>
        public void RenderSlot(params object[] parameters)
        {
            RenderHandler(parameters[0]);
        }

        /// <summary>
        /// Key down slot
        /// </summary>
        /// <param name="parameters"></param>
        public void KeyDownSlot(params object[] parameters)
        {
            KeyDownHandler((UnityEngine.Event)parameters[0]);
        }

        /// <summary>
        /// Key up slot
        /// </summary>
        /// <param name="parameters"></param>
        public void KeyUpSlot(params object[] parameters)
        {
            KeyUpHandler((UnityEngine.Event)parameters[0]);
        }

        #endregion

        #region Constructor

        protected GuiInspector() // constructor is protected
        {
            //_inputSlot = new RenderSlot(this);
            //_keyDownSlot = new KeyDownSlot(this);
            //_keyUpSlot = new KeyUpSlot(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The flag indicating that the inspector should be started on start
        /// If not auto-started, the component could be enabled/disabled using the Ctrl + Delete
        /// </summary>
        //public bool AutoStart;

        //private Font _font;
        ///// <summary>
        ///// Font used for displaying the label
        ///// </summary>
        //public Font Font;

        //private int _fontSize;
        ///// <summary>
        ///// Label font size
        ///// </summary>
        //public int FontSize = 8;

        private bool _sticky;
        /// <summary>
        /// The mode used with the CTRL key
        /// </summary>
        public static bool Sticky;

        //private int _borderWidth = 2;
        ///// <summary>
        ///// Border width
        ///// </summary>
        //public int BorderWidth = 2;

        private Color _borderColor;
        /// <summary>
        /// Border color
        /// </summary>
        public Color BorderColor = Color.red;

        private Color _textColor;
        /// <summary>
        /// Text color
        /// </summary>
        public Color TextColor = Color.white;

        private Font _font;
        /// <summary>
        /// Font
        /// </summary>
        public Font Font;

        #endregion

        #region Members

        private DisplayListMember _currentComponent;
        public DisplayListMember CurrentComponent
        {
            set
            {
                _currentComponent = value;
            }
        }
        
        private Rectangle _previousBounds = new Rectangle();
        
        #endregion

        #region Methods

        // ReSharper disable UnusedMember.Local
        /*[Obfuscation(Exclude = true)]
        void Awake() // changed from Start to Awake for the inspector becoming visible after the assembly rebuild - 20131222
        // ReSharper restore UnusedMember.Local
        {
            InitStage();
        }*/

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector enabled");
#endif
            InitStage(); // added for the inspector becoming visible after the assembly rebuild - 20131222
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
                Debug.Log("GuiInspector disabled");
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

            MouseEventDispatcher.InspectMode = true;

            if (null != _currentComponent && IsInspectable(_currentComponent))
                ShowOverlay(_currentComponent);
        }

        private void TurnOff()
        {
            HandleEventListeners();

            MouseEventDispatcher.InspectMode = false;

            HideOverlay();

            SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
        }

        /// <summary>
        /// Returns true if the component is inspectable
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        internal static bool IsInspectable(DisplayListMember component)
        {
            return component.Stage != InspectorOverlayStage.Instance || !DisableOverlayOnInspector;
        }

        private InspectorOverlayStage _stage;

        private void InitStage()
        {
            if (null != _stage)
                return; // init only once

            //Debug.Log("Initializing Stage");
            _stage = InspectorOverlayStage.Instance;
            _stage.Sticky = Sticky;
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
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target); // +", true" (TODO): Capture phase. Disable actual click.

                SystemManager.Instance.RenderSignal.Connect(RenderSlot); // get update signals for checking GUI state
                SystemManager.Instance.KeyDownSignal.Connect(KeyDownSlot);
                SystemManager.Instance.KeyUpSignal.Connect(KeyUpSlot);
            }
            else
            {
                //MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OVER, MouseOverSlot);
                //MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OUT, MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseOverSignal.Disconnect(MouseOverSlot);
                MouseEventDispatcher.Instance.InspectorMouseOutSignal.Disconnect(MouseOutSlot);
                MouseEventDispatcher.Instance.InspectorMouseLeaveSignal.Disconnect(MouseLeaveSlot);
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);

                SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
                SystemManager.Instance.KeyDownSignal.Disconnect(KeyDownSlot);
                SystemManager.Instance.KeyUpSignal.Disconnect(KeyUpSlot);
            }
        }

        //private void MouseOverHandler(Event e)
        private void MouseOverSlot(params object[] parameters)
        {
            Component comp = (Component)parameters[0];
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector->MouseOverSlot");
#endif
            if (Sticky)
                return;

            _currentComponent = comp;
            //_stack.Clear();

            ShowOverlay(_currentComponent);
        }

        //internal void MouseOutHandler(Event e)
        internal void MouseOutSlot(params object[] parameters)
        {
            //Component comp = (Component)parameters[0];
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector->MouseOutSlot");
#endif
            if (Sticky)
                return;

            _currentComponent = null;

            HideOverlay();
        }

        //internal void MouseOutSlot(Event e)
        internal void MouseLeaveSlot(params object[] parameters)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("PlayModeOverlay->MouseLeaveSlot");
#endif
            HideOverlay();
        }

        internal void ClickHandler(Event e)
        {
            //Debug.Log("GuiInspector->ClickHandler: " + e.Target);
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector->ClickHandler");
#endif
            MouseEvent me = (MouseEvent)e;
            if (me.CurrentEvent.control || me.CurrentEvent.shift)
            {
                //Debug.Log("Click");

                if (Sticky)
                {
                    // we are in sticky mode
                    if (e.Target == _currentComponent) // clicking the same component resets Sticky
                        Sticky = false;
                    else // else set new component as Sticky
                        _currentComponent = (Component)e.Target;
                }
                else // turn off sticky
                    Sticky = true;

                _sticky = Sticky; // cancel off GUI changes

                e.CancelAndStopPropagation(); // disable the actual button click in capture phase. Sweet ^_^
            }
            //_stack.Clear();

            //ClickSignal.Emit(_currentComponent);
            //Debug.Log("ClickSignal.NumSlots: " + ClickSignal.NumSlots);
        }

        #endregion

        #region Keys

        //private readonly Stack<DisplayListMember> _stack = new Stack<DisplayListMember>();

        internal void KeyDownHandler(UnityEngine.Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector->KeyDownHandler");
#endif

            if (!(e.control || e.shift))
                return;

            bool recognized = false;

            if (e.keyCode == KeyCode.LeftArrow)
            {
                if (null != _currentComponent && null != _currentComponent.Parent)
                {
                    //Debug.Log(string.Format("Pushing {0} to stack", _currentComponent));
                    //_stack.Push(_currentComponent);
                    _currentComponent = _currentComponent.Parent;
                    recognized = true;
                }
            }
            else if (e.keyCode == KeyCode.RightArrow)
            {
                //if (_stack.Count > 0)
                //{
                //    _currentComponent = _stack.Pop();
                //    recognized = true;
                //}
                //else
                //{
                    //Container container = _currentComponent as Container;
                    DisplayObjectContainer container = _currentComponent as DisplayObjectContainer; // 20121212
                    if (null != container && container.QNumberOfChildren > 0)
                    {
                        //_currentComponent = container.GetChildAt(0);
                        _currentComponent = container.QGetChildAt(0);
                        recognized = true;
                    }

                //}
            }
            else if (e.keyCode == KeyCode.UpArrow)
            {
                if (null != _currentComponent && null != _currentComponent.Parent)
                {
                    var index = _currentComponent.Parent.QGetChildIndex(_currentComponent);
                    if (index > 0){
                        index--;
                        _currentComponent = _currentComponent.Parent.QGetChildAt(index);
                        //_stack.Clear();
                    }
                    recognized = true;
                }
            }
            else if (e.keyCode == KeyCode.DownArrow)
            {
                if (null != _currentComponent && null != _currentComponent.Parent)
                {
                    var index = _currentComponent.Parent.QGetChildIndex(_currentComponent);
                    if (index < _currentComponent.Parent.QNumberOfChildren - 1)
                    {
                        index++;
                        _currentComponent = _currentComponent.Parent.QGetChildAt(index);
                        //_stack.Clear();
                    }
                    recognized = true;
                }
            }

            if (recognized)
            {
                HideOverlay();
                //Debug.Log("Showing: " + _currentComponent);
                ShowOverlay(_currentComponent);
            }
        }

        internal void KeyUpHandler(UnityEngine.Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("GuiInspector->KeyUpHandler");
#endif

            if (e.keyCode == KeyCode.Escape)
            {
                if (null != _currentComponent)
                {
                    // clear all
                    //_stack.Clear();
                    _currentComponent = null;
                    Sticky = false;
                    HideOverlay();
                }
            }
        }

        #endregion

        #region Input

        internal void RenderHandler(object parameter)
        {
            if (null == _stage) // could be null on application recompile
                return;

            if (_sticky != Sticky)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("Sticky changed");
#endif
                _sticky = Sticky;
                if (!_sticky)
                    HideOverlay();
            }

            // Checking if the current component moved and/or resized
            // NOTE: Using brute force checking here (instead of listening to move and resize events)
            // because the component may choose not to dispatch any events
            // However - no harm done - the GuiInspector should be off in the production
            if (null != _currentComponent) // && null != _stage)
            {
                if (!_previousBounds.Equals(_currentComponent.Transform.GlobalBounds) && IsInspectable(_currentComponent))
                {
                    _stage.DoShowOverlay(_currentComponent);
                    _previousBounds = _currentComponent.Transform.GlobalBounds;
                }
            }
        }

        

        #endregion

        #region Overlay

        private void ShowOverlay(DisplayObject target)
        {
            if (null == _stage) // could be null on application recompile
                return;

            _stage.DoShowOverlay(target);
        }

        private void HideOverlay()
        {
            if (null == _stage) // could be null on application recompile
                return;

            _stage.HideOverlay();
        }

        #endregion

        /// <summary>
        /// TEMP
        /// </summary>
        /// <returns></returns>
        public Component GetCurrentComponent()
        {
            return _currentComponent as Component;
        }

        /// <summary>
        /// TEMP
        /// </summary>
        /// <returns></returns>
        public static bool GetSticky()
        {
            return Sticky;
        }

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

        StyleDeclaration _declaration;

// ReSharper disable UnusedMember.Local
        void Update()
// ReSharper restore UnusedMember.Local
        {
            if (null == _declaration)
                _declaration = StyleManager.Instance.GetStyleDeclaration(typeof(InspectorOverlay).FullName);

            if (_borderColor != BorderColor || _textColor != TextColor || _font != Font)
            {
                if (_borderColor != BorderColor)
                {
                    _borderColor = BorderColor;
                    _declaration.SetStyle("borderColor", _borderColor);
                }
                if (_textColor != TextColor)
                {
                    _textColor = TextColor;
                    _declaration.SetStyle("textColor", _textColor);
                }
                if (_font != Font)
                {
                    _font = Font;
                    _declaration.SetStyle("font", _font);
                }
            }
        }

    }
}