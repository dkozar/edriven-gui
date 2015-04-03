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
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal class ChildGroup
    {
        private readonly DraggableItem _headerItem;
        public DraggableItem HeaderItem
        {
            get { return _headerItem; }
        }

        public bool HasHeader
        {
            get { return null != _headerItem; }
        }

        public ChildGroup()
        {
        }

        public ChildGroup(GUIContent header)
        {
            if (null != header)
            {
                _headerItem = new GroupRow(null, new Rect(0, 0, 0, 0)) {Content = header};
            }
        }

        private List<DraggableItem> _items = new List<DraggableItem>();
        public List<DraggableItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public void Add(DraggableItem item)
        {
            if (!_items.Contains(item)) {
                //Debug.Log("ChildGroup Add: " + item);
                _items.Add(item);
            }
        }

        public void Remove(DraggableItem item)
        {
            _items.Remove(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(@"HasHeader: {0}", HasHeader));
            foreach (DraggableItem item in Items)
            {
                sb.AppendLine(string.Format(@"      {0}", item));
            }

            return sb.ToString();
        }
    }
}