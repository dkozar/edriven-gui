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

using eDriven.Core.Managers;
using eDriven.Core.Signals;
using UnityEngine;

namespace eDriven.Gui.Designer
{
    public class DesignerState
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static DesignerState _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DesignerState()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DesignerState Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DesignerState instance"));
#endif
                    _instance = new DesignerState();
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

        }

        private static bool _isPlaying;
        /// <summary>
        /// Using this property we are decouplilng from UnityEditor
        /// This assembly doesn't have to reference it
        /// The important thing is that when play mode is started/stopped, this value should be set
        /// This is currently done from PlayModeStateChangeProcessor
        /// </summary>
        public static bool IsPlaying
        { 
            get
            {
                return _isPlaying;
            }
            set
            {
                if (value == _isPlaying)
                    return;

                _isPlaying = value;
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("IsPlaying changed to: " + _isPlaying);
                }
#endif
            }
        }

        private readonly Signal _componentRemovedSignal = new Signal();
        /// <summary>
        /// Emits when the component is removed
        /// </summary>
        public Signal ComponentRemovedSignal
        {
            get { return _componentRemovedSignal; }
        }

        private bool _componentRemoved;
        internal void ComponentRemoved()
        {
            if (!_componentRemoved)
            {
                _componentRemoved = true;
                ComponentRemovedSignal.Emit();
                SystemManager.Instance.UpdateSignal.Connect(UpdateSlot, true); // auto disconnect
            }
        }

        private void UpdateSlot(object[] parameters)
        {
            _componentRemoved = false;
        }
    }
}
