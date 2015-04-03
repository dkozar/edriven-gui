using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    public class LoadingMaskPlugin : IPlugin
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        private Component _component;

        private LoadingMaskAnimator _maskGraphics;

        private DisplayObjectContainer _parent;

        //public static string AnimationId = "progress";

        //public string AnimationId;

        #region Implementation of IPlugin

        private bool _initialized;

        public void Initialize(InvalidationManagerClient component)
        {
            if (_initialized)
                return;

            _initialized = true;
            _component = (Component) component;

            // subscribe to TARGET PHASE component events
            _component.AddEventListener(LoadingEvent.START, StartHandler, EventPhase.Target);
            _component.AddEventListener(LoadingEvent.PROGRESS, ProgressHandler, EventPhase.Target);
            _component.AddEventListener(LoadingEvent.END, EndHandler, EventPhase.Target);
            _component.AddEventListener(LoadingEvent.ERROR, ErrorHandler, EventPhase.Target);

            //if (null == AnimationId)
            //    AnimationId = AnimationId;
        }

        #endregion

        private void StartHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("StartHandler " + _component.Width + ", " + _component.Height);
            }
#endif
            if (null != _maskGraphics)
                return; // already masking this component

            _parent = _component.Parent ?? (_component is Stage ? _component : null);

            if (null == _parent)
                return; // we are not on the display list, so we have nothing to mask indeed

            _maskGraphics = new LoadingMaskAnimator {
                IncludeInLayout = false, 
                X = _component.X, 
                Y = _component.Y, 
                Width = _component.Width, 
                Height = _component.Height,
                //Bounds = (Rectangle) _component.Bounds.Clone() // BEWARE! This was the reference bug (without Clone())!!!!!!!!!
            };

            _parent.AddChild(_maskGraphics);

            LoadingEvent le = e as LoadingEvent;
            if (null != le)
                _maskGraphics.Message = le.Message;

            // critical!
            //_maskGraphics.Transform.Apply();
            _maskGraphics.InvalidateTransform();

            // subscribe to MOVE and RESIZE events of the component
            // we shall be levitating just over the component
            _component.AddEventListener(MoveEvent.MOVE, MoveHandler, EventPhase.Target);
            _component.AddEventListener(ResizeEvent.RESIZE, ResizeHandler, EventPhase.Target);

            _maskGraphics.Play();
        }

        private void MoveHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("MOVE *** " + _component);
            }
#endif
            //_mask.Bounds = (Rectangle) _component.Bounds.Clone();
            // NOTE: the following works only if parent is container (it doesn't work when masking components) (validation calls UpdatePositionInternals()) ?Or does it?
            _maskGraphics.X = _component.X;
            _maskGraphics.Y = _component.Y;
        }

        private void ResizeHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("RESIZE *** " + _component);
            }
#endif
            _maskGraphics.Width = _component.Width;
            _maskGraphics.Height = _component.Height;
        }

        private void ProgressHandler(Event e)
        {
            LoadingEvent le = (LoadingEvent)e;
            if (null != _maskGraphics)
                _maskGraphics.Message = le.Message;
        }

        private void EndHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("EndHandler");
            }
#endif
            Cleanup();
        }

        private void ErrorHandler(Event e)
        {
            // TODO: let the mask display the error for the predefined time, and than dissapear!

            //LoadingEvent le = e as LoadingEvent;
            //if (null != le)
            //    _mask.Message = le.Message;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("ErrorHandler");
            }
#endif
            Cleanup();
        }

        #region IDisposable

        public void Dispose()
        {
            _component.RemoveEventListener(LoadingEvent.START, StartHandler, EventPhase.Target);
            _component.RemoveEventListener(LoadingEvent.PROGRESS, ProgressHandler, EventPhase.Target);
            _component.RemoveEventListener(LoadingEvent.END, EndHandler, EventPhase.Target);
            _component.RemoveEventListener(LoadingEvent.ERROR, ErrorHandler, EventPhase.Target);

            Cleanup();
        }

        private void Cleanup()
        {
            _maskGraphics.Stop();
            
            _component.RemoveEventListener(MoveEvent.MOVE, MoveHandler, EventPhase.Target);
            _component.RemoveEventListener(ResizeEvent.RESIZE, ResizeHandler, EventPhase.Target);

            if (null != _parent)
                _parent.RemoveChild(_maskGraphics);
            
            _maskGraphics = null;
        }

        #endregion

    }
}