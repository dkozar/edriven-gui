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
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Hierarchy;

namespace eDriven.Gui.Editor.Display.Order
{
    internal class GroupManager
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode = true;

// ReSharper restore UnassignedField.Global
#endif

        private readonly GroupAdapter _groupAdapter;

        private readonly List<ChildGroupDescriptor> _groupDescriptors;
        public List<ChildGroupDescriptor> GroupDescriptors
        {
            get { return _groupDescriptors; }
        }

        private ChildGroupPack _pack;
        
        public GroupManager(GroupAdapter adapter)
        {
            //Debug.Log("Creating GroupManager");
            _groupAdapter = adapter;
            _groupDescriptors = DesignerReflection.GetChildGroupsReferences(adapter);

            _pack = ChildGroupPack.Read(adapter);
        }

        /// <summary>
        /// Note: this method actually changes the collections
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public ChildGroupPack Remove(ComponentAdapter adapter)
        {
            _pack = ChildGroupPack.Read(_groupAdapter);

            //Debug.Log(string.Format("Reorder: grp:{0}, item:{1}", groupIndex, itemIndex));

            // get old group index
            var oldGroupIndex = _pack.GetGroupHostingAdapterIndex(adapter);

            // remove adapter from old group
            _pack.Groups[oldGroupIndex].Remove(adapter);

            return _pack;
        }

        /// <summary>
        /// Note: this method actually changes the collections
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="groupIndex"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public ChildGroupPack Reorder(ComponentAdapter adapter, int groupIndex, int itemIndex)
        {
            _pack = ChildGroupPack.Read(_groupAdapter);

            //Debug.Log(string.Format("Reorder: grp:{0}, item:{1}", groupIndex, itemIndex));

            // get old group index
            var oldGroupIndex = _pack.GetGroupHostingAdapterIndex(adapter);

            // remove adapter from old group
            _pack.Groups[oldGroupIndex].Remove(adapter);

            // add adapter to a new group
            _pack.Groups[groupIndex].Insert(adapter, itemIndex);

            return _pack; // (ChildGroupPack)_pack.Clone();
            
            ////Debug.Log("_groupDescriptors.Count: " + _groupDescriptors.Count);
            ////Debug.Log("groupIndex: " + groupIndex);
            //ChildGroupDescriptor groupDescriptor = _groupDescriptors[groupIndex];
            ////Debug.Log("groupDescriptor: " + groupDescriptor);

            //List<ComponentAdapter> adaptersCollection = groupDescriptor.GetChildAdaptersCollection(_containerAdapter);
            ////var targetContainer = groupDescriptor.GetTargetContainer((Container)_containerAdapter.Component);

            ////Debug.Log("adaptersCollection: " + adaptersCollection);

            //if (null == adaptersCollection)
            //    return;

            ///**
            // * 1. Find the old collection containing the adapter and remove the adapter
            // * */
            //var oldCollection = GetGroupContainingAdapter(adapter);
            ////Debug.Log("Old collection: " + collection);
            //oldCollection.Remove(adapter);

            ///**
            // * 2. Insert it into a new collection at given index
            // * */

            //if (oldCollection == adaptersCollection)
            //    itemIndex = Math.Max(itemIndex, 0);

            ////Debug.Log(string.Format("Inserting to : [{0}] at index {1}", adaptersCollection, itemIndex));
            //adaptersCollection.Insert(itemIndex, adapter);

            ////Debug.Log("targetContainer: " + targetContainer);
            ////Debug.Log("newItemIndex: " + itemIndex);
            ////Debug.Log("childAdapters[newItemIndex]: " + adaptersCollection[itemIndex]);

            //if (Application.isPlaying)
            //    Flush();

            ////var comp = adaptersCollection[itemIndex].Component;

            ////if (null != targetContainer && null != comp)
            ////    targetContainer.AddContentChildAt(itemIndex, comp);

            ////            if (_depthMode)
            ////            {
            ////                ContainerAdapter.SetChildDepth(index, adapter);
            ////            }
            ////            else
            ////            {
            ////                ContainerAdapter.Reorder(index, adapter);
            ////            }
        }

//        public void Flush()
//        {
//            //Debug.Log("_groupDescriptors.Count: " + _groupDescriptors.Count);
//            //Debug.Log("groupIndex: " + groupIndex);

//            foreach (ChildGroupDescriptor groupDescriptor in _groupDescriptors)
//            {
//                if (null == _containerAdapter.Component) // not instantiated
//                    continue;

//                //Debug.Log("Getting a target container for " + _containerAdapter);
//                var targetContainer = groupDescriptor.GetTargetContainer((Container)_containerAdapter.Component);

//                if (null == targetContainer)
//                    throw new Exception("targetContainer is null");

//                targetContainer.RemoveAllContentChildren();

//                var adapters = groupDescriptor.GetChildAdaptersCollection(_containerAdapter);
//#if DEBUG
//                if (DebugMode)
//                {
//                    Debug.Log(string.Format(@"groupDescriptor: {0}
//{1}", groupDescriptor.Attribute.Label, ComponentAdapterUtil.DescribeAdapterList(adapters)));
//                }
//#endif

//                foreach (ComponentAdapter adapter in adapters)
//                {
//                    if (null == adapter)
//                        continue; // throw new Exception("adapter is null");

//                    var comp = adapter.Component;
//                    if (null == comp)
//                        //throw new Exception("adapter.Component is null");
//                        continue;

//                    targetContainer.AddContentChild(comp);
//                }
//            }
//        }

        ///<summary>
        ///</summary>
        ///<param name="childAdapter"></param>
        ///<returns></returns>
        public List<ComponentAdapter> GetGroupContainingAdapter(ComponentAdapter childAdapter)
        {
            foreach (ChildGroupDescriptor groupDescriptor in _groupDescriptors)
            {
                List<ComponentAdapter> collection = groupDescriptor.GetChildAdaptersCollection(_groupAdapter);
                if (collection.Contains(childAdapter))
                    return collection;
            }
            return null;
        }
    }
}