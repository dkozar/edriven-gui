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

using eDriven.Gui.Geom;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class StyleDeclarationInsetRenderer
    {
        /// <summary>
        /// The color of the chrome
        /// </summary>
        public static Color ChromeColor = ColorMixer.FromHex(0xFFFFFF).ToColor();/// <summary>
        
        /// Collapsible/expandable?
        /// </summary>
        //public bool Collapsible = true;

        private bool _expanded;

        private readonly TextureCache _tc = TextureCache.Instance;

        public static int HeaderHeight = 28;

        private static readonly EdgeMetrics FoldoutBorderMetrics = new EdgeMetrics(6, 6, 4, 4);

        public string Error;

        public bool MediaQueriesPassed;
        
        //public SerializedProperty TypeProperty;

        public bool RenderStart(Rect position, string title, bool expanded)
        {
            var icon = MediaQueriesPassed ? _tc.StyleDeclaration : _tc.StyleDeclarationNotApplied;

            if (!string.IsNullOrEmpty(Error))
                icon = _tc.Error;

            GUIContent content = new GUIContent(
                " " + title, icon,
                Error
            );

            GUI.backgroundColor = ChromeColor;

            GUI.Box(position, GUIContent.none, StyleCache.Instance.InsetChrome); // inset chrome

            GUI.backgroundColor = Color.white;

            var pos = new Rect(position.x + FoldoutBorderMetrics.Left, position.y + FoldoutBorderMetrics.Top, position.width - FoldoutBorderMetrics.Left - FoldoutBorderMetrics.Right, HeaderHeight - FoldoutBorderMetrics.Top - FoldoutBorderMetrics.Bottom);
            
            //if (Collapsible)
            //{
                _expanded = GUI.Toggle(pos, expanded, content, _expanded ? StyleCache.Instance.InsetHeaderExpanded : StyleCache.Instance.InsetHeaderCollapsed);
                if (expanded != _expanded)
                {
                    expanded = _expanded;
                }
            //}
            //else
            //{
            //    // not collapsible. Render label instead.
            //    //GUI.Label(pos, content, StyleCache.Instance.InsetHeaderCollapsed);
            //    EditorGUI.PropertyField(pos, TypeProperty, GUIContent.none, true);
            //}

            return expanded;
        }
    }
}