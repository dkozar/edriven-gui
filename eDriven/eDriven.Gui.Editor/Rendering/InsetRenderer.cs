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

using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    public class InsetRenderer
    {
        /// <summary>
        /// Collapsible/expandable?
        /// </summary>
        public bool Collapsible = true;

        /// <summary>
        /// Show collapse button?
        /// </summary>
        public bool ShowCollapseButton;

        /// <summary>
        /// Collapsible/expandable?
        /// </summary>
        public bool ExpandWidth = true;

        /// <summary>
        /// The color of the chrome
        /// </summary>
        public Color ChromeColor = ColorMixer.FromHex(0xFFFFFF).ToColor();

        private bool _expanded;

        private readonly TextureCache _tc = TextureCache.Instance;

        /// <summary>
        /// Inner style
        /// </summary>
        public GUIStyle ContentStyle;

        public bool RenderStart(GUIContent content, bool expanded)
        {
            GUI.backgroundColor = ChromeColor;

            EditorGUILayout.BeginVertical(StyleCache.Instance.InsetChrome); // inset chrome

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();

            if (Collapsible)
            {
                _expanded = GUILayout.Toggle(expanded, content, 
                    _expanded ? StyleCache.Instance.InsetHeaderExpanded : StyleCache.Instance.InsetHeaderCollapsed,
                    GUILayout.ExpandWidth(ExpandWidth), GUILayout.ExpandHeight(false));

                if (expanded != _expanded && !ShowCollapseButton)
                {
                    expanded = _expanded;
                }

                if (ShowCollapseButton)
                    expanded = GUILayout.Toggle(expanded, expanded ? TextureCache.Instance.Remove : TextureCache.Instance.Add, StyleCache.Instance.ImageOnlyNoFrameButton, GUILayout.ExpandWidth(false));    
            }
            else
            {
                // not collapsible. Render label instead.
                GUILayout.Label(content, StyleCache.Instance.InsetHeaderCollapsed, GUILayout.ExpandWidth(false));
            }
            
            EditorGUILayout.EndHorizontal();

            if (expanded)
                EditorGUILayout.BeginVertical(ContentStyle ?? StyleCache.Instance.InsetContent, GUILayout.ExpandWidth(true)); // panel content
            else    
                EditorGUILayout.EndVertical();

            return expanded;
        }

        public bool RenderStart(string title, bool expanded)
        {
            return RenderStart(new GUIContent(title + " ", expanded ? _tc.InsetExpanded : _tc.InsetCollapsed), expanded);
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void RenderEnd()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            EditorGUILayout.EndVertical(); // panel content
            EditorGUILayout.EndVertical(); // panel chrome
        }
    }
}