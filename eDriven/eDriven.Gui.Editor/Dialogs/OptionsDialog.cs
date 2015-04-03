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
using System.Reflection;
using eDriven.Core;
using eDriven.Core.Geom;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal sealed class OptionsDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static OptionsDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private OptionsDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static OptionsDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating OptionsDialog instance"));
#endif
                    //_instance = new AboutDialog();
                    _instance = (OptionsDialog)CreateInstance(typeof(OptionsDialog));
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
            title = "Options";
            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            Rectangle winRectangle = Rectangle.FromSize(new Point(450, 400));
            winRectangle = winRectangle.CenterInside(screenRectangle);
            position = winRectangle.ToRect();
            //Debug.Log("position: " + position);
            minSize = maxSize = winRectangle.Size.ToVector2();
            wantsMouseMove = true;

            IconSetter.SetIcon(this);
        }

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false
        };

// ReSharper disable UnusedMember.Local
        void OnGUI() {
// ReSharper restore UnusedMember.Local

            DialogContentWrapper.Start();

            if (null == _panelRenderer.ChromeStyle)
                _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared; // only OnGUI

            _panelRenderer.RenderStart(GuiContentCache.Instance.OptionsPanelTitle, true);

            OptionsBox.Instance.Render();

            _panelRenderer.RenderEnd();

            DialogContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }



        [MenuItem("Component/eDriven/Remove ghost framework objects")]
        public static void RemoveFrameworkObjects()
        {
            List<GameObject> objects = new List<GameObject>();
            
            /**
             * 1) Find by name
             * */
            GameObject fo = GameObject.Find("/" + Framework.FrameworkObjectName);
            if (null != fo)
            {
                objects.Add(fo);
            }

            /**
             * 2) Find all the framework objects
             * */
            var scripts = Object.FindObjectsOfType<FrameworkMonoBehaviour>();
            foreach (FrameworkMonoBehaviour frameworkMonoBehaviour in scripts)
            {
                if (null != frameworkMonoBehaviour.gameObject)
                    objects.Add(frameworkMonoBehaviour.gameObject);
            }

            if (objects.Count == 0)
            {
                EditorUtility.DisplayDialog("Info", "No objects found.", "OK");
            }
            else
            {
                foreach (GameObject gameObject in objects)
                {
                    if (EditorApplication.isPlaying)
                        Object.Destroy(gameObject);
                    else
                        Object.DestroyImmediate(gameObject);
                }
                EditorUtility.DisplayDialog("Info", "Object removed.", "OK");
            }
        }
    }
}