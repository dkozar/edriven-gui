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
using eDriven.Core.Managers;
using eDriven.Gui.Editor.Styles;
using UnityEngine;
using GuiComponentTraverser = eDriven.Gui.Styles.GuiComponentTraverser;

namespace eDriven.Gui.Editor
{
    internal class StylingOverlayConnector
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StylingOverlayConnector _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StylingOverlayConnector()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StylingOverlayConnector Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StylingOverlayConnector instance"));
#endif
                    _instance = new StylingOverlayConnector();
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
            // render selection for selected components
            GuiComponentTraverser.SelectorSignal.Connect(OnTraverserSelectors);

            // remove selection when the screen is clicked or resized
            SystemManager.Instance.MouseDownSignal.Connect(ClearStage);
            SystemManager.Instance.ResizeSignal.Connect(ClearStage);
        }

        /// <summary>
        /// Gets a list of components fo draw the overlay for
        /// </summary>
        /// <param name="parameters"></param>
        private static void OnTraverserSelectors(object[] parameters)
        {
            if (!EditorSettings.InspectorEnabled)
            {
                StylingOverlayStage.Instance.Clear();
                return;
            }

            List<Components.Component> components = (List<Components.Component>)parameters[0];
            //Debug.Log("OnTraverserSelectors: " + components.Count);

            StylingOverlayStage.Instance.Clear();
            StylingOverlayStage.Instance.Draw(components);

            /*DeferManager.Instance.Defer(delegate
            {
                //StylingOverlayStage.Instance.Clear();
                //StylingOverlayStage.Instance.Draw(components);
            }, 3);*/
        }

        /// <summary>
        /// After the selector overlay has been drawn, we need a way to clear it
        /// </summary>
        /// <param name="parameters"></param>
        private static void ClearStage(object[] parameters)
        {
            //Debug.Log("ClearStage");
            StylingOverlayStage.Instance.Clear();
        }
    }
}
