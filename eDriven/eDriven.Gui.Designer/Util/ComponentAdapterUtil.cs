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
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer.Adapters;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Util
{
    ///<summary>
    ///</summary>
    public static class ComponentAdapterUtil
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        ///<summary>
        ///</summary>
        ///<param name="adapterList"></param>
        ///<returns></returns>
        public static bool ListContainsNullReference(List<ComponentAdapter> adapterList)
        {
            //Debug.Log("Running ContainsNullReference:");
            int count = adapterList.Count;
            //Debug.Log("count: " + count);

            for (int i = 0; i < count; i++)
            {
                //Debug.Log(string.Format(@"   - Comparing: {0} and {1}", list1[i], list2[i])); // beware of null for Debug
                if (adapterList[i] == null) 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Prints out the component list
        /// </summary>
        /// <param name="collection"></param>
        public static string DescribeAdapterList(List<ComponentAdapter> collection)
        {
            return DescribeAdapterList(collection, false);
        }

        /// <summary>
        /// Prints out the component list
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="showInstanceIds"></param>
        public static string DescribeAdapterList(List<ComponentAdapter> collection, bool showInstanceIds)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ComponentAdapter adapter in collection)
            {
                string id = string.Empty;
                if (null != adapter && showInstanceIds)
                {
                    id = string.Format(" [{0}]", adapter.GetInstanceID());
                }
                sb.AppendLine(string.Format("    -> {0}{1}", null == adapter ? "-" : adapter.ToString(), id));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Populates the already instantiated container with children specified by component adapters
        /// </summary>
        /// <param name="thisComponent"></param>
        /// <param name="targetContainer"></param>
        /// <param name="componentAdapters"></param>
        /// <param name="assignToDescriptor"></param>
        /// <param name="muteEvents"></param>
        /// <param name="removeAllChildren"></param>
        public static void PopulateContainer(Component thisComponent, Group targetContainer, ComponentAdapter[] componentAdapters, bool assignToDescriptor, bool muteEvents, bool removeAllChildren)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"PopulateContainer: {0};
Child adapters: 
{1}", targetContainer, DescribeAdapterList(new List<ComponentAdapter>(componentAdapters))));
            }
#endif

            if (null == targetContainer && null == thisComponent)
                return;

            //Debug.Log(string.Format(@"DrawingList: {0}", container.QDrawingList.Count));

            //if (muteEvents)
            //    container.CreatingContentPane = true;

            //Debug.Log("container.NumberOfContentChildren: " + container.NumberOfContentChildren);

            var list = thisComponent as IContentChildList; // Group, Panel, Window etc.

            if (removeAllChildren)
            {
                if (null != targetContainer)
                    targetContainer.RemoveAllContentChildren();
                else
                {
                    if (list != null)
                        list.RemoveAllContentChildren();
                }
            }
                

            //Debug.Log(string.Format(@"SortedChildren2: {0}", container.QDrawingList.Count));

            foreach (ComponentAdapter adapter in componentAdapters)
            {
                if (null == adapter || !adapter.enabled || !adapter.gameObject.activeInHierarchy) // no descriptors on this node or descriptor disabled. skip
                    continue; // 20130426

                Component component = adapter.Produce(!adapter.FactoryMode, assignToDescriptor);
                if (null != component) {
                    //Debug.Log("  *Adding component: " + component);
                    
                    if (null != targetContainer)
                        targetContainer.AddContentChild(component);
                    else
                    {
                        if (list != null)
                            list.AddContentChild(component);
                    }

                    //// 20130813 - moved from ComponentAdapter.Produce, 
                    //// to avoid to subscribe/unsubscribe to PREINITIALIZE
                    //ContainerAdapter ca = adapter as ContainerAdapter;
                    //if (null != ca)
                    //{
                    //    ca.InstantiateChildren(assignToDescriptor);
                    //}
                }
            }

            //if (muteEvents)
            //    container.CreatingContentPane = false;
        }
    }
}