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

using eDriven.Gui.Editor.Dialogs;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class AboutBox
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static AboutBox _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private AboutBox()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static AboutBox Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating AboutBox instance"));
#endif
                    _instance = new AboutBox();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {

        }

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false
        };

        private bool _step;
        private bool _oldStep;

        internal void Render()
        {
            //GUILayout.Space(-5);

            _oldStep = _step;
            
            _step = Time.time % (2 * DipSwitches.AboutDuration) > DipSwitches.AboutDuration;
            _panelRenderer.ContentStyle = _step ? StyleCache.Instance.AboutPanelContent2 : StyleCache.Instance.AboutPanelContent;

            if (null == _panelRenderer.ChromeStyle)
                _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared; // only OnGUI

            _panelRenderer.RenderStart(new GUIContent(string.Format(@"{0} {1}", Info.AssemblyName, Info.AssemblyVersion), TextureCache.Instance.Information)/*GuiContentCache.Instance.AboutPanelTitle*/, true);

            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_step ? GuiContentCache.Instance.Logo2 : GuiContentCache.Instance.Logo1);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("eDriven.Gui - Unity3d GUI framework", StyleCache.Instance.AboutLabel, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Copyright Danko Kozar 2010-2014. All rights reserved.", StyleCache.Instance.AboutLabel, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            _panelRenderer.RenderEnd();

            if (_step != _oldStep)
            {
                _oldStep = _step;
                AboutDialog.Instance.Repaint();
            }
        }
    }
}