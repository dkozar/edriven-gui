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

using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal class GroupRow : DraggableItem
// This class just has the capability of being dragged in GUI - it could be any type of generic data class
    {
        public Color? Color;
        public bool IsContainer;
        
        public static bool OldEnabled
        {
            get { return _oldEnabled; }
        }

        public override object Data { get; set;}

        public GroupRow(object data, Rect position)
            : base(position)
        {
            Data = data;
        }

        private Color _oldColor;

        private static bool _oldEnabled;

        public GUIContent Content;

        public override ItemAction Render()
        {
            //Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 100.0f);
            Rect drawRect = Bounds;

            GUILayout.BeginArea(drawRect, StyleCache.Instance.GroupRow); //GUI.skin.GetStyle("Box"));
            GUILayout.BeginHorizontal();

            //GUILayout.Label(new GUIContent(string.Empty, TextureCache.Instance.DragHandle), StyleCache.Instance.DragHandle, GUILayout.ExpandWidth(false));
            
            if (null != Color)
            {
                _oldColor = GUI.color;
                GUI.backgroundColor = (Color) Color;
            }

            GUILayout.Label(TextureCache.Instance.GroupRowArrow, StyleCache.Instance.GroupRowLabel, GUILayout.ExpandWidth(false));

            if (null != Content)
                GUILayout.Label(Content, StyleCache.Instance.GroupRowLabel, GUILayout.ExpandWidth(false));

            if (null != Color)
                GUI.backgroundColor = _oldColor;

            //GUILayout.Label(_mapping.EventType, StyleCache.Instance.DragHandle, GUILayout.MaxWidth(120)); //(position.width - 60)/2));

            GUILayout.FlexibleSpace();

            // open script button
            _oldEnabled = GUI.enabled;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            return null;
        }
    }
}