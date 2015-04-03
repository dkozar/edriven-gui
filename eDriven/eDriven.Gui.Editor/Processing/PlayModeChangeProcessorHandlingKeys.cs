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
using eDriven.Gui.Designer.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    /// <summary>
    /// Subscribes to play mode start/stop
    /// Adds and removes the keyboard listener for editor keys active in play mode
    /// </summary>
    internal class PlayModeChangeProcessorHandlingKeys
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PlayModeChangeProcessorHandlingKeys _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PlayModeChangeProcessorHandlingKeys()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PlayModeChangeProcessorHandlingKeys Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PlayModeChangeProcessorHandlingKeys instance"));
#endif
                    _instance = new PlayModeChangeProcessorHandlingKeys();
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
            var p = PlayModeStateChangeEmitter.Instance;
            p.PlayModeStartedSignal.Connect(PlayModeStartedSlot);
            p.PlayModeStoppedSignal.Connect(PlayModeStoppedSlot);
        }

        private void PlayModeStartedSlot(object[] parameters)
        {
            SystemManager.Instance.KeyDownSignal.Connect(KeyDownSlot);
        }

        private void PlayModeStoppedSlot(object[] parameters)
        {
            SystemManager.Instance.KeyDownSignal.Disconnect(KeyDownSlot);
        }

        #region Slots

        /// <summary>
        /// Processes keys in play mode
        /// One of the keys is delete key
        /// When delete key pressed, and the game view in focus, deleting the selected component
        /// </summary>
        /// <param name="parameters"></param>
        private void KeyDownSlot(object[] parameters)
        {
            Event e = (Event) parameters[0];

            switch (e.keyCode)
            {
                case KeyCode.Delete:
                    /**
                     * Delete selected component only if inspector enabled
                     * */
                    if ("UnityEditor.GameView" == EditorWindow.focusedWindow.title && EditorSettings.InspectorEnabled)
                    {
                        //Debug.Log("Delete detected");
                        if (null != EditorState.Instance.Adapter)
                        {
                            DesignerOverlay.Instance.Unhover();
                            DesignerOverlay.Instance.Deselect();
                            Object.Destroy(EditorState.Instance.SelectedTransform.gameObject);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}