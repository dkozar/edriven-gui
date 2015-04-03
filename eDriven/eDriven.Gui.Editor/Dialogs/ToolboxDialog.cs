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

using System.Reflection;
using eDriven.Core.Geom;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal sealed class ToolboxDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static ToolboxDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private ToolboxDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ToolboxDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating ToolboxDialog instance"));
#endif
                    //_instance = new ToolboxDialog();
                    _instance = (ToolboxDialog)CreateInstance(typeof(ToolboxDialog));
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public bool ShowHelp;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            title = "Toolbox";
            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            //Debug.Log("screenRectangle: " + screenRectangle);
            Rectangle winRectangle = Rectangle.FromSize(new Point(600, 400));
            winRectangle = winRectangle.CenterInside(screenRectangle);
            position = winRectangle.ToRect();
            minSize = new Point(240, 240).ToVector2();
            wantsMouseMove = true;

            PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Connect(SelectionChangedSlot);
        }

        private void SelectionChangedSlot(object[] parameters)
        {
            //Debug.Log("ToolboxDialog: SelectionChangedSlot");
            Repaint();
        }

        ~ToolboxDialog()
        {
            PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Disconnect(SelectionChangedSlot);
        }

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false,
            Tools = new System.Collections.Generic.List<ToolDescriptor>(new[]
            {
                new ToolDescriptor("options", TextureCache.Instance.Options, EditorSettings.ShowAddChildOptions),
                new ToolDescriptor("help", TextureCache.Instance.Help)
            })
        };

// ReSharper disable UnusedMember.Local
        void OnGUI() {
// ReSharper restore UnusedMember.Local
            //GUILayout.Space(-5);

            DialogContentWrapper.Start();

            _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared; // only OnGUI

            _panelRenderer.RenderStart(new GUIContent("Add Child", TextureCache.Instance.Add), true);
            ShowHelp = _panelRenderer.ClickedTools.Contains("help");
            EditorSettings.ShowAddChildOptions = _panelRenderer.ClickedTools.Contains("options");

            Toolbox.Instance.Render();

            _panelRenderer.RenderEnd();

            //GUILayout.Space(3);
            DialogContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }
    }
}