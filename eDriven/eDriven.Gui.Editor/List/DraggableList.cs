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

using System;
using System.Collections.Generic;
using eDriven.Core.Signals;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal class DraggableList
    {
        public IEnumerable<DraggableItem> Items
        {
            get
            {
//                List<DraggableItem> items = new List<DraggableItem>();
//                _data.ForEach(delegate(DraggableListGroup data)
//                {
//                    // items
//                    data.Items.ForEach(delegate(DraggableItem item)
//                    {
//                        items.Add(item);
//                    });
//                });
//                return items;
                return _data.RenderingItems;
            }
        }

        private ChildGroupRenderingList _data = new ChildGroupRenderingList();
        internal ChildGroupRenderingList Data
        {
            get
            {
                return _data;
            }
            set
            {
                //Debug.Log("* Setting data: " + value);
                _data = value;
                RebuildList();
                //_displayItems.Process(YComparison);
                
                //_doRepaint = true;
            }
        }

        public void RebuildList()
        {
            _displayItems.Clear();

            if (null != _data)
            {
                _data.Groups.ForEach(delegate(ChildGroup data)
                                         {
                                             // header
                                             if (data.HasHeader)
                                             {
                                                 _displayItems.Add(data.HeaderItem);
                                             }

                                             // items
                                             data.Items.ForEach(delegate(DraggableItem item)
                                                                    {
                                                                        _displayItems.Add(item);
                                                                    });
                                         });
                UpdateLayout(_width, _height);
            }
        }

        private readonly List<DraggableItem> _displayItems = new List<DraggableItem>();

        internal Signal PositionChangedSignal = new Signal();
        internal Signal RemovedSignal = new Signal();
        internal Signal EnabledSignal = new Signal();
        internal Signal PhaseChangedSignal = new Signal();
        
        private Vector2 _scrollPosition;

        private float _preferedHeight;
        private bool _needsScrollbar;
        
        private float _width;
        private float _height;
        
        private float _elementWidth = 50f;
        
        public float ElementHeight;
       
        //public float ToolbarHeight;
        public float ScrollbarWidth;

        private static DraggableItem _draggedItem;
        private DraggableItem _removedItem;
        private DraggableItem _enabledItem;
        private DraggableItem _phaseChangedItem;

        private int? _oldIndex;
        private int? _newIndex;

        private bool _doRepaint;
        public bool DoRepaint
        {
            get
            {
                if (_doRepaint)
                {
                    _doRepaint = false;
                    return true;
                }
                return _doRepaint;
            }
        }

        private int _removedItemIndex;
        private int _count;

        internal void Render()
        {
            //Debug.Log("Render");
            //_draggedItem = null;
            _doRepaint = false;
            bool dropped = false;
            //int index = 0;

            bool dragStart = false;

            //Debug.Log(">" + _height);

            var oldAlignment = GUI.skin.label.alignment;
            _scrollPosition = GUI.BeginScrollView(
                new Rect(0, 0, _width, _height),
                _scrollPosition,
                new Rect(0, 0, _width - ScrollbarWidth, Data.RenderingItemsCount * ElementHeight));

            GUI.skin.box.alignment = TextAnchor.MiddleLeft;

            if (Data.RenderingItemsCount > 0)
            {
                Color color = GUI.color;
                GUI.color = new Color(0.25f, 0.25f, 0.25f);
                GUI.Box(new Rect(0, 0, _width, Data.RenderingItemsCount * ElementHeight), string.Empty, StyleCache.Instance.Background);
                GUI.color = color;
            }

            //Debug.Log("Data.Count: " + Data.Count);
            _count = 0;

            // render rows
            foreach (DraggableItem data in Data.RenderingItems)
            {
                bool previousState = data.Dragging;

                ItemAction action = data.Render();
                if (action != null)
                {
                    switch (action.Type)
                    {
                        case ItemAction.REMOVE:
                            _removedItem = data;
                            _removedItemIndex = _count;
                            return;
                        case ItemAction.ENABLE:
                            _enabledItem = data;
                            break;
                        case ItemAction.PHASES_CHANGED:
                            _phaseChangedItem = data;
                            break;
                    }
                }

                if (Event.current.type == EventType.Repaint)
                {
                    //GUI.skin = GUI.skin;
                    //_draggingHandle.Draw(new Rect(3, 3, 16, 16), false, false, false, false);
                }

                if (data.Dragging)
                {
                    if (null == _draggedItem)
                    {
                        _oldIndex = data.Index;
                        //Debug.Log("??? " + _oldIndex);
                    }

                    _draggedItem = data;
                    //Debug.Log("*: " + _draggedItem);

                    if (!dragStart)
                    {
                        dragStart = true;
                    }

                    _doRepaint = true;

                    HandleBounds(data);

                    _newIndex = data.Index;
                }
                else if (previousState)
                {
                    dropped = true;
                }

                _count++;
            }

            GUI.skin.box.alignment = oldAlignment;

            GUI.EndScrollView();

            if (null != _draggedItem)
            {
                // bring to front // TODO
                //Data.RemoveItem(_draggedItem);
                //Data.AddItem(_draggedItem);
            }

            if (null != _enabledItem)
            {
                EnabledSignal.Emit(_enabledItem);
                _doRepaint = true;
                _enabledItem = null;
            }
            else if (null != _phaseChangedItem)
            {
                PhaseChangedSignal.Emit(_phaseChangedItem);
                _doRepaint = true;
                _phaseChangedItem = null;
            }

            if (dropped)
            {
                //Data.GetGroupAndItemIndex((int)_newIndex, out _newGroupIndex, out _newItemIndex);
                //Debug.Log("Drop to " + _newIndex);

                int groupIndex;
                int itemIndex;
                GetGroupIndex((int) _newIndex, out groupIndex, out itemIndex);

                //Debug.Log(string.Format("**** Result: grp:{0}, item:{1}", groupIndex, itemIndex));

                //CollectionChangedSignal.Emit(_draggedItem, _newItemIndex, _newGroupIndex);
                PositionChangedSignal.Emit(_draggedItem, itemIndex, groupIndex);

                Apply();

                _doRepaint = true;
                _draggedItem = null;
            }

            if (null != _removedItem)
            {
                int groupIndex;
                int itemIndex;
                GetGroupIndex(_removedItemIndex, out groupIndex, out itemIndex);
                RemoveRow(groupIndex, itemIndex);

                Apply();
                
                _doRepaint = true;
                _removedItem = null;
            }
        }

        protected void GetGroupIndex(int index, out int groupIndex, out int itemIndex)
        {
            if (Data.Groups.Count == 0)
                throw new Exception("No groups");

            var firstGroupHasHeader = Data.Groups[0].HasHeader;

            // if first group has no header, we are not waiting for a group row to appear
            // for switching groups - we are starting from 0
            groupIndex = firstGroupHasHeader ? -1 : 0;

            itemIndex = 0;
            for (int i = 0; i < _displayItems.Count; i++)
            {
                var item = _displayItems[i];
                //Debug.Log(i + ". Row type: " + item.GetType().Name);

                bool isGroupRow = item is GroupRow;

                if (isGroupRow)
                {
                    // group switch
                    groupIndex++;
                    itemIndex = 0;
                    //Debug.Log(" -> group switch: " + groupIndex);
                }
                    
                // is this THE row?
                if (index == i)
                {
                    break;
                }
                    
                if (!isGroupRow)
                {
                    itemIndex++;
                }
            }
        }

        internal void UpdateLayout(float width, float height)
        {
            //Debug.Log("UpdateLayout: " + width + "; " + height);
            _width = width;
            _height = height;
            _preferedHeight = Data.RenderingItemsCount * ElementHeight;
            _needsScrollbar = _preferedHeight > height;
            _elementWidth = _needsScrollbar ? width - ScrollbarWidth : width;
            //Debug.Log("_elementWidth: " + _elementWidth);
            int count = 0;
            _displayItems.ForEach(delegate(DraggableItem data)
                                    {
                                        //data.Bounds = new Rect(3, data.Bounds.y, _elementWidth - 7, data.Bounds.height);
                                        data.Bounds = new Rect(0, ElementHeight * count, _elementWidth, ElementHeight);
                                        count++;
                                    });
        }

        private void HandleBounds(DraggableItem data)
        {
            var newIndex = Mathf.Clamp((int)Math.Round((data.Bounds.y) / ElementHeight), 0, Data.RenderingItemsCount - 1);
            if (newIndex != _newIndex)
            {
                //Debug.Log("Flip");
                _newIndex = newIndex;
                Flip(data, newIndex);
            }
        }

        private void Flip(DraggableItem data, int index)
        {
            _newIndex = index;

            //Debug.Log("Flip to index: " + index);
            _displayItems.Remove(data);
            _displayItems.Insert(index, data);
            data.Index = index;

            for (int i = 0; i < _displayItems.Count; i++)
            {
                var d = _displayItems[i];
                d.Bounds = d != data ? new Rect(d.Bounds.x, i * ElementHeight, _elementWidth, ElementHeight) : new Rect(d.Bounds.x, d.Bounds.y, _elementWidth, ElementHeight);
            }
        }

        private void RemoveRow(int rowIndex, int itemIndex)
        {
            //Debug.Log("RemoveRow " + rowIndex + ", " + itemIndex);
            RemovedSignal.Emit(_removedItem, rowIndex, itemIndex);
        }

        int _newGroupIndex;
        int _newItemIndex;
        //private int _removedItemIndex;
        
        private void Apply()
        {
//            Debug.Log("_draggedItem: " + _draggedItem);
//            Debug.Log("_oldIndex: " + _oldIndex);
//            Debug.Log("_newIndex: " + _newIndex);

            //Data.GetGroupAndItemIndex((int) _newIndex, out _newGroupIndex, out _newItemIndex);

            //Data.Groups[_newGroupIndex].Items.

            //_groupManager.Reposition(adapter, _newGroupIndex, _newItemIndex);

            // take the index and rebuild the complex collection

            _displayItems.Clear();

            foreach (DraggableItem item in Data.RenderingItems)
            {
                 _displayItems.Add(item);
            }
                             
            _displayItems.Sort(YComparison);

            for (int i = 0; i < _displayItems.Count; i++)
            {
                Rect bounds = _displayItems[i].Bounds;
                _displayItems[i].Bounds = new Rect(0, i * ElementHeight, bounds.width, bounds.height);
                _displayItems[i].Index = i;
            }
        }

        private static readonly Comparison<DraggableItem> YComparison = delegate(DraggableItem o1, DraggableItem o2)
                                                                            {
                                                                                if (o1.Bounds.y < o2.Bounds.y)
                                                                                    return -1;

                                                                                if (o1.Bounds.y > o2.Bounds.y)
                                                                                    return 1;

                                                                                return 0;
                                                                            };

        public static void StopDrag()
        {
            _draggedItem = null;
        }
    }
}