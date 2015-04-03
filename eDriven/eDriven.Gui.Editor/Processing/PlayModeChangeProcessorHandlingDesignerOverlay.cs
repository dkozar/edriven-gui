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

using eDriven.Core;
using eDriven.Gui.Designer.Rendering;
using eDriven.Gui.Mappers;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    /// <summary>
    /// Takes care of showing/hiding the designer overlay
    /// </summary>
    internal class PlayModeChangeProcessorHandlingDesignerOverlay
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PlayModeChangeProcessorHandlingDesignerOverlay _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PlayModeChangeProcessorHandlingDesignerOverlay()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PlayModeChangeProcessorHandlingDesignerOverlay Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PlayModeChangeProcessorHandlingDesignerOverlay instance"));
#endif
                    _instance = new PlayModeChangeProcessorHandlingDesignerOverlay();
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

        public void PlayModeStartedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("PlayModeChangeProcessorHandlingDesignerOverlay->PlayModeStartedSlot");
            }
#endif
            //_changesCached = false;

            if (EditorSettings.InspectorEnabled && Application.isEditor && Application.isPlaying/* && null == DesignerOverlay.Instance*/)
            {
                //if (FontMapper.IsMapping(DipSwitches.DefaultFontMapperId))
                if (null != FontMapper.GetDefault())
                {
                    // ReSharper disable once UnusedVariable
                    DesignerOverlay.Attach();
                    //overlay.Font = FontMapper.GetWithFallback(DipSwitches.DefaultFontMapperId).Font; // set font
                }
            }
        }

        public void PlayModeStoppedSlot(object[] parameters)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("PlayModeChangeProcessorHandlingDesignerOverlay->PlayModeStoppedSlot");
            }
#endif
            //_changesCached = false;

            if (EditorSettings.InspectorEnabled && Application.isEditor && Application.isPlaying/* && null == DesignerOverlay.Instance*/)
            {
                //if (FontMapper.IsMapping(DipSwitches.DefaultFontMapperId))
                if (null != FontMapper.GetDefault())
                {
                    // ReSharper disable once UnusedVariable
                    DesignerOverlay overlay = (DesignerOverlay)Framework.GetComponent<DesignerOverlay>(true); // add if non-existing
                    //overlay.Font = FontMapper.GetWithFallback(DipSwitches.DefaultFontMapperId).Font; // set font
                }
            }
        }
    }
}