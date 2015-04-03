using System;
using System.Collections.Generic;
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Check;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// Singleton class for handling drag
    /// Coded by Danko Kozar
    /// </summary>
    public class DragManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        private static bool _isDragging;
        /// <summary>
        /// When freezed, no hover styles should be shown
        /// Note: This manager only hosts this static property
        /// </summary>
        public static bool IsDragging
        {
            get
            {
                return _isDragging;
            }
            set
            {
                if (value == _isDragging)
                    return;

                _isDragging = value;
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("IsDragging changed to: " + _isDragging);
                }
#endif
            }
        }

        #region Singleton

        private static DragManager _instance;

        private DragManager()
        {
            // Constructor is protected!
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DragManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DragManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private List<Component> _dragTargets;

        private Point _lastPos;

        public bool StopAllDragsOnMouseUp; // = true;

        /// <summary>
        /// Initialization routine
        /// Put inside the initialization stuff, if needed
        /// </summary>
        private void Initialize()
        {
            _dragTargets = new List<Component>();

            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DOWN, OnMouseDown);

#if RELEASE
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        private void OnMouseDown(Event e)
        {
            //Debug.Log("OnMouseDown");
            MouseEvent me = (MouseEvent)e;
            _lastPos = me.GlobalPosition;

            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);
            SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_UP, OnMouseUp);
        }

        private void OnMouseMove(Event e)
        {
            //Debug.Log("OnMouseMove");
            MouseEvent me = (MouseEvent)e;
            
            if (null == _lastPos) {
                //Debug.Log("was null");
                _lastPos = me.GlobalPosition;
            }

            Point delta = me.GlobalPosition.Subtract(_lastPos);

            //Debug.Log(string.Format("### OnMouseMove ### last: {0}; now: {1}; delta: {2}", _lastPos, me.GlobalPosition, delta));

            //Debug.Log("       delta: " + delta);

            _lastPos = me.GlobalPosition;

            foreach (Component target in _dragTargets)
            {
                //target.Bounds = new Rectangle(target.X + delta.X, target.Y + delta.Y, target.Width, target.Height);
                target.Move(-100, -100);
                target.SetActualSize(10, 10);

                //target.X += delta.X;
                //target.Y += delta.Y;
                target.InvalidateTransform();
                target.ValidateTransform();
            }
        }

        private void OnMouseUp(Event e)
        {
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, OnMouseUp);

            _lastPos = null;
            if (StopAllDragsOnMouseUp)
                Reset();
        }

        public void StartDrag(params Component[] targets)
        {
            //Debug.Log("### StartDrag ###");
            _dragTargets.AddRange(targets);
        }

        public void StopDrag(params Component[] targets)
        {
            //Debug.Log("### StopDrag ###");
            List<Component> toRemove = new List<Component>(targets);
            _dragTargets.RemoveAll(delegate(Component t)
            {
                return toRemove.Contains(t);
            });
        }

        public void Reset()
        {
            _dragTargets.Clear();
        }
    }

    public class DragManagerException : Exception
    {

        public const string ComponentNotInFocus = "Component not in focus, so cannot be blurred";
        
        public DragManagerException()
        {

        }

        public DragManagerException(string message)
            : base(message)
        {

        }
    }
}