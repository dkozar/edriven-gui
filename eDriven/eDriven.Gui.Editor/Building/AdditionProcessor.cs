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
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Hierarchy;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Building
{
    /// <summary>
    /// Processes live additions of components
    /// </summary>
    internal static class AdditionProcessor
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// Is processing additions, as well as top level additions
        /// </summary>
        /// <param name="topLevelAdditions">Needed for adding into order</param>
        /// <param name="additions">Needed for child instantiation</param>
        internal static void Process(List<Node> topLevelAdditions, List<Node> additions)
        {
            if (0 == topLevelAdditions.Count && 0 == additions.Count)
                return;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Processing {0} topLevelAdditions, {1} additions.", topLevelAdditions.Count, additions.Count));
            }
#endif
            foreach (Node node in additions)
            {
                bool isTopLevelAddition = topLevelAdditions.Contains(node);
                bool isPrefab = PrefabUtility.GetPrefabType(node.Adapter) == PrefabType.PrefabInstance;

                /**
                 * 1. Process transforms
                 * */
                Transform transform = node.Transform;

                if (null == transform)
                    continue; // ROOT

                ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                bool isStage = adapter is StageAdapter;

                Transform parentTransform = node.ParentTransform;

                if (null == parentTransform && !isStage)
                    continue; // not a stage, return (if stage, should process)

                GroupAdapter parentAdapter = null;

                if (null != parentTransform)
                    parentAdapter = GuiLookup.GetAdapter(node.ParentTransform) as GroupAdapter;

                /**
                 * 2. Process adapters
                 * */
                if (null != adapter)
                {
                    // this is eDriven.Gui component. process it properly
                    if (null == parentAdapter)
                    {
                        if (!isStage)
                        {
                            const string txt = "eDriven.Gui components could be added to containers only";
                            throw new Exception(txt);
                        }
                    }
                    else
                    {
                        /**
                         * Update parent ordering for top level additions only!
                         * */
                        if (isTopLevelAddition)
                        {
                            //node.Transform.parent = parentTransform;
                            parentAdapter.AddChild(adapter, true); // instantiate and change the parent collection

                            // ReSharper disable once RedundantCheckBeforeAssignment
                            if (adapter.transform.parent != parentTransform) // avoid multiple OnHierarchyChange callbacks
                                adapter.transform.parent = parentTransform;
                            
                            //adapter.transform.parent = parentTransform;
                            ParentChildLinker.Instance.Update(parentAdapter);
                        }
                        else
                        {
                            //parentAdapter.DoInstantiate(adapter, true); // instantiate only
                            ChildGroupPack pack = ChildGroupPack.Read(parentAdapter);
                            pack.Consolidate(); // there is a null slot here. We have to re-render children
                            parentAdapter.InstantiateChildren(true);
                        }

                        /**
                         * Instantiate component
                         * */
                        if (!adapter.Instantiated)
                            adapter.DoInstantiate(true);

                        /**
                         * By now, the component should be instantiated
                         * */
                        var cmp = adapter.Component;
                        if (null == cmp)
                        {
                            throw new Exception("Component not found on ComponentAdapter: " + adapter);
                        }
                    }
                }

                /**
                 * 3. If adapter stuff went without a problem...
                 * */
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (node.Transform.parent != parentTransform)
                    node.Transform.parent = parentTransform;
            }
        }
    }
}
