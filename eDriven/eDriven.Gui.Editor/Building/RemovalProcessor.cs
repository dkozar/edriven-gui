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
using eDriven.Gui.Editor.Persistence;
using UnityEngine;

namespace eDriven.Gui.Editor.Building
{
    /// <summary>
    /// Processes live additions of components
    /// </summary>
    internal static class RemovalProcessor
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        internal static void Process(List<Node> removals)
        {
            if (0 == removals.Count)
                return;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Processing {0} removals.", removals.Count));
            }
#endif
            //Debug.Log(string.Format("Processing {0} removals.", removals.Count));
            // TODO: do removals bottom up! nesting level should be used here

            foreach (Node node in removals)
            {
                //if (null == node.Transform)
                //    continue; // ROOT

                node.RemoveFromHierarchy();

                //Debug.Log("node.ParentTransform: " + node.ParentTransform);
                if (null == node.ParentTransform ) //|| /*!*/(adapter is StageAdapter))
                    continue; // not a stage, return (if stage, should process)

                // consolidate parent transform

                ComponentAdapter adapter = node.Adapter;

                GroupAdapter parentGroupAdapter = GuiLookup.GetAdapter(node.ParentTransform) as GroupAdapter;
                if (null != parentGroupAdapter) {

                    /**
                     * Stop monitoring
                     * */
                    PersistenceManager.Instance.Unwatch(node.AdapterId);

                    //PersistenceManager.Instance.RemoveAdapter(node.AdapterId);

                    /**
                     * Note: if object removed from the hierarchy, the adapter is destroyed
                     * In that case following command doesn nothing (doesn not remove the slot from the parent)
                     * Thus we have the consolidation below (removing null)
                     * */
                    parentGroupAdapter.RemoveChild(adapter);
                    
                    // TODO: consolidate only for top level removals (add parameter)

                    /*parentContainerAdapter.RemoveChild(adapter);*/
                    //parentContainerAdapter.RemoveAllChildren();
                    ChildGroupPack pack = ChildGroupPack.Read(parentGroupAdapter);
                    pack.Consolidate(); // there is a null slot here. We have to re-render children
                    //Debug.Log("*pack: " + pack);
                    parentGroupAdapter.InstantiateChildren(true);
                }
            }
        }
    }
}
