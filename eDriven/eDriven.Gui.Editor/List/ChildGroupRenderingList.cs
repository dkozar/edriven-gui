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
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal class ChildGroupRenderingList
    {
        private readonly List<ChildGroup> _groups = new List<ChildGroup>();
        public List<ChildGroup> Groups
        {
            get { return _groups; }
        }

        //public ChildGroupList()
        //{
        //    //_groups.Add(new ChildGroup());
        //}

        //internal IEnumerable<DraggableItem> Items
        //{
        //    get
        //    {
        //        foreach (DraggableListGroup group in _groups)
        //        {
        //            foreach (DraggableItem item in group.Items)
        //            {
        //                yield return item;
        //            }
        //        }
        //    }
        //}

        public int ItemCount
        {
            get
            {
                if (_groups.Count == 0)
                    return 0;

                int count = 0;
                for (int i = 0; i < _groups.Count; i++)
                {
                    count += _groups[i].Items.Count;
                }
                return count;
            }
        }

        internal IEnumerable<DraggableItem> RenderingItems
        {
            get
            {
                foreach (ChildGroup group in _groups)
                {
                    if (group.HasHeader)
                        yield return group.HeaderItem;

                    foreach (DraggableItem item in group.Items)
                    {
                        yield return item;
                    }
                }
            }
        }

        public int RenderingItemsCount
        {
            get
            {
                int count = 0;
                if (_groups.Count == 0)
                    return 0;

                for (int i = 0; i < _groups.Count; i++)
                {
                    if (_groups[i].HasHeader)
                        count++;
                    count += _groups[i].Items.Count;
                }
                return count;
            }
        }

        public void AddGroup(ChildGroup group)
        {
            if (!_groups.Contains(group))
                _groups.Add(group);
        }

        public void RemoveGroup(ChildGroup group)
        {
            //Debug.Log("RemoveGroup!");
            _groups.Remove(group);
        }

        public void AddItem(DraggableItem item)
        {
            if (_groups.Count == 0)
                throw new Exception("Not groups defined");

            _groups[0].Add(item);
        }

        public void AddItem(DraggableItem item, int groupIndex)
        {
            if (groupIndex > _groups.Count - 1)
                throw new Exception("Out of bounds exception");

            _groups[groupIndex].Add(item);
        }

        public void AddItem(DraggableItem item, ChildGroup group)
        {
            group.Add(item);
        }

        public ChildGroup RemoveItem(DraggableItem item)
        {
            //Debug.Log("RemoveItem!");
            foreach (ChildGroup group in _groups)
            {
                if (group.Items.Contains(item))
                {
                    group.Remove(item);
                    return group;
                }
            }
            return null;
        }

        public void Clear()
        {
            Clear(false);
        }

        public void Clear(bool purgeGroups)
        {
            //Debug.Log("Clear!");
            foreach (ChildGroup group in _groups)
            {
                group.Clear();
            }
            if (purgeGroups)
            {
                _groups.Clear();
            }
        }

//        public void Reposition(DraggableItem item, int index, out int newGroupIndex, out int newItemIndex)
//        {
//            //Debug.Log("Reposition. Index: " + index);
//            //Debug.Log("this: " + this);
            
////            ChildGroup newGroup;
////            int newIndex;

//            GetGroupAndItemIndex(index, out newGroupIndex, out newItemIndex);
//            //Debug.Log(string.Format(@"newGroup: {0}; newIndex: {1}", newGroup, newIndex));

//            //ChildGroup oldGroup = RemoveItem(item);
//            //if (null != newGroup)
//            //{
//            //    Debug.Log(string.Format(@"Adding to group ""{0}"" at index {1}", newGroup, newIndex));
//            //    AddItem(item, newGroup);
//            //}
//        }

        //private void GetGroupAndItemIndex(int index, out ChildGroup group, out int itemIndex)
        internal void GetGroupAndItemIndex(int index, out int newGroupIndex, out int newItemIndex)
        {
            Debug.Log(string.Format("GetGroupAndItemIndex: {0}", index));

            int count = 0;
            newGroupIndex = -1;
            newItemIndex = 0;
            //Debug.Log("_groups.Count: " + _groups.Count);
            for (int i = 0; i < _groups.Count; i++)
            {
                var currentGroup = _groups[i];
                //Debug.Log("currentGroup: " + currentGroup);
                //Debug.Log("currentGroup.Items.Count: " + currentGroup.Items.Count);

                if (currentGroup.HasHeader)
                    count++;

                //Debug.Log("count1: " + count);

                if (currentGroup.Items.Count == 0) // empty group
                {
                    newGroupIndex = i;
                    newItemIndex = 0;
                    return;
                }
                
                for (int j = 0; j < currentGroup.Items.Count; j++)
                {
                    //Debug.Log("count: " + count);
                    if (count == index)
                    {
                        newGroupIndex = i;
                        newItemIndex = j;
                        return;
                    }
                    count++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ChildGroup group in _groups)
            {
                sb.Append(string.Format(@"Group: {0}", group));
            }

            return string.Format(@"***** ChildGroupList:
{0}", sb);
        }
    }
}