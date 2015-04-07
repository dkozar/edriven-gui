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
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// The manager responsible for handling key events on FOCUSED components
    /// </summary>
    public class KeyEventDispatcher : EventDispatcher
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static KeyEventDispatcher _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private KeyEventDispatcher()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static KeyEventDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating KeyEventDispatcher instance"));
#endif
                    _instance = new KeyEventDispatcher();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region Members

        private KeyboardEvent _keyEvent;

        #endregion

        #region Methods

        /// <summary>
        /// Subscription to SystemManager
        /// </summary>
        private void Initialize()
        {
            SystemEventDispatcher.Instance.AddEventListener(KeyboardEvent.KEY_DOWN, OnKeyDown);
            SystemEventDispatcher.Instance.AddEventListener(KeyboardEvent.KEY_UP, OnKeyUp);

#if TRIAL
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        #endregion

        #region Key events

        private void OnKeyDown(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("KeyEventDispatcher:OnKeyDown");
#endif
            InteractiveComponent comp = FocusManager.Instance.FocusedComponent;

            if (null == comp || !comp.PassesKeyDownFilter(e))
                return;

            e.Bubbles = true;

            RedispatchKeyEvent(comp, e);
        }

        private void OnKeyUp(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("KeyEventDispatcher:OnKeyUp");
#endif

            InteractiveComponent comp = FocusManager.Instance.FocusedComponent;

            if (null == comp || !comp.PassesKeyUpFilter(e))
                return;

            e.Bubbles = true;

            RedispatchKeyEvent(comp, e);
        }

        #endregion

        #region Helper

        private void RedispatchKeyEvent(IEventDispatcher targetComponent, ICloneable systemManagerKeyEvent)
        {
            _keyEvent = (KeyboardEvent)systemManagerKeyEvent.Clone();
            _keyEvent.Target = targetComponent;
            
            /**
             * 1) Dispatch from here
             * */
            DispatchEvent(_keyEvent);

            // the event might be canceled
            if (_keyEvent.Canceled)
                return;

            /**
             * 2) Dispatch from the component
             * */
            targetComponent.DispatchEvent(_keyEvent);
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

            SystemEventDispatcher.Instance.RemoveEventListener(KeyboardEvent.KEY_DOWN, OnKeyDown);
            SystemEventDispatcher.Instance.RemoveEventListener(KeyboardEvent.KEY_UP, OnKeyUp);
        }

        #endregion
    }
}