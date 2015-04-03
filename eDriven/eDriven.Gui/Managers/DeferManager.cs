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

using System.Collections.Generic;
using eDriven.Core;
using eDriven.Core.Managers;
using eDriven.Gui.Check;
using UnityEngine;

namespace eDriven.Gui.Managers
{
    public class DeferManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static DeferManager _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DeferManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DeferManager Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DeferManager instance"));
#endif
                    _instance = new DeferManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("Initializing DeferManager"));
#endif
            //_updateSlot = new DelayManagerUpdateSlot(this);
            //_inputSlot = new DelayManagerInputSlot(this);

#if RELEASE
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }
        
        //private ISlot _updateSlot;
        //private ISlot _inputSlot;

        public delegate void DelayedAction();

        private class DeferredActionDescriptor
        {
            internal readonly DelayedAction Action;
            internal int Counter;

            public DeferredActionDescriptor(DelayedAction action, int counter)
            {
                Action = action;
                Counter = counter;
            }
        }

        private readonly List<DeferredActionDescriptor> _updateActions = new List<DeferredActionDescriptor>();
        private readonly List<DeferredActionDescriptor> _inputActions = new List<DeferredActionDescriptor>();

        public void Defer(DelayedAction action, int frames, SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.Update:
                    _updateActions.Add(new DeferredActionDescriptor(action, frames));
                    SystemManager.Instance.UpdateSignal.Connect(UpdateSlot);
                    break;
                case SubscriptionType.Input:
                    _inputActions.Add(new DeferredActionDescriptor(action, frames));
                    SystemManager.Instance.RenderSignal.Connect(RenderSlot);
                    break;
            }
        }

        public virtual void Defer(DelayedAction action, SubscriptionType subscriptionType)
        {
            Defer(action, 1, subscriptionType);
        }

        public virtual void Defer(DelayedAction action, int frames)
        {
            Defer(action, frames, SubscriptionType.Update);
        }

        public virtual void Defer(DelayedAction action)
        {
            Defer(action, SubscriptionType.Update);
        }

        readonly List<DeferredActionDescriptor> _toFire = new List<DeferredActionDescriptor>();

        internal void ProcessUpdate()
        {
            _updateActions.ForEach(delegate(DeferredActionDescriptor descriptor)
            {
                //Debug.Log("checking: " + descriptor.Counter);
                descriptor.Counter--;
                if (descriptor.Counter <= 0)
                    _toFire.Add(descriptor);
            });

            _toFire.ForEach(delegate(DeferredActionDescriptor descriptor)
            {
                descriptor.Action(); // fire
                _updateActions.Remove(descriptor); // remove
            });

            _toFire.Clear();

            if (_updateActions.Count == 0)
                SystemManager.Instance.UpdateSignal.Disconnect(UpdateSlot);
        }

        internal void ProcessInput()
        {
            _inputActions.ForEach(delegate(DeferredActionDescriptor descriptor)
            {
                descriptor.Counter--;
                if (descriptor.Counter <= 0)
                    _toFire.Add(descriptor);
            });

            _toFire.ForEach(delegate(DeferredActionDescriptor descriptor)
            {
                descriptor.Action(); // fire
                _inputActions.Remove(descriptor); // remove
            });

            _toFire.Clear();

            if (_inputActions.Count == 0)
                SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
        }

        #region Slots

        public void UpdateSlot(params object[] parameters)
        {
            //Debug.Log("Update received");
            ProcessUpdate();
        }

        public void RenderSlot(params object[] parameters)
        {
            //Debug.Log("Input received");
            ProcessInput();
        }

        #endregion

    }

    public enum SubscriptionType
    {
        Update, Input
    }

    //internal class DelayManagerUpdateSlot : ISlot
    //{
    //    private readonly DeferManager _manager;

    //    public DelayManagerUpdateSlot(DeferManager manager)
    //    {
    //        _manager = manager;
    //    }

    //    #region Implementation of ISlot

    //    public void UpdateSlot(params object[] parameters)
    //    {
    //        //Debug.Log("Update received");
    //        _manager.ProcessUpdate();
    //    }

    //    #endregion
    //}

    //internal class DelayManagerInputSlot : ISlot
    //{
    //    private readonly DeferManager _manager;

    //    public DelayManagerInputSlot(DeferManager manager)
    //    {
    //        _manager = manager;
    //    }

    //    #region Implementation of ISlot

    //    public void RenderSlot(params object[] parameters)
    //    {
    //        //Debug.Log("Input received");
    //        _manager.ProcessInput();
    //    }

    //    #endregion
    //}
}
