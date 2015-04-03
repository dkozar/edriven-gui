using System;
using eDriven.Core.Events;
using eDriven.Gui.Animation;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// This class is a driver for loading mask
    /// I made this to allow for another types of progress indicators, 
    /// which should not only be styleable, but completely customizable.
    /// The type parameter indicates the mask used
    /// The mask must subclass ProgressIndicatorBase, i.e. Message { get; set; }, Play(), Stop();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoadingMaskBase<T> : ILoadingMask where T : ProgressIndicatorBase, new()
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Members

        private DisplayListMember _component;
        private ProgressIndicatorBase _maskGraphics;
        private DisplayObjectContainer _parent;

        #endregion

        //public Type Animator = typeof(ProgressIndicatorBase);

        #region Constructor

        public LoadingMaskBase()
        {
        }

        public LoadingMaskBase(DisplayListMember component)
        {
            Mask(component);
        }

        public LoadingMaskBase(DisplayListMember component, string message)
        {
            Mask(component);
            SetMessage(message);
        }

        #endregion

        #region Implementation of ILoadingMask

        /// <summary>
        /// Shows the mask
        /// </summary>
        /// <param name="component"></param>
        public void Mask(DisplayListMember component)
        {
            _component = component;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Masking component: " + component);
            }
#endif
            if (null != _maskGraphics)
                return; // already masking this component

            _parent = _component.Parent ?? (_component is Stage ? _component as Stage : null);

            if (null == _parent)
                return; // we are not on the display list, so we have nothing to mask indeed

            var imc = _component as InvalidationManagerClient;

            _maskGraphics = new T
                                {
                                    IncludeInLayout = false,
                                    X = _component.X,
                                    Y = _component.Y,
                                    Width = null != imc ? imc.GetExplicitOrMeasuredWidth() : _component.Width,
                                    Height = null != imc ? imc.GetExplicitOrMeasuredHeight() : _component.Height
                                    //Bounds = (Rectangle)_component.Bounds.Clone() // NOTE: BEWARE! This was the reference bug (without Clone())!!!!!!!!!
                                };

            _parent.AddChild(_maskGraphics);
            //_maskGraphics.ValidateNow(); // commented out 20130331 and moved to LoadingMaskAnimator

            // critical!
            //_maskGraphics.Transform.Apply(); // TODO: remove
            //_maskGraphics.Parent.Transform.ValidateChild(_maskGraphics);
            _maskGraphics.InvalidateTransform();

            // subscribe to MOVE and RESIZE events of the component
            // we shall be levitating just over the component
            _component.AddEventListener(MoveEvent.MOVE, MoveHandler, EventPhase.Target);
            _component.AddEventListener(ResizeEvent.RESIZE, ResizeHandler, EventPhase.Target);

            _maskGraphics.Play();
        }

        /// <summary>
        /// Updates the mask message
        /// </summary>
        public void SetMessage(string message)
        {
            if (null == _component)
                throw new Exception("No component is being masked using this mask");

            if (null != _maskGraphics)
                _maskGraphics.Message = message;
        }

        /// <summary>
        /// Hides the mask
        /// </summary>
        public void Unmask()
        {
            if (null == _maskGraphics)
                return;

            _maskGraphics.Stop();

            _component.RemoveEventListener(MoveEvent.MOVE, MoveHandler, EventPhase.Target);
            _component.RemoveEventListener(ResizeEvent.RESIZE, ResizeHandler, EventPhase.Target);

            if (null != _parent)
                _parent.RemoveChild(_maskGraphics);

            _maskGraphics.Dispose();
            _maskGraphics = null;
        }

        #endregion

        #region Handlers

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

        #endregion

        #region IDisposable

        public void Dispose()
        {
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