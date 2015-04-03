using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using eDriven.Gui.Geom;
using eDriven.Gui.Managers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    public class Draggable : IPlugin
    {
        /// <summary>
        /// Enabled
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// Auto bring to front on mouse down
        /// </summary>
        public bool AutoBringToFront = true;

        private Component _component;

        private ConstraintMetrics _constraintMetrics = new ConstraintMetrics(0, 0, 0, 0);
        /// <summary>
        /// Constrains for dragging this draggable
        /// </summary>
        public ConstraintMetrics Constraints
        {
            get
            {
                return _constraintMetrics;
            }
            set
            {
                _constraintMetrics = value;
            }
        }

        private DisplayObject _dragHandle;
        /// <summary>
        /// Draggable area (a display object which is a drag handle)
        /// </summary>
        public DisplayObject DragHandle
        {
            get
            {
                return _dragHandle;
            }
            set
            {
                //if (value == _dragHandle)
                //    return;

                _dragHandle = value;
            }
        }

        #region Implementation of IPlugin

        private bool _constraintMode;

        private bool _initialized;

        public void Initialize(InvalidationManagerClient component)
        {
            Debug.Log("Draggable Initialize: _dragHandle: " + _dragHandle);

            if (_initialized)
                return;

            _initialized = true;

            _component = (Component) component;
            
            // subscribe to CAPTURE PHASE component events

            _constraintMode = null == _dragHandle;
            //Debug.Log("Constrain mode: " + _constraintMode);

            //if (null != _dragHandle && _dragHandle is InvalidationManagerClient)
            //    ((InvalidationManagerClient)_dragHandle).SetStyle("cursor", "move");

            DisplayObject target = _dragHandle ?? _component;

            target.AddEventListener(MouseEvent.MOUSE_MOVE, ComponentMouseMoveHandler, /*EventPhase.Capture | */EventPhase.Target); // NOTE: Target phase must be present because of the simple components
            target.AddEventListener(MouseEvent.MOUSE_OUT, ComponentMouseOutHandler, /*EventPhase.Capture | */EventPhase.Target);
            target.AddEventListener(MouseEvent.MOUSE_DOWN, ComponentMouseDownHandler, /*EventPhase.Capture | */EventPhase.Target);
            target.AddEventListener(FrameworkEvent.REMOVE, ComponentRemovedHandler);
        }

        #endregion

        Point _startCoordinates;
        Point _clickCoordinates;

        Rectangle _constrainedRect = new Rectangle();

        private int _cursorId = -1;
        
        /// <summary>
        /// We should listen the mouse move events over the component
        /// If mouse moved over the event, we should check border and draw overlay
        /// </summary>
        /// <param name="e"></param>
        private void ComponentMouseMoveHandler(Event e)
        {
            if (!Enabled)
                return;

            Debug.Log(string.Format("ComponentMouseMoveHandler [{0}, {1}]", e.Phase, e.CurrentTarget));

            if (!(e.Target is DisplayListMember))
                return;

            DisplayListMember dlm = (DisplayListMember)e.Target;

            MouseEvent me = (MouseEvent)e;

            //Container container = _component as Container;

            //Point coordsInComponentSpace = null != container ? 
            //      container.GlobalToContent(dlm.LocalToGlobal(me.LocalPosition)) : 
            //      _component.GlobalToLocal(dlm.LocalToGlobal(me.LocalPosition));

            Point coordsInComponentSpace = _component.GlobalToLocal(dlm.LocalToGlobal(me.LocalPosition));

            // if some component is already being dragged/resized, do not draw borders on hover
            if (DragManager.IsDragging)
                return;

            if (_constraintMode)
            {
                _constrainedRect = _constraintMetrics.GetConstrainedRectangle(_component.Transform.LocalBounds);
                DoRenderOverlay = _constrainedRect.Contains(coordsInComponentSpace);
            }
            else
            {
                DoRenderOverlay = true;
            }

            if (DoRenderOverlay)
            {
                e.Cancel();
            }
        }

        private void ComponentMouseOutHandler(Event e)
        {
            Debug.Log(string.Format("ComponentMouseOutHandler [{0}, {1}]", e.Phase, e.CurrentTarget));
            
            if (DragManager.IsDragging)
                return;
            
            e.Cancel();

            DragManager.IsDragging = false;

            DoRenderOverlay = false;
        }

        private void ComponentMouseDownHandler(Event e)
        {
            Debug.Log(string.Format("ComponentMouseDownHandler [{0}, {1}]", e.Phase, e.CurrentTarget));

            if (!Enabled)
                return;

            if (AutoBringToFront)
                _component.BringToFront();

            MouseEvent me = (MouseEvent) e;

            //var container = _component.Parent as Container; // 20121125 (because dragging didn't work well when parent scrolled)
            //_startCoordinates = null != container ?
            //    container.GlobalToContent(_component.Transform.GlobalPosition) : // GlobalToContent no more - TODO
            //    _component.Parent.GlobalToLocal(_component.Transform.GlobalPosition);

            _clickCoordinates = me.GlobalPosition;

            if (_constraintMode)
            {
                _constrainedRect = _constraintMetrics.GetConstrainedRectangle(_component.Transform.LocalBounds);

                if (!_constrainedRect.Contains(me.LocalPosition))
                    return;
            }

            e.CancelAndStopPropagation();

            DragManager.IsDragging = true;

            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);

            if (-1 == _cursorId)
            {
                _cursorId = CursorManager.Instance.SetCursor(CursorType.Move, CursorPriority.High);
            }
        }

        private void MouseMoveHandler(Event e)
        {
            MouseEvent me = (MouseEvent)e;
            Debug.Log(1);
            Point delta = me.GlobalPosition.Subtract(_clickCoordinates);
            Debug.Log(2);
            var position = _startCoordinates.Add(delta);
            Debug.Log(3);
            _component.Move(position.X, position.Y);
            Debug.Log(4);
        }

        private void MouseUpHandler(Event e)
        {
            e.CancelAndStopPropagation();

            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);

            DoRenderOverlay = false;

            DragManager.IsDragging = false;

            if (-1 != _cursorId) {
                CursorManager.Instance.RemoveCursor(_cursorId);
                _cursorId = -1;
            }
        }

        private void ComponentRemovedHandler(Event e)
        {
            CursorManager.Instance.RemoveCursor(_cursorId);
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, MouseUpHandler);

            if (null != _component)
            {
                _component.RemoveEventListener(MouseEvent.MOUSE_MOVE, ComponentMouseMoveHandler, EventPhase.Capture | EventPhase.Target); // NOTE: Target phase must be present because of the simple components
                _component.RemoveEventListener(MouseEvent.MOUSE_OUT, ComponentMouseOutHandler, EventPhase.Capture | EventPhase.Target);
                _component.RemoveEventListener(MouseEvent.MOUSE_DOWN, ComponentMouseDownHandler, EventPhase.Capture | EventPhase.Target);
            }

            CursorManager.Instance.RemoveCursor(_cursorId);
        }

        #endregion

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

                /**
                 * Subscribe to the last draw call of the component
                 * Add own draw logic
                 * */
                if (_doRenderOverlay)
                    _component.RenderDoneSignal.Connect(OverlayRenderSlot);
                else
                    _component.RenderDoneSignal.Disconnect(OverlayRenderSlot);
            }
        }

        //private GUIStyle _overlayStyle;
        /// <summary>
        /// The style that is being drawn over the draggable area on rollover and drag
        /// </summary>
        public static GUIStyle OverlayStyle; // = DraggableAreaStyle.Instance;

        private void OverlayRenderSlot(params object[] parameters)
        {
            if (null == OverlayStyle)
                return;

            if (UnityEngine.Event.current.type != EventType.Repaint)
                return;

            //if (null == _overlayStyle)
            //{
            //    _overlayStyle = DraggableAreaStyle.Instance; // TODO: via proxy
            //}

            // TODO: Optimize

            Rectangle rectangle = _constraintMode ?
                Rectangle.FromPositionAndSize(_component.Transform.Position, _constrainedRect.Size) :
                DragHandle.Transform.GlobalBounds;
            
            OverlayStyle.Draw(rectangle.ToRect(), false, false, false, true);
        }
    }
}