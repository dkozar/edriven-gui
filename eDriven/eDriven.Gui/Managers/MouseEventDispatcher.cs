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
using System.Collections.Generic;
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Core.Signals;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Cursor;
using eDriven.Gui.Events;
using eDriven.Gui.Util;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// The manager responsible for handling mouse events on components
    /// </summary>
    public class MouseEventDispatcher : EventDispatcher
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// Optimization<br/>
        /// Some calculations, like for the MouseWheelTarget for example, doesn't need to be updated<br/>
        /// on each mouse move. However, when using the GuiInspector, we'd like to update its label for MWT<br/>
        /// each time the user mouse-overs the new MWT
        /// </summary>
        public static bool InspectMode;

        /// <summary>
        /// Optimization<br/>
        /// Some calculations, like for the MouseWheelTarget for example, doesn't need to be updated<br/>
        /// on each mouse move. However, when using the GuiInspector, we'd like to update its label for MWT<br/>
        /// each time the user mouse-overs the new MWT
        /// </summary>
        public static bool PlayModeInspect;

        #region Singleton

        private static MouseEventDispatcher _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private MouseEventDispatcher()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MouseEventDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating MouseEventDispatcher instance"));
#endif
                    _instance = new MouseEventDispatcher();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region Static

        /// <summary>
        /// Time in seconds that has to pass between two clicks not to be recognized as a double-click gesture
        /// </summary>
        public static float DoubleClickDelay = 0.5f;

        /// <summary>
        /// Time in seconds that has to pass between two clicks not to be recognized as a double-click gesture
        /// </summary>
        public static float MiddleDoubleClickDelay = 0.5f;

        /// <summary>
        /// Time in seconds that has to pass between two clicks not to be recognized as a double-click gesture
        /// </summary>
        public static float RightDoubleClickDelay = 0.5f;

        /// <summary>
        /// The reference to a component that is under the mouse
        /// This component is looked upon after each mouse move on all stages
        /// </summary>
        public static Component MouseTarget { get; private set; }

        private static List<Component> _mouseWheelTargets = new List<Component>();

        /// <summary>
        /// The reference to a component that will receive mouse wheel events
        /// This component is looked upon after each mouse move on all stages
        /// </summary>
        public static List<Component> MouseWheelTargets
        {
            get
            {
                return _mouseWheelTargets;
            }
        }

        /// <summary>
        /// The reference to a component that is under the mouse
        /// This component is looked upon after each mouse move on all stages
        /// </summary>
        public static Component InspectorTarget { get; private set; }

        /// <summary>
        /// The reference to a component that is mouse-downed
        /// </summary>
        public static Component MouseDownComponent
        {
            get { return Instance._mouseDownComponent; }
        }

        /// <summary>
        /// Fires when mouse over occurs (even with disabled components)
        /// </summary>
        public Signal InspectorMouseOverSignal = new Signal();

        /// <summary>
        /// Fires when mouse out occurs (even with disabled components)
        /// </summary>
        public Signal InspectorMouseOutSignal = new Signal();

        /// <summary>
        /// Fires when mouse leave occurs
        /// </summary>
        public Signal InspectorMouseLeaveSignal = new Signal();

        #endregion

        #region Members

        private MouseEvent _mouseEvent;

        private Component _previousMouseOveredComponent;
        private Component _mouseDownComponent;
        private Component _rightMouseDownComponent;
        private Component _middleMouseDownComponent;
        private Component _lastClickedComponent;
        private DateTime _lastClickedTime;
        private DateTime _lastMiddleClickedTime;
        private DateTime _lastRightClickedTime;
        private readonly List<Component> _rollOveredComponentsToChangeState = new List<Component>();
        
        #endregion

        #region Methods

        /// <summary>
        /// Subscription to SystemManager
        /// </summary>
        private void Initialize()
        {
            // subscribe to system manager mouse down and mouse up events
            //SystemManager.Instance.AddEventListener(MouseEvent.MOUSE_DRAG, OnMouseDrag); // -> mouse drag is processed by DragDropManager
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DRAG, OnMouseDrag);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DOWN, OnMouseDown);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_UP, OnMouseUp);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.RIGHT_MOUSE_DOWN, OnRightMouseDown);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.RIGHT_MOUSE_UP, OnRightMouseUp);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MIDDLE_MOUSE_DOWN, OnMiddleMouseDown);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MIDDLE_MOUSE_UP, OnMiddleMouseUp);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_WHEEL, OnMouseWheel);

#if RELEASE
    /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        #endregion

        private bool _shouldDispatchRollOver;
        private bool _shouldDispatchRollOut;
        private bool _hasBeenMouseOut;
        private bool _hasBeenMouseDown;

        private int _currentCursorId = -1;

        #region Mouse events

        private Point _pos;
        private Point _size;
        private bool _isMouseLeave;
        private MouseEvent _mouseLeaveEvent;

        /// <summary>
        /// Clears the hover cursor
        /// </summary>
        public void ClearHoverCursor()
        {
            if (_currentCursorId > -1)
                CursorManager.Instance.RemoveCursor(_currentCursorId);
        }

        private void OnMouseMove(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMouseMove");
#endif
            _mouseEvent = (MouseEvent) e;

            _pos = _mouseEvent.GlobalPosition;
            _size = SystemManager.Instance.ScreenSize;

            if (_pos.X < 0 || _pos.Y < 0 || _pos.X > _size.X || _pos.Y > _size.Y)
            {
                if (!_isMouseLeave)
                {
                    /**
                     * 1) InvalidateDrawingList the event
                     * */
                    _mouseLeaveEvent = new MouseEvent(MouseEvent.MOUSE_LEAVE)
                                           {
                                               Target = this,
                                               GlobalPosition = _pos
                                           };
                }
                _isMouseLeave = true;
            }
            else
            {
                _isMouseLeave = false;
            }

            //Debug.Log("..... will recalculate ..... ");
            /**
             * 1) Find (any) component under under mouse
             * */
            RecalculateMouseTarget();

            //RecalculateMouseWheelTargets();

            //Debug.Log("InspectMode: " + InspectMode);

            /**
             * 2) Handle inspector target under mouse
             * */
            if (PlayModeInspect || InspectMode)
            {
                RecalculateInspectorTarget();
            }

            if (null != MouseTarget)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("OnMouseMove component: " + MouseTarget);
#endif
                MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MOUSE_MOVE,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
            }

            if (MouseTarget != _previousMouseOveredComponent || _isMouseLeave)
            {
                if (null != _previousMouseOveredComponent)
                {
                    MouseEventHelper.BuildAndDispatchMouseEvent(this, _previousMouseOveredComponent,
                                                                MouseEvent.MOUSE_OUT, _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
                    //Debug.Log("MOUSE_OUT: " + _previousMouseOveredComponent);
                    _hasBeenMouseOut = true;

                    string cursorStyle = (string) _previousMouseOveredComponent.GetStyle("cursor");
                    if (!string.IsNullOrEmpty(cursorStyle))
                        CursorManager.Instance.RemoveCursor(_currentCursorId);
                }

                if (MouseTarget != null)
                {
                    /**
                     * MOUSE OVER
                     * */

                    if (!_hasBeenMouseDown && 0 != MouseTarget.HotControlId)
                    {
                        //GUIUtility.hotControl = MouseTarget.HotControlId;
                        //Debug.Log("GUIUtility.hotControl: " + GUIUtility.hotControl);
                    }

                    MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MOUSE_OVER,
                                                                _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    string cursorStyle = (string) MouseTarget.GetStyle("cursor");
                    if (!string.IsNullOrEmpty(cursorStyle))
                        _currentCursorId = CursorManager.Instance.SetCursor(cursorStyle, CursorPriority.Low);

                    //Debug.Log("_previousMouseOveredComponent: " + _previousMouseOveredComponent);
                    //Debug.Log("_rollOveredComponents.Count: " + _rollOveredComponents.Count);

                    foreach (Component comp in _rollOveredComponents.Keys)
                    {
                        // this is the component subscribed to rollover events

                        _shouldDispatchRollOver = false;
                        _shouldDispatchRollOut = false;

                        /**
                         * 1) Both components are the child of this parent
                         * From parent's point of view, there has been no rollover nor rollout
                         * */
                        if (comp.Contains(MouseTarget, true) &&
                            null != _previousMouseOveredComponent
                            && comp.Contains(_previousMouseOveredComponent, true))
                        {
                            // do nothing
                            continue;
                        }

                        /**
                         * 2) Component child has been mouseovered.
                         * The component has not been in rollovered state.
                         * Dispatch ROLL_OVER.
                         * */
                        if (comp.Contains(MouseTarget, true) && !_rollOveredComponents[comp])
                        {
                            _shouldDispatchRollOver = true;
                            _rollOveredComponentsToChangeState.Add(comp);
                        }

                            /**
                         * 3) Component child has been mouseouted.
                         * New mouseovered component is not a child of this component, and component has been in rollovered state.
                         * Dispatch ROLL_OUT.
                         * */
                        else if (null != _previousMouseOveredComponent &&
                                 comp.Contains(_previousMouseOveredComponent, true) && _rollOveredComponents[comp])
                        {
                            _shouldDispatchRollOut = true;
                            _rollOveredComponentsToChangeState.Add(comp);
                        }

                        // rethink once again
                        // check if there has been a mouse out and no mouse in (blank Stage example)
                        //else if (_hasBeenMouseOut)// && !_shouldDispatchRollOut)
                        //{
                        //    Debug.Log("_hasBeenMouseOut");
                        //    _shouldDispatchRollOut = true;
                        //    _rollOveredComponentsToChangeState.Add(comp);
                        //}

                        if (_shouldDispatchRollOut)
                        {
#if DEBUG
                            if (DebugMode)
                            {
                                Debug.Log("Dispatching ROLL_OUT: " + comp);
                            }
#endif
                            MouseEventHelper.BuildAndDispatchMouseEvent(this, comp, MouseEvent.ROLL_OUT,
                                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
                        }
                        else if (_shouldDispatchRollOver)
                        {
#if DEBUG
                            if (DebugMode)
                            {
                                Debug.Log("Dispatching ROLL_OVER: " + comp);
                            }
#endif
                            MouseEventHelper.BuildAndDispatchMouseEvent(this, comp, MouseEvent.ROLL_OVER,
                                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
                        }
                    }
                }

                    // new mouse target is null
                else if (_hasBeenMouseOut || _isMouseLeave)
                {
                    foreach (Component comp in _rollOveredComponents.Keys)
                    {
                        /**
                         * 
                         * */
                        if (null != _previousMouseOveredComponent
                            && comp.Contains(_previousMouseOveredComponent, true)
                            && _rollOveredComponents[comp])
                        {
#if DEBUG
                            if (DebugMode)
                            {
                                Debug.Log("Dispatching ROLL_OUT (special): " + comp);
                            }
#endif
                            _rollOveredComponentsToChangeState.Add(comp);
                            MouseEventHelper.BuildAndDispatchMouseEvent(this, comp, MouseEvent.ROLL_OUT,
                                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
                        }
                    }
                }
            }

            _rollOveredComponentsToChangeState.ForEach(delegate(Component changing)
                                                           {
                                                               _rollOveredComponents[changing] =
                                                                   !_rollOveredComponents[changing];
                                                           });
            _rollOveredComponentsToChangeState.Clear();

            _previousMouseOveredComponent = MouseTarget;
            _hasBeenMouseOut = false;

            if (null != _mouseLeaveEvent)
            {
                /**
                 * 2) Dispatch from manager
                 * */
                DispatchEvent(_mouseLeaveEvent);
            }
        }

        private void OnMouseDown(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMouseDown");
#endif
            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget == null)
                return;

            _mouseDownComponent = MouseTarget;
#if DEBUG
            if (DebugMode)
                Debug.Log("OnMouseDown component: " + MouseTarget);
#endif
            //if (null != _mouseDownComponent && null != _mouseDownComponent.Uid) // 0 != _mouseDownComponent.HotControlId && GUIUtility.keyboardControl != _mouseDownComponent.HotControlId)
            //{
            //    if (_mouseDownComponent is TextField)
            //    {
            //        //GUIUtility.keyboardControl = _mouseDownComponent.HotControlId;
            //        //Debug.Log("GUIUtility.keyboardControl: " + GUIUtility.keyboardControl);
            //        //GUI.FocusControl(_mouseDownComponent.Uid);
            //    }
            //}

            _hasBeenMouseDown = true;
            MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MOUSE_DOWN,
                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
        }

        private void OnMouseUp(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMouseUp");
#endif

            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget != null)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("OnMouseUp component: " + MouseTarget);
#endif
                MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MOUSE_UP,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                // click processing
                // if previously mouse-downed component is same as mouse-up component, the additional CLICK event should be fired
                // but only if no native click mode is set (button)
                //bool doProcess = true;

                //if (MouseTarget is Button)
                //{
                //    Button b = (Button) MouseTarget;
                //    /**
                //     * Check UnityClickMode
                //     * If UnityClickMode == true, do not process buttons here
                //     * but leave it to Unity
                //     * */
                //    doProcess = b.Enabled && !b.UnityClickMode;
                //}

                if (/*doProcess && */MouseTarget == _mouseDownComponent)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log("MouseEventDispatcher.Click: " + MouseTarget);
#endif

                    MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.CLICK,
                                                                _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    // double click?
                    if (MouseTarget == _lastClickedComponent
                        && DateTime.Now.Subtract(_lastClickedTime).TotalMilliseconds < DoubleClickDelay*1000)
                        MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.DOUBLE_CLICK,
                                                                    _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    _lastClickedComponent = MouseTarget;
                    _lastClickedTime = DateTime.Now;
                }
            }

            //if (_dragging && _mouseDownComponent != null)
            //{
            //    //Debug.Log("Drag stop");
            //    BuildAndDispatchMouseEvent(_mouseDownComponent, MouseEvent.DRAG_DROP, _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
            //}

            _mouseDownComponent = null;
            _rightMouseDownComponent = null;
            _middleMouseDownComponent = null;

            _hasBeenMouseDown = false;
            //CursorLogic.CursorLogic.Show(CursorType.NormalColor);
        }

        private void OnRightMouseDown(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnRightMouseDown");
#endif


            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget == null)
                return;

            _rightMouseDownComponent = MouseTarget;

#if DEBUG
            if (DebugMode)
                Debug.Log("OnRightMouseDown component: " + MouseTarget);
#endif
            MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.RIGHT_MOUSE_DOWN,
                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
        }

        private void OnRightMouseUp(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnRightMouseUp");
#endif

            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget != null)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("OnRightMouseUp component: " + MouseTarget);
#endif
                MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.RIGHT_MOUSE_UP,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                // click processing
                // if previously mouse-downed component is same as mouse-up component, the additional CLICK event should be fired
                // but only if no native click mode is set (button)
                //bool doProcess = true;

                //if (MouseTarget is Button)
                //{
                //    Button b = (Button) MouseTarget;
                //    /**
                //     * Check UnityClickMode
                //     * If UnityClickMode == true, do not process buttons here
                //     * but leave it to Unity
                //     * */
                //    doProcess = b.Enabled && !b.UnityClickMode;
                //}

                if (/*doProcess && */MouseTarget == _rightMouseDownComponent)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log("MouseEventDispatcher.RightClick: " + MouseTarget);
#endif
                    MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.RIGHT_CLICK,
                                                                _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    // right double click?
                    if (MouseTarget == _lastClickedComponent
                        && DateTime.Now.Subtract(_lastRightClickedTime).TotalMilliseconds < RightDoubleClickDelay * 1000)
                        MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.RIGHT_DOUBLE_CLICK,
                                                                    _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    _lastClickedComponent = MouseTarget;
                    _lastRightClickedTime = DateTime.Now;
                }
            }

            _mouseDownComponent = null;
            _rightMouseDownComponent = null;
            _middleMouseDownComponent = null;
        }

        private void OnMiddleMouseDown(Event e)
        {

#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMiddleMouseDown");
#endif


            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget == null)
                return;

            _middleMouseDownComponent = MouseTarget;

#if DEBUG
            if (DebugMode)
                Debug.Log("OnMiddleMouseDown component: " + MouseTarget);
#endif
            MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MIDDLE_MOUSE_DOWN,
                                                        _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
        }

        private void OnMiddleMouseUp(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMiddleMouseUp");
#endif

            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (MouseTarget != null)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("OnMiddleMouseUp component: " + MouseTarget);
#endif

                MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MIDDLE_MOUSE_UP,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                // click processing
                // if previously mouse-downed component is same as mouse-up component, the additional CLICK event should be fired
                // but only if no native click mode is set (button)
                //bool doProcess = true;

                //if (MouseTarget is Button)
                //{
                //    Button b = (Button) MouseTarget;
                //    /**
                //     * Check UnityClickMode
                //     * If UnityClickMode == true, do not process buttons here
                //     * but leave it to Unity
                //     * */
                //    doProcess = b.Enabled && !b.UnityClickMode;
                //}

                if (/*doProcess && */MouseTarget == _middleMouseDownComponent)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log("MouseEventDispatcher.Click: " + MouseTarget);
#endif
                    MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MIDDLE_CLICK,
                                                                _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    // middle double click?
                    if (MouseTarget == _lastClickedComponent
                        && DateTime.Now.Subtract(_lastMiddleClickedTime).TotalMilliseconds < RightDoubleClickDelay * 1000)
                        MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MIDDLE_DOUBLE_CLICK,
                                                                    _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

                    _lastClickedComponent = MouseTarget;
                    _lastMiddleClickedTime = DateTime.Now;
                }
            }

            _mouseDownComponent = null;
            _rightMouseDownComponent = null;
            _middleMouseDownComponent = null;
        }

        private void OnMouseDrag(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMouseDrag");
#endif

            _mouseEvent = (MouseEvent) e;

            RecalculateMouseTarget();

            if (null != MouseTarget)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("OnMouseDrag component: " + MouseTarget);
#endif
                MouseEventHelper.BuildAndDispatchMouseEvent(this, MouseTarget, MouseEvent.MOUSE_DRAG,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);
            }
        }

        //private float _spX;
        //private float _spY;

        private void OnMouseWheel(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("MouseEventDispatcher.OnMouseWheel: " + e);
#endif
            _mouseEvent = (MouseEvent) e;

            RecalculateMouseWheelTargets();

            //UnityEngine.Event.current.delta.y

            //Debug.Log("UnityEngine.Event.current.delta: " + UnityEngine.Event.current.delta);
            //Point amount = new Point(UnityEngine.Event.current.delta.x, UnityEngine.Event.current.delta.y);

#if DEBUG
            if (DebugMode)
            {
                if (_mouseWheelTargets.Count > 0)
                {
                    Debug.Log(string.Format(@"Mouse wheel targets ({0}): 
{1}", _mouseWheelTargets.Count, ComponentUtil.DescribeComponentList(_mouseWheelTargets)));
                }
            }
#endif
            //var deltaY = UnityEngine.Event.current.delta.y;

            //foreach (Component mouseWheelTarget in _mouseWheelTargets)
            //{
            //    //if (mouseWheelTarget.StopMouseWheelPropagation)
            //    //    break;

            //    //IScrollable sc = mouseWheelTarget as IScrollable;
            //    //if (null != sc)
            //    //{
            //    //    if (sc.MouseWheelStep == 0)
            //    //        throw new Exception("MouseWheelStep of IScrollable should be greater than 0");

            //    //    //Debug.Log("!!!");
            //    //    var pixels = deltaY * sc.MouseWheelStep;
            //    //    Debug.Log("pixels: " + pixels);

            //    //    var oldVScroll = sc.VerticalScrollPosition;
            //    //    sc.VerticalScrollPosition += pixels;
            //    //    var diff = sc.VerticalScrollPosition - oldVScroll;

            //    //    var res = pixels - diff;

            //    //    //Debug.Log("diff: " + diff);
            //    //    //Debug.Log("oldVScroll: " + oldVScroll);
            //    //    //Debug.Log("sc.VerticalScrollPosition: " + sc.VerticalScrollPosition);
            //    //    //Debug.Log("res: " + res);

            //    //    //amount = amount.Divide(sc.MouseWheelStep);
            //    //    //Debug.Log("amount: " + amount);
            //    //    //if (amount.Equals(Point.Zero)) // no residuum, exit
            //    //    if (res == 0)
            //    //        break;

            //    //    deltaY = res / pixels * deltaY;
            //    //}
            //}

//            if (MouseWheelTarget == null)
//                return;
//#if DEBUG
//            if (DebugMode)
//                Debug.Log("OnMouseWheel component: " + MouseWheelTarget);
//#endif
            //IScrollable scrollable = MouseWheelTarget as IScrollable;
            //Container container = scrollable as Container;

            //Debug.Log(container);
            ////Debug.Log("    -> scrollable: " + scrollable);
            ////Debug.Log("    -> QScrollContent: " + container.QScrollContent);
            ////Debug.Log("    -> ScrollContent: " + container.ScrollContent);
            //if (null != scrollable) // ScrollContent?
            //{
            //    //bool sc = null != container ? container.QScrollContent : scrollable.ScrollContent; // commented out 20130410
            //    bool sc = scrollable.ScrollContent; // commented out 20130410
            //    Debug.Log("    -> sc: " + sc);
            //    //bool sc = scrollable.ScrollContent;
            //    if (sc)
            //    {
            //        Debug.Log(sc);
            //        var delta = UnityEngine.Event.current.delta;
            //        if (delta.x != 0 || delta.y != 0)
            //        {
            //            _spX = scrollable.ScrollPosition.X;
            //            float d = delta.y*scrollable.MouseWheelStep;
            //            Debug.Log("Delta: " + d);
            //            Debug.Log("scrollable.ScrollPosition.Y: " + scrollable.ScrollPosition.Y);
            //            _spY = scrollable.ScrollPosition.Y + d;
            //            Debug.Log("_spY 2: " + _spY);
            //            scrollable.ScrollPosition = new Point(_spX, _spY);
            //        }

            //        //scrollable.ScrollPosition = new Point(0, 100);
            //    }
            //}

            /**
             * Dispatch the mouse wheel event on the top component
             * Other component could receive the event via the event bubbling
             * */
            if (_mouseWheelTargets.Count > 0)
                MouseEventHelper.BuildAndDispatchMouseEvent(this, _mouseWheelTargets[0], MouseEvent.MOUSE_WHEEL,
                                                            _mouseEvent.GlobalPosition, _mouseEvent.CurrentEvent);

            /**
             * Stuff could have moved, so we need recalculating // 20120520
             * TODO: perhaps this needs to be defered?
             * */
            RecalculateMouseTarget();
            RecalculateMouseWheelTargets();
        }

        #endregion

        #region Roll over / roll out

        private readonly Dictionary<Component, bool> _rollOveredComponents = new Dictionary<Component, bool>();

        /// <summary>
        /// Registers rollover component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="isOver"></param>
        public void RegisterRollOverComponent(Component component, bool isOver)
        {
            if (!_rollOveredComponents.ContainsKey(component))
            {
                _rollOveredComponents[component] = isOver;
            }
        }

        /// <summary>
        /// Registers rollover component
        /// </summary>
        /// <param name="component"></param>
        public void RegisterRollOverComponent(Component component)
        {
            RegisterRollOverComponent(component, false);
        }

        /// <summary>
        /// Unregisters rollover component
        /// </summary>
        /// <param name="component"></param>
        public void UnregisterRollOverComponent(Component component)
        {
            if (_rollOveredComponents.ContainsKey(component))
            {
                _rollOveredComponents.Remove(component);
            }
        }

        #endregion

        #region Helper

// ReSharper disable MemberCanBePrivate.Global
        /// <summary>
        /// Finds the component under the mouse poiner
        /// </summary>
        public void RecalculateMouseTarget()
// ReSharper restore MemberCanBePrivate.Global
        {
            if (_isMouseLeave)
            {
                MouseTarget = null;
                return;
            }

            MouseTarget = CoordinateProcessor.GetComponentUnderCoordinatesOnAllStages(
                _mouseEvent.GlobalPosition,
                MouseTargetFilter,
                true, // stopOnDisabled
                true // stopOnInvisible
            ) as Component;
            //Debug.Log("MouseTarget: " + ComponentUtil.PathToString(MouseTarget, "->"));
        }

        private Component _oldInspectorTarget;

        /// <summary>
        /// Recalculates the inspector target
        /// </summary>
        private void RecalculateInspectorTarget()
        {
            //Debug.Log("RecalculateInspectorTarget");
            _oldInspectorTarget = InspectorTarget;

            if (_isMouseLeave)
            {
                //Debug.Log("   -> InspectorTarget: mouse leave");
                InspectorMouseOutSignal.Emit(InspectorTarget);
                InspectorMouseLeaveSignal.Emit();
                InspectorTarget = null;
                return;
            }

            InspectorTarget = CoordinateProcessor.GetComponentUnderCoordinatesOnAllStages(
                _mouseEvent.GlobalPosition,
                InspectorTargetFilter,
                !SystemManager.Instance.ShiftKeyPressed, // don't stopOnDisabled
                !SystemManager.Instance.AltKeyPressed  // don't stopOnInvisible
            ) as Component;

            // emit signals
            if (_oldInspectorTarget != InspectorTarget) {
                //Debug.Log("   -> InspectorTarget: " + InspectorTarget);
                //Debug.Log(3);
                InspectorMouseOutSignal.Emit(_oldInspectorTarget);
                InspectorMouseOverSignal.Emit(InspectorTarget);
            }
        }

        /// <summary>
        /// Finds the Component stack under the mouse poiner
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public void RecalculateMouseWheelTargets()
// ReSharper restore MemberCanBePrivate.Global
        {
            //Debug.Log("RecalculateMouseWheelTargets: " + deltaY);
            if (_isMouseLeave)
            {
                _mouseWheelTargets.Clear();
                return;
            }

            var members = CoordinateProcessor.GetComponentStackUnderCoordinatesOnAllStages(
                _mouseEvent.GlobalPosition,
                MouseWheelFilter, 
                true, true // stopOnDisabled, stopOnInvisible
            );

            _mouseWheelTargets = new List<Component>();

            foreach (DisplayListMember member in members)
            {
                _mouseWheelTargets.Add((Component)member);
            }

            // Important: reverse targets
            //_mouseWheelTargets.Reverse();

//            Debug.Log(string.Format(@"Mouse wheel targets ({0}): 
//{1}", _mouseWheelTargets.Count, ComponentUtil.DescribeComponentList(_mouseWheelTargets)));

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Mouse wheel targets ({0}): 
{1}", _mouseWheelTargets.Count, ComponentUtil.DescribeComponentList(_mouseWheelTargets)));
            }
#endif
        }

        /// <summary>
        /// The function that filters out potential mouse targets that must be Components
        /// </summary>
        /// <param name="dlm"></param>
        /// <returns></returns>
        private static bool MouseTargetFilter(DisplayListMember dlm)
        {
            var component = dlm as Component;
            if (null == component)
                return false;

            // get visible and mouse enabled COMPONENTS
            return component.Visible && component.MouseEnabled;
        }

        /// <summary>
        /// The function that filters out potential inspector targets that must be Components
        /// </summary>
        /// <param name="dlm"></param>
        /// <returns></returns>
        private static bool InspectorTargetFilter(DisplayListMember dlm)
        {
            var component = dlm as Component;
            if (null == component)
                return false;

            if (!SystemManager.Instance.AltKeyPressed) {
                return component.Visible && component.MouseEnabled;
            }

            // if Alt key pressed, do show both mouse enabled and mouse disabled components

            if (component is Stage)
            {
                // alow clicking the bottom stage event if there is another stage on top
                // so alow stages to be turned on/off with MouseEnabled as a switch
                return component.Visible && component.MouseEnabled;
            }
            
            return dlm.Visible;
        }

        /// <summary>
        /// The function that filters out only scroll-enabled Components
        /// </summary>
        /// <param name="dlm"></param>
        /// <returns></returns>
        private static bool MouseWheelFilter(DisplayListMember dlm)
        {
            //Debug.Log("MouseWheelFilter: " + dlm);

            var component = dlm as Component;
            if (null == component)
                return false;

            /**
             * 1. not a candidate - return
             * */
            if (!component.MouseEnabled) // this blocks the Group from mouse wheeling
                return false;

            /**
             * 2. not a candidate - return
             * */
            if (!component.Visible) // || !component.MouseEnabled)
                return false;

            ///**
            // * 2. forced mouse wheel target
            // * */
            //if (component.StopMouseWheelPropagation)
            //    return true;

            //Debug.Log("  -> passed");

            return true;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        override public void Dispose()
        {
            base.Dispose();

            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_DRAG, OnMouseDrag);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_DOWN, OnMouseDown);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, OnMouseUp);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.RIGHT_MOUSE_DOWN, OnRightMouseDown);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.RIGHT_MOUSE_UP, OnRightMouseUp);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MIDDLE_MOUSE_DOWN, OnMiddleMouseDown);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MIDDLE_MOUSE_UP, OnMiddleMouseUp);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_WHEEL, OnMouseWheel);
        }

        #endregion
    }
}