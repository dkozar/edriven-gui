using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Core.Managers;
using eDriven.Gui.Animation;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Cursor;
using eDriven.Gui.Managers;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Stages;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.DragDrop
{
    /**
     *  Flex explanation :)
     * 
     *  The DragManager class manages drag and drop operations, which let you 
     *  move data from one place to another in a Flex application.
     *  For example, you can select an object, such as an item in a List control
     *  or a Flex control, such as an Image control, and then drag it over
     *  another component to add it to that component.
     *  
     *  <p>All methods and properties of the DragManager are static,
     *  so you do not need to create an instance of it.</p>
     *  
     *  <p>All Flex components support drag and drop operations.
     *  Flex provides additional support for drag and drop to the List,
     *  Tree, and DataGrid controls.</p>
     *  
     *  <p>When the user selects an item with the mouse,
     *  the selected component is called the drag initiator.
     *  The image displayed during the drag operation is called the drag proxy.</p>
     *  
     *  <p>When the user moves the drag proxy over another component,
     *  the <code>dragEnter</code> event is sent to that component. 
     *  If the component accepts the drag, it becomes the drop target
     *  and receives <code>dragOver</code>, <code>dragExit</code>,  
     *  and <code>dragDrop</code> events.</p>
     *  
     *  <p>When the drag is complete, a <code>dragComplete</code> event
     *  is sent to the drag initiator.</p>
     */

    /// <summary>
    /// A class for handling drag & drop
    /// Static class (could be singleton)
    /// </summary>
    public static class DragDropManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool DebugMode;

        public enum Action
        {
            Copy, Link, Move, None
        }

        private static Component _dragInitiator;
        private static DragSource _dragSource;
        private static MouseEvent _mouseEvent;
        private static DisplayObject _dragImage;
        private static float _xOffset;
        private static float _yOffset;
        private static float _imageAlpha;
        private static bool _allowMove;

        private static bool _proxyShouldBeVisible;
        private static bool _overlayShouldBeVisible;
        private static bool _feedbackShouldBeVisible;

        //private static Component _mouseDownComponent;
        private static Component _dragOverComponent;
        
        private static bool _dragging;
        /// <summary>
        /// Read-only property that returns true if a drag is in progress
        /// </summary>
        public static bool IsDragging
        {
            get
            {
                return _dragging;
            }
        }

        internal static List<Stage> StageList;

        private static int _cursor = -1;

        #region Quasi-Singleton

        private static Point Offset = new Point();

        private static Component _overlay = new DragOverlay();
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Component Overlay
        {
            get
            {
                if (_overlay == null)
                {
                    _overlay = new DragOverlay();
                    //_overlay.Alpha = 0.5f;
                }

                return _overlay;
            }
        }

        private static Component _proxy;
        /// <summary>
        /// Singleton instance
        /// </summary>
        //public static Component Proxy
        //{
        //    get
        //    {
        //        if (_proxy == null)
        //        {
        //            _proxy = new DragProxy();
        //            DragDropStage.Instance.AddChild(_proxy);
        //        }

        //        return _proxy;
        //    }
        //}

        #endregion

        private static Component _lastDragOverComponent;
        private static Component _acceptedTarget;
        private static Action _action = Action.None;

        #region Public methods

        /**
	     *  Initiates a drag and drop operation.
	     *
	     *  Param: dragInitiator IUIComponent that specifies the component initiating
	     *  the drag.
	     *
	     *  Param: dragSource DragSource object that contains the data
	     *  being dragged.
	     *
	     *  Param: mouseEvent The MouseEvent that contains the mouse information
	     *  for the start of the drag.
	     *
	     *  Param: dragImage The image to drag. This argument is optional.
	     *  If omitted, a standard drag rectangle is used during the drag and
	     *  drop operation. If you specify an image, you must explicitly set a 
	     *  height and width of the image or else it will not appear.
	     *
	     *  Param: xOffset Number that specifies the x offset, in pixels, for the
	     *  <code>dragImage</code>. This argument is optional. If omitted, the drag proxy
	     *  is shown at the upper-left corner of the drag initiator. The offset is expressed
	     *  in pixels from the left edge of the drag proxy to the left edge of the drag
	     *  initiator, and is usually a negative number.
	     *
	     *  Param: yOffset Number that specifies the y offset, in pixels, for the
	     *  <code>dragImage</code>. This argument is optional. If omitted, the drag proxy
	     *  is shown at the upper-left corner of the drag initiator. The offset is expressed
	     *  in pixels from the top edge of the drag proxy to the top edge of the drag
	     *  initiator, and is usually a negative number.
	     *
	     *  Param: imageAlpha Number that specifies the alpha value used for the
	     *  drag image. This argument is optional. If omitted, the default alpha
	     *  value is 0.5. A value of 0.0 indicates that the image is transparent;
	     *  a value of 1.0 indicates it is fully opaque. 
             *
             *  Param: allowMove Indicates if a drop target is allowed to move the dragged data.
	     *  
	     */
        
        /// <summary>
        /// Starts a drag and drop operation
        /// </summary>
        /// <param name="dragInitiator"></param>
        /// <param name="dragSource"></param>
        /// <param name="mouseEvent"></param>
        /// <param name="dragImage"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="imageAlpha"></param>
        /// <param name="allowMove"></param>
        /// <param name="options"></param>
        public static void DoDrag(Component dragInitiator, DragSource dragSource, MouseEvent mouseEvent, Component dragImage, float xOffset, float yOffset, float imageAlpha, bool allowMove, params DragOption[] options)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DragDropManager.DoDrag: " + dragInitiator);
#endif
            _dragInitiator = dragInitiator;
            _dragSource = dragSource;
            _mouseEvent = mouseEvent;
            _dragImage = dragImage;
            _imageAlpha = imageAlpha;
            _allowMove = allowMove;

            ApplyOptions(options);

            /**
             * 20130307
             * Found some glitches regarding the starting the new drag operation while the previous one hasn't been finished yet
             * Since I was dealing with a single proxy instance since, some of the old (custom) proxies were stale, e.g. never removed when the tween finished
             * Now I added the line to a tween callback which destroys the animation target
             * */
            _proxy = dragImage ?? new DragProxy();

            DragProxy.Proxify(_proxy);

            _xOffset = xOffset;
            _yOffset = yOffset;
            
            DragDropStage.Instance.AddChild(_proxy); // TODO: cleanup after the drag operaion

            var dragInitiatorGlobalBounds = dragInitiator.Parent.LocalToGlobal(dragInitiator.Position);

            _proxy.X = dragInitiatorGlobalBounds.X;
            _proxy.Y = dragInitiatorGlobalBounds.Y;
            //_proxy.Bounds = (Rectangle)dragInitiator.Transform.GlobalBounds.Clone();
            _proxy.Visible = _proxyShouldBeVisible;
            _proxy.Alpha = imageAlpha;

            Offset = dragInitiatorGlobalBounds.Subtract(mouseEvent.GlobalPosition);
            
            if (_feedbackShouldBeVisible)
                ChangeCursorTo(CursorType.RejectDrop);

            /**
             * Subscribe to drag and mouse up events on system manager
             * */
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DRAG, OnMouseDrag);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_UP, OnMouseUp);

            //DragEvent dragStartEvent = BuildEvent(DragEvent.DRAG_START);
            //_dragInitiator.DispatchEvent(dragStartEvent);
        }

        private static void ChangeCursorTo(string type)
        {
            RemoveCursor();
            _cursor = CursorManager.Instance.SetCursor(type);
        }

        private static void RemoveCursor()
        {
            if (-1 != _cursor)
                CursorManager.Instance.RemoveCursor(_cursor);
        }

        /**
	     *  Call this method from your <code>dragEnter</code> event handler if you accept
	     *  the drag/drop data.
	     */
        public static void AcceptDragDrop(Component target, params DragOption[] options)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DragDropManager.AcceptDragDrop: " + target);
#endif
            _acceptedTarget = target;

            ApplyOptions(options);
        }

        /**
	     *  Sets the feedback indicator for the drag and drop operation.
	     *  Possible values are <code>DragManager.COPY</code>, <code>DragManager.MOVE</code>,
	     *  <code>DragManager.LINK</code>, or <code>DragManager.NONE</code>.
	     *
	     *  Param: feedback The type of feedback indicator to display.
	     */
        public static void ShowFeedback(Action action)
        {
            _action = action;
        }

        /**
	     *  Returns the current drag and drop feedback.
	     *
	     *  Returns:  Possible return values are <code>DragManager.COPY</code>, 
	     *  <code>DragManager.MOVE</code>,
	     *  <code>DragManager.LINK</code>, or <code>DragManager.NONE</code>.
	     */
        public static Action GetFeedback()
        {
            return _action;
        }

        #endregion

        #region Event handlers

        private static void OnMouseDrag(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("DragDropManager.OnMouseDrag: " + _dragInitiator);
#endif

            MouseEvent input = (MouseEvent)e;

            DragEvent de;

            /**
             * 1) Check for drag start
             * */
            if (!_dragging) // if not yet dragging
            {
                _dragOverComponent = null;
                _dragging = true; // we're dragging now
                if (null != _dragInitiator) // if dragging something
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log("DragEvent.DRAG_START: " + _dragInitiator);
#endif
                    de = BuildEvent(DragEvent.DRAG_START);
                    _dragInitiator.DispatchEvent(de);
                }

                return;
            }

            _dragOverComponent = CoordinateProcessor.GetComponentUnderCoordinatesOnAllStages(input.GlobalPosition, delegate(DisplayListMember dlm)
            {
                Component component = dlm as Component;
                return null != component && component.MouseEnabled; // && !(component is Stage); // commented 20130307
            }, true, true) as Component; // stopOnDisabled, stopOnInvisible

            bool isDragEnter = null != _dragOverComponent && 
                               _dragOverComponent != _lastDragOverComponent &&
                               _dragInitiator != null &&
                               _dragInitiator != _dragOverComponent;

            /**
             * 2) Check for drag enter
             * */
            if (isDragEnter)
            {
                // nullify accepted target, and allow the user to react on DRAG_ENTER event (to call AcceptDragDrop)
                _acceptedTarget = null;
                _action = Action.None;
                //Debug.Log("DragEvent.DRAG_ENTER: " + _dragOverComponent.GetType().Name);
#if DEBUG
                if (DebugMode)
                    Debug.Log("DragEvent.DRAG_ENTER: " + _dragOverComponent.GetType().Name);
#endif
                // dispatch DragEvent.DRAG_EXIT on _lastDragOverComponent
                if (null != _lastDragOverComponent)
                {
                    de = BuildEvent(DragEvent.DRAG_EXIT);
                    _lastDragOverComponent.DispatchEvent(de);
                }

                // dispatch DragEvent.DRAG_ENTER on _dragOverComponent
                de = BuildEvent(DragEvent.DRAG_ENTER);
                _dragOverComponent.DispatchEvent(de);

                // set _lastDragOverComponent
                _lastDragOverComponent = _dragOverComponent;
            }

            /**
             * 3) Handle feedback to the user
             * By this point, if the user was subscribed to DragEvent.DRAG_ENTER, he could have react with AcceptDragDrop() and (optionally) ShowFeedback()
             * */

            // check if the user has accepted drag and drop
            if (null != _acceptedTarget)
            {
                // if drop is accepted, show overlay, and show CursorType.AcceptDrop cursor
                Overlay.Visible = _overlayShouldBeVisible;
                //_overlay.SetBounds(_acceptedTarget.GlobalBounds); // fix? .MoveBy(new Point(-1, 0)),
                //Overlay.Bounds = (Rectangle)_acceptedTarget.Transform.GlobalBounds.Clone(); // consider cloning bounds 20120415
                
                if (_feedbackShouldBeVisible)
                {
                    if (Action.None == _action)
                        ChangeCursorTo(CursorType.AcceptDrop);
                    else
                        ProcessFeedback(_action);
                }
                else
                    ChangeCursorTo(CursorType.Normal);
            }
            else
            {
                // if drop is rejected (meaning not accepted by developer), hide overlay, and show CursorType.RejectDrop cursor
                Overlay.Visible = false;
                if (_feedbackShouldBeVisible)
                    ChangeCursorTo(CursorType.RejectDrop);
                else
                    ChangeCursorTo(CursorType.Normal);
            }

            /**
             * 4) Move proxy
             * */
            _proxy.X = input.GlobalPosition.X + Offset.X;
            _proxy.Y = input.GlobalPosition.Y + Offset.Y;
            _proxy.ValidateNow();
        }

        private static void OnMouseUp(Event e)
        {
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, OnMouseUp);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_DRAG, OnMouseDrag);
            ChangeCursorTo(CursorType.Normal);

            MouseEvent me = (MouseEvent) e;

            bool dropAccepted = null != _acceptedTarget;

            //_proxy.Visible = false;
            _overlay.Visible = false;

            var newBounds = dropAccepted
                ? Rectangle.FromPositionAndSize(me.GlobalPosition, new Point(0, 0))
                : _dragInitiator.Transform.GlobalBounds;

            DragProxyTween tween = new DragProxyTween(_proxy, _proxy.Bounds, newBounds)
                                       {
                                           Callback = delegate(IAnimation anim)
                                           {
                                               //Debug.Log("*** Finished");
                                               Component proxy = (Component) anim.Target;
                                               proxy.Visible = false;
                                               DragDropStage.Instance.RemoveChild(proxy);
                                           }
                                       };
            tween.Play(_proxy);

            RemoveCursor();

            if (null != _acceptedTarget)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("DragEvent.DRAG_DROP: " + _acceptedTarget.GetType().Name);
#endif
                DragEvent de = BuildEvent(DragEvent.DRAG_DROP);
                _acceptedTarget.DispatchEvent(de);
            }

            DragEvent dragCompleteEvent = BuildEvent(DragEvent.DRAG_COMPLETE);
            _dragInitiator.DispatchEvent(dragCompleteEvent);

            _acceptedTarget = null;
            _lastDragOverComponent = null;

            _dragging = false;
            _dragInitiator = null;
            _dragOverComponent = null;
        }

        #endregion

        #region Helper

        private static DragEvent BuildEvent(string type)
        {
            DragEvent dragStartEvent = new DragEvent(type)
                                           {
                                               DragSource = _dragSource,
                                               DragInitiator = _dragInitiator,
                                               DraggedItem = _dragImage,
                                               Action = _action
                                           };
            return dragStartEvent;
        }

        private static void ProcessFeedback(Action action)
        {
            switch (action)
            {
                case Action.Copy:
                    ChangeCursorTo(CursorType.DragCopy);
                    break;
                case Action.Move:
                    ChangeCursorTo(CursorType.DragMove);
                    break;
                case Action.Link:
                    ChangeCursorTo(CursorType.DragLink);
                    break;
                case Action.None:
                    //CursorLogic.CursorLogic.Show(CursorType.RejectDrop);
                    //ChangeCursorTo(CursorType.NormalColor);
                    RemoveCursor();
                    break;
            }
        }

        internal static void ApplyOptions(DragOption[] options)
        {
            //Debug.Log("ApplyOptions");

            _proxyShouldBeVisible = true;
            _overlayShouldBeVisible = true;
            _feedbackShouldBeVisible = true;

            if (options != null)
            {
                int len = options.Length;
                for (int i = 0; i < len; i++)
                {
                    DragOption option = options[i];
                    switch (option.Type)
                    {
                        case DragOptionType.ProxyVisible:
                            _proxyShouldBeVisible = (bool)option.Value;
                            break;

                        case DragOptionType.OverlayVisible:
                            _overlayShouldBeVisible = (bool)option.Value;
                            break;

                        case DragOptionType.FeedbackVisible:
                            _feedbackShouldBeVisible = (bool)option.Value;
                            break;
                    }
                }
            }

            //_boxButtons.Initialize();
        }

        #endregion

    }

    public class DragOption
    {
        public DragOptionType Type;
        public object Value;

        public DragOption(DragOptionType type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum DragOptionType
    {
        // ReSharper disable UnusedMember.Global
        ProxyVisible, OverlayVisible, FeedbackVisible
        // ReSharper restore UnusedMember.Global
    }
}
