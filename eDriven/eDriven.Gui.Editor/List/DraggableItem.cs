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

using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal abstract class DraggableItem
    {
        private Vector2 _dragStart;
        private bool _dragging;
        public int Index;

        protected DraggableItem(Rect position)
        {
            _bounds = position;
        }

        public bool Dragging
        {
            get
            {
                return _dragging;
            }
        }

        public abstract object Data
        {
            get;
            set;
        }

        private Rect _bounds;
        
        public GUIContent Content;
        
        /// <summary>
        /// Min Y position while dragging
        /// </summary>
        public float? YMin;

        /// <summary>
        /// Max Y position while dragging
        /// </summary>
        public float? YMax;

        public Rect Bounds
        {
            get
            {
                return _bounds;
            }

            set
            {
                _bounds = value;
            }
        }

        public abstract ItemAction Render();

        public void Drag(Rect dragRect)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                _dragging = false;
            }
            else if (Event.current.type == EventType.MouseDown && dragRect.Contains(Event.current.mousePosition))
            {
                _dragging = true;
                _dragStart = Event.current.mousePosition - new Vector2(_bounds.x, _bounds.y);
                Event.current.Use();
            }

            if (_dragging)
            {
                Vector2 v = Event.current.mousePosition - _dragStart;

                float posY = Mathf.Max(v.y, 0);
                if (null != YMin)
                {
                    posY = Mathf.Max(posY, (float)YMin);
                }
                if (null != YMax)
                {
                    posY = Mathf.Min(posY, (float)YMax);
                }

                _bounds = new Rect(0, posY, _bounds.width, _bounds.height);
            }
        }
    }
}