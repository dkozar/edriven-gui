using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using eDriven.Gui.Events;
using eDriven.Gui.Managers;
using eDriven.Gui.GUIStyles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    /// <summary>
    /// The resizable plugin
    /// </summary>
    public class Resizable : IPlugin
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        private Component _component;

        /// <summary>
        /// Is plugin enabled
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// Border weithgt of the resizable
        /// </summary>
        public float BorderWeight = 8;

        /// <summary>
        /// If set to true, the resizable will set the explicit width and height, instead of setting only the actual size
        /// </summary>
        public bool ChangeExplicitSize = true;

        #region Implementation of IPlugin

        private bool _initialized;

        public void Initialize(InvalidationManagerClient component)
        {
            if (_initialized)
                return;

            _initialized = true;

            _component = (Component) component;

            _component.OptimizeMeasureCalls = false;

            // subscribe to CAPTURE PHASE component events
            // listening for mouse move over the component border
            //_component.AddEventListener(MouseEvent.MOUSE_MOVE, IdleMouseMoveHandler, EventPhase.Capture | EventPhase.Target, EventPriority.CURSOR_MANAGEMENT); // NOTE: Target phase must be present because of the simple components
            _component.AddEventListener(MouseEvent.ROLL_OVER, IdleRollOverHandler, EventPhase.Capture | EventPhase.Target); //, EventPriority.CURSOR_MANAGEMENT); // NOTE: Target phase must be present because of the simple components
            _component.AddEventListener(MouseEvent.ROLL_OUT, IdleRollOutHandler, EventPhase.Capture | EventPhase.Target); //, EventPriority.CURSOR_MANAGEMENT); // NOTE: Target phase must be present because of the simple components

            //// listening for mouse click just to cancel it if being over the border
            //// TODO: implement priority and use the highest one
            //_component.AddEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);
        }

        private void IdleRollOverHandler(Event e)
        {
            if (e.Target != _component)
                return;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("IdleRollOverHandler");
            }
#endif
            SystemManager.Instance.MouseMoveSignal.Connect(MouseMoveSlot);
            //SystemManager.Instance.MouseUpSignal.Connect(MouseUpSlot);
        }

        private void IdleRollOutHandler(Event e)
        {
            if (IsResizing)
                return;

            if (e.Target != _component)
                return;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("IdleRollOutHandler");
            }
#endif
            SystemManager.Instance.MouseMoveSignal.Disconnect(MouseMoveSlot);
            //SystemManager.Instance.MouseUpSignal.Disconnect(MouseUpSlot);

            /* Important (the source of the bug of right side cursor not dissapearing on roll out */
            HideCursor();
            if (_scanning)
            {
                StopScanning();
            }
        }

        private void MouseMoveSlot(object[] parameters)
        {
            var position = (Point) parameters[1];

            if (!Enabled || IsResizing)
                return;

            var mob = IsMouseOverBorder(position);

            // if some component is already being dragged/resized, do not draw borders on hover
            if (DragManager.IsDragging)
            {
                //Debug.Log("DragManager is already dragging.");
                return;
            }

            if (IsResizing)
                //Debug.Log("Already resizing.");
                return;

            DoRenderOverlay = mob;

            if (mob)
            {
                //e.CancelAndStopPropagation();
                if (_previousResizeMode != _resizeMode)
                {
                    HideCursor();
                    ShowCursor(); // this call should be here, because we may walk around the border and change cursor
                    _previousResizeMode = _resizeMode;
                }
                if (!_scanning)
                {
                    StartScanning();
                }
            }
            else
            {
                HideCursor();
                if (_scanning)
                {
                    StopScanning();
                }
            }
        }
        
        /// <summary>
        /// A flag indicating that we were over the border and started the process of scanning 
        /// </summary>
        private bool _scanning;

        private void StartScanning()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("StartScanning");
            }
#endif
            // listening for roll out to remove resizing overlay
            _component.AddEventListener(MouseEvent.MOUSE_OUT, IdleMouseOutHandler, EventPhase.Capture | EventPhase.Target, EventPriority.CURSOR_MANAGEMENT);

            // listening for mouse down to start resizing process
            _component.AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, EventPhase.Capture | EventPhase.Target);

            // listening for mouse click just to cancel it if being over the border
            // TODO: implement priority and use the highest one
            _component.AddEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);

            _scanning = true;
        }

        private void StopScanning()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("StopScanning");
            }
#endif
            _component.RemoveEventListener(MouseEvent.MOUSE_OUT, IdleMouseOutHandler, EventPhase.Capture | EventPhase.Target);
            _component.RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, EventPhase.Capture | EventPhase.Target);
            _component.RemoveEventListener(MouseEvent.CLICK, ClickHandler, EventPhase.Capture | EventPhase.Target);

            _scanning = false;

            _previousResizeMode = null;
        }

        #region Cursor

        private int _cursorId = -1;

        private void ShowCursor()
        {
            //Debug.Log("ShowCursor -> " + _resizeMode);
            switch (_resizeMode)
            {
                case ResizeMode.Left:
                    _cursorType = CursorType.WResize;
                    break;
                case ResizeMode.Right:
                    _cursorType = CursorType.EResize;
                    break;
                case ResizeMode.Top:
                    _cursorType = CursorType.NResize;
                    break;
                case ResizeMode.Bottom:
                    _cursorType = CursorType.SResize;
                    break;
                case ResizeMode.TopLeft:
                    _cursorType = CursorType.NwResize;
                    break;
                case ResizeMode.TopRight:
                    _cursorType = CursorType.NeResize;
                    break;
                case ResizeMode.BottomLeft:
                    _cursorType = CursorType.SwResize;
                    break;
                case ResizeMode.BottomRight:
                    _cursorType = CursorType.SeResize;
                    break;
                default:
                    _cursorType = CursorType.Normal;
                    break;
            }
            _cursorId = CursorManager.Instance.SetCursor(_cursorType, CursorPriority.High);
            //Debug.Log(CursorManager.Instance.Report());
        }

        private void HideCursor()
        {
            if (-1 != _cursorId)
            {
                //Debug.Log("removing cursor: " + _cursorId);
                CursorManager.Instance.RemoveCursor(_cursorId);
                _cursorId = -1;
            }
        }

        #endregion

        private bool _showOverlay = true;
        /// <summary>
        /// A switch for turning off the overlay (border)<br/>
        /// True by default
        /// </summary>
        public bool ShowOverlay
        {
            get { 
                return _showOverlay;
            }
            set
            {
                if (value == _showOverlay)
                    return;

                _showOverlay = value;
                HandleOverlayVisibility();
            }
        }

        private bool _doRenderOverlay;
        protected bool DoRenderOverlay
        {
            get
            {
                return _doRenderOverlay;
            }
            set
            {
                if (value == _doRenderOverlay)
                    return;

                _doRenderOverlay = value;
                HandleOverlayVisibility();
            }
        }

        /// <summary>
        /// Connects or disconnects from RenderDoneSignal
        /// </summary>
        private void HandleOverlayVisibility()
        {
            /**
             * Subscribe to the last draw call of the component
             * Add own draw logic
             * */
            if (_doRenderOverlay && ShowOverlay) {
                if (null != _component)
                    _component.RenderDoneSignal.Connect(OverlayRenderSlot);
            }
            else {
                if (null != _component)
                    _component.RenderDoneSignal.Disconnect(OverlayRenderSlot);
            }
        }

        /// <summary>
        /// Fires on mouse out when idle
        /// </summary>
        /// <param name="e"></param>
        private void IdleMouseOutHandler(Event e)
        {
            MouseEvent me = (MouseEvent) e;
            if (_component.Bounds.Contains(_component.GlobalToLocal(me.GlobalPosition)))
                return; // still inside

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("IdleMouseOutHandler");
            }
#endif
            if (DragManager.IsDragging)
                return;

            e.Cancel();

            DoRenderOverlay = false;

            HideCursor();
            StopScanning();

            _previousResizeMode = null;
        }

        #endregion

        //private bool _isMouseDown;

        /// <summary>
        /// Should we bring the component to front on border mousedown
        /// </summary>
        public bool AutoBringToFront = true;

        /// <summary>
        /// Should we exclude component from layout on border mousedown
        /// </summary>
        public bool AutoExcludeFromLayout;

        /// <summary>
        /// Mouse down handler
        /// </summary>
        /// <param name="e"></param>
        private void MouseDownHandler(Event e)
        {
            //Debug.Log(string.Format("Resizable [{0}] -> MouseDownHandler", e.Target));
            if (AutoBringToFront)
                _component.BringToFront();

            if (AutoExcludeFromLayout)
                _component.IncludeInLayout = false;

            MouseEvent me = (MouseEvent)e;

            IsResizing = IsMouseOverBorder(me.GlobalPosition);
            if (IsResizing)
            {
                e.CancelAndStopPropagation();
                
                DoRenderOverlay = true;

                _origBounds = _component.Bounds;
                
                _clickCoords = me.GlobalPosition;

                // subscribe to mouse moves
                SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler, EventPhase.Capture | EventPhase.Target);
                SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);

                DragManager.IsDragging = true;
            }
        }

        /// <summary>
        /// Click handler. Stop propagation if clicked on border.
        /// </summary>
        /// <param name="e"></param>
        private void ClickHandler(Event e)
        {
            MouseEvent me = (MouseEvent)e;
            if (IsMouseOverBorder(me.GlobalPosition))
                e.CancelAndStopPropagation();
        }

        private bool _r;
        private bool _l;
        private bool _t;
        private bool _b;
        
        internal static bool IsResizing;
        private ResizeMode _resizeMode = ResizeMode.Right;

        private string _cursorType = CursorType.Normal;

        private Point _clickCoords;
        private Rectangle _origBounds;
        private Rectangle _newBounds;

        /// <summary>
        /// Mouse move handler
        /// Fires when in processing mode (resizing)
        /// </summary>
        /// <param name="e"></param>
        private void MouseMoveHandler(Event e)
        {
            if (!IsResizing)
                return;

            e.CancelAndStopPropagation();

            MouseEvent me = (MouseEvent)e;

            Point position;

            position = me.GlobalPosition;

            var delta = position.Subtract(_clickCoords);

            switch (_resizeMode)
            {
                case ResizeMode.Right:
                    _newBounds = _origBounds.Expand(0, delta.X, 0, 0);
                    break;
                case ResizeMode.Left:
                    _newBounds = _origBounds.Expand(-delta.X, 0, 0, 0);
                    break;
                case ResizeMode.Top:
                    _newBounds = _origBounds.Expand(0, 0, -delta.Y, 0);
                    break;
                case ResizeMode.Bottom:
                    _newBounds = _origBounds.Expand(0, 0, 0, delta.Y);
                    break;
                case ResizeMode.TopLeft:
                    _newBounds = _origBounds.Expand(-delta.X, 0, -delta.Y, 0);
                    break;
                case ResizeMode.TopRight:
                    _newBounds = _origBounds.Expand(0, delta.X, -delta.Y, 0);
                    break;
                case ResizeMode.BottomLeft:
                    _newBounds = _origBounds.Expand(-delta.X, 0, 0, delta.Y);
                    break;
                case ResizeMode.BottomRight:
                    _newBounds = _origBounds.Expand(0, delta.X, 0, delta.Y);
                    break;
            }

            // gracefully handle the minimum sizes set by the user
            _newBounds.X = Mathf.Min(_newBounds.X, _origBounds.Right - _component.MinWidth);
            _newBounds.Y = Mathf.Min(_newBounds.Y, _origBounds.Bottom - _component.MinHeight);
            _newBounds.Width = Mathf.Min(Mathf.Max(_newBounds.Width, _component.MinWidth), _component.MaxWidth); // MeasuredMinWidth
            _newBounds.Height = Mathf.Min(Mathf.Max(_newBounds.Height, _component.MinHeight), _component.MaxHeight);

            // apply
            if (ChangeExplicitSize)
            {
                _component.Width = _newBounds.Width;
                _component.Height = _newBounds.Height;
            }
            else
            {
                _component.SetActualSize(_newBounds.Width, _newBounds.Height);
            }
            
            _component.Move(_newBounds.X, _newBounds.Y);

            /**
             * When resizing by the top/left edge, the visual jitter can be observed in component's right-aligned children
             * so to avoid this jitter, we should validate now
             * */
            if (_resizeMode == ResizeMode.Left || 
                _resizeMode == ResizeMode.Top || 
                _resizeMode == ResizeMode.TopLeft)
            {
                //_component.ValidateNow();    
            }
        }

        private GUIStyle _borderStyle;

        private void OverlayRenderSlot(params object[] parameters)
        {
            if (UnityEngine.Event.current.type != EventType.Repaint)
                return;

            if (null == _borderStyle)
            {
                _borderStyle = ResizableBorderStyle.Instance; // TODO: via proxy
            }

            _borderStyle.Draw(_component.RenderingRect, false, false, false, true);
        }

        /// <summary>
        /// Mouse up handler: unsubscribe from everything
        /// </summary>
        /// <param name="e"></param>
        private void MouseUpHandler(Event e)
        {
            MouseEvent me = (MouseEvent) e;

            e.CancelAndStopPropagation();
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler, EventPhase.Capture | EventPhase.Target);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);

            _component.IncludeInLayout = true;
            IsResizing = false;

            DragManager.IsDragging = false;

            DeferManager.Instance.Defer(delegate
             {
                 //Debug.Log("check");
                 //Point p = _component.GlobalToLocal(me.GlobalPosition);
                 DoRenderOverlay = IsMouseOverBorder(me.GlobalPosition);

                 if (DoRenderOverlay)
                 {
                     if (!_scanning)
                         ShowCursor();
                 }
                 else
                 {
                     if (_scanning)
                         HideCursor();
                 }
                     
             });
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (null != _component)
            {
                HideCursor();
                StopScanning();
                //_component.RemoveEventListener(MouseEvent.MOUSE_MOVE, IdleMouseMoveHandler, EventPhase.Capture | EventPhase.Target);
            }

            //SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            //SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);
            SystemManager.Instance.MouseMoveSignal.Disconnect(MouseMoveSlot);
            //SystemManager.Instance.MouseUpSignal.Disconnect(MouseUpSlot);
        }

        #endregion

        #region Helper

        ///// <summary>
        ///// Checks of the value is in range
        ///// </summary>
        ///// <param name="value"></param>
        ///// <param name="min"></param>
        ///// <param name="max"></param>
        ///// <param name="including"></param>
        ///// <returns></returns>
        ////private static bool InRange(float value, float min, float max, bool including)
        ////{
        ////    return including ? value >= min && value <= max : value > min && value < max;
        ////}

        private ResizeMode? _previousResizeMode;
        private bool IsMouseOverBorder(Point globalPosition)
        {
            //Point coordsInComponentSpace = _component.GlobalToLocal(globalPosition);
            
            //_l = InRange(coordsInComponentSpace.X, 0, BorderWeight, true) && InRange(coordsInComponentSpace.Y, 0, _component.Height, true);
            //_r = InRange(coordsInComponentSpace.X, _component.Width - BorderWeight, _component.Width, true) && InRange(coordsInComponentSpace.Y, 0, _component.Height, true);
            //_t = InRange(coordsInComponentSpace.Y, 0, BorderWeight, true) && InRange(coordsInComponentSpace.X, 0, _component.Width, true);
            //_b = InRange(coordsInComponentSpace.Y, _component.Height - BorderWeight, _component.Height, true) && InRange(coordsInComponentSpace.X, 0, _component.Width, true);

            var leftBorder = _component.LocalToGlobal(new Rectangle(0, 0, BorderWeight, _component.Height));
            var rightBorder = _component.LocalToGlobal(new Rectangle(_component.Width - BorderWeight, 0, BorderWeight, _component.Height));
            var topBorder = _component.LocalToGlobal(new Rectangle(0, 0, _component.Width, BorderWeight));
            var bottomBorder = _component.LocalToGlobal(new Rectangle(0, _component.Height - BorderWeight, _component.Width, BorderWeight));

            _l = leftBorder.Contains(globalPosition);
            _r = rightBorder.Contains(globalPosition);
            _t = topBorder.Contains(globalPosition);
            _b = bottomBorder.Contains(globalPosition);

            bool isResizing = true;

            if (_l && _t) {
                _resizeMode = ResizeMode.TopLeft;
            }
            else if (_r && _t) {
                _resizeMode = ResizeMode.TopRight;
            }
            else if (_l && _b) {
                _resizeMode = ResizeMode.BottomLeft;
            }
            else if (_r && _b) {
                _resizeMode = ResizeMode.BottomRight;
            }
            else if (_l) {
                _resizeMode = ResizeMode.Left;
            }
            else if (_r) {
                _resizeMode = ResizeMode.Right;
            }
            else if (_t) {
                _resizeMode = ResizeMode.Top;
            }
            else if (_b) {
                _resizeMode = ResizeMode.Bottom;
            }
            else {
                isResizing = false;
            }

            return isResizing;
        }

        #endregion
    }

    internal enum ResizeMode
    {
        Left, Right, Top, Bottom,
        TopLeft, TopRight, BottomLeft, BottomRight
    }
}