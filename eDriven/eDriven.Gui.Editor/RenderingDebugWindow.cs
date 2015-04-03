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

//#if DEBUG
using eDriven.Core.Managers;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal sealed class RenderingDebugWindow : EDrivenEditorWindowBase
    {
        //[ExecuteInEditMode]

        //private SystemManager _systemManager;

        [MenuItem("Window/eDriven.Gui/Debug/Rendering")]
// ReSharper disable UnusedMember.Local
        static void Init()
// ReSharper restore UnusedMember.Local
        {
            /**
             * Instantiate window
             * */
            var window = GetWindow(typeof(RenderingDebugWindow), false, DipSwitches.RenderingDebugWindowName);
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(300, 300);
            window.Show();

            IconSetter.SetIcon(window);
        }

        [Obfuscation(Exclude = true)]
        // ReSharper disable UnusedMember.Local
        private void OnEnable()
            // ReSharper restore UnusedMember.Local
        {
            IconSetter.SetIcon(this);
        }

        private Rect _position; // small Rect
        private Vector2 _scrollPosition;
        private Rect _viewRect; // big Rect

// ReSharper disable UnusedMember.Local
        /// <summary>
        /// Renders the controls allowed for instantiation in relation to selected parent
        /// </summary>
        [Obfuscation(Exclude = true)]
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            if (EditorApplication.isPlaying)
            {
                _position.width = Screen.width;
                _position.height = Screen.height - 22; // minus scrollbar height

                _viewRect.width = SystemManager.Instance.ScreenSize.X;
                _viewRect.height = SystemManager.Instance.ScreenSize.Y;

                _scrollPosition = GUI.BeginScrollView(_position, _scrollPosition, _viewRect);

                StageManager.Instance.Render();

                GUI.EndScrollView();
            }
            else
            {
                GUILayout.Label(GuiContentCache.Instance.PlayModeOnly, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
        }
    }
}

//#endif