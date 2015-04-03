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
using eDriven.Core.Managers;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// Registers components on stage and alows easy access by component ID
    /// </summary>
    public class ComponentManager
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static ComponentManager _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private ComponentManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ComponentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ComponentManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private Dictionary<string, DisplayListMember> _systemDict;
        private Dictionary<string, InteractiveComponent> _customDict;

        /// <summary>
        /// Initialization routine
        /// Put inside initialization stuff, if needed
        /// </summary>
        internal void Initialize()
        {
            _systemDict = new Dictionary<string, DisplayListMember>();
            _customDict = new Dictionary<string, InteractiveComponent>();

            SystemManager.Instance.DisposingSignal.Connect(DisposingSlot, true);

#if RELEASE
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        private static void DisposingSlot(object[] parameters)
        {
            _instance = null;
        }

        #region Internal registration

        //private int _uid;

        internal void RegisterInternal(ref string uid, DisplayListMember component)
        {
            if (null == uid)
                //uid = _uid++.ToString();
                uid = NamingUtil.CreateUid(component); //_uid++.ToString();

            //Debug.Log(uid);

            if (_systemDict.ContainsKey(uid))
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(ComponentManagerException.IdAlreadyRegistered, uid));
                    Debug.Break();
                }
#endif
                return;
                //TODO: Examine this error
                //throw new ComponentManagerException(string.Format(ComponentManagerException.IdAlreadyRegistered, uid));
            }

            _systemDict.Add(uid, component);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Component registered [INTERNAL]: [{0}] => {1}", uid, component));
            }
#endif
        }

        internal bool IsRegisteredInternal(DisplayListMember component)
        {
            return _systemDict.ContainsValue(component);
        }

        public DisplayListMember GetInternal(string uid)
        {
            if (!_systemDict.ContainsKey(uid))
                throw new ComponentManagerException(string.Format(ComponentManagerException.IdNotRegistered, uid));

            return _systemDict[uid];
        }

        #endregion

        #region Custom registration

        internal void Register(/*ref */string id, InteractiveComponent component)
        {
            //if (null == id)
            //    throw new ComponentManagerException(ComponentManagerException.CannotRegisterNull);

            if (string.IsNullOrEmpty(id))
                return; // do not register an empty ID

            if (_customDict.ContainsKey(id)) 
                return; // no multiple registration

            if (_customDict.ContainsKey(id))
            {
#if DEBUG
if (DebugMode)
                {
                    Debug.Log(string.Format(ComponentManagerException.IdAlreadyRegistered, id));
                    Debug.Break();
                }
#endif

                throw new ComponentManagerException(string.Format(ComponentManagerException.IdAlreadyRegistered, id));
            }

            _customDict.Add(id, component);

#if DEBUG
if (DebugMode)
            {
                Debug.Log(string.Format(@"Component registered: [{0}] => {1}", id, component));
            }
#endif

        }

        public bool IsRegistered(InteractiveComponent component)
        {
            return _customDict.ContainsValue(component);
        }

        public InteractiveComponent Get(string id)
        {
            if (!_customDict.ContainsKey(id))
                throw new ComponentManagerException(string.Format(ComponentManagerException.IdNotRegistered, id));

            return _customDict[id];
        }

        #endregion

        // ReSharper disable UnusedMember.Global
        public InteractiveComponent Get(string id, bool suppressError)
            // ReSharper restore UnusedMember.Global
        {
            if (suppressError)
            {
                if (!_systemDict.ContainsKey(id))
                    return null;
            }

            return Get(id);
        }

        public void Clear()
        {
            _systemDict.Clear();
            _customDict.Clear();
        }
    }

    public class ComponentManagerException : Exception
    {
        public const string CannotRegisterNull = "Cannot register null as component";
        public const string IdAlreadyRegistered = "Component with Id [{0}] already registered";
        public const string IdNotRegistered = "Component with Id [{0}] not registered";

// ReSharper disable UnusedMember.Global
        public ComponentManagerException()
// ReSharper restore UnusedMember.Global
        {
        }

        public ComponentManagerException(string message)
            : base(message)
        {
        }
    }
}