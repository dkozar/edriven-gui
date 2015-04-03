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
using UnityEngine;

namespace eDriven.Gui.Editor.Building
{
    /// <summary>
    /// Processes play mode moves of components
    /// </summary>
    internal static class MovesProcessor
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        internal static void Process(List<Node> moves)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Processing {0} moves.", moves.Count));
            }
#endif

            foreach (Node node in moves)
            {
                /**
                 * 1. Process transforms
                 * */
                Transform transform = node.Transform;

                Transform parentTransform = node.ParentTransform;

                if (null == transform)
                    continue; // ROOT

                //Debug.Log("node.Transform: " + transform);

                ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                bool thisIsStage = adapter is StageAdapter;

                /**
                 * The adapter has been moved
                 * It is still placed on the same (moved) transform
                 * We are accessing the old and the new parent adapter via transforms:
                 * - the old parent transform could be referenced using the node.OldParentTransform
                 * - the new parent transform could be referenced using the node.ParentTransform
                 * */

                if (null == node.Transform)
                    throw new Exception("Transform is null");

                GroupAdapter oldParentAdapter = GuiLookup.GetAdapter(node.OldParentTransform) as GroupAdapter;
                GroupAdapter newParentAdapter = GuiLookup.GetAdapter(node.ParentTransform) as GroupAdapter;

                //Debug.Log(string.Format("  -> moving from [{0}] to [{1}]", oldParentAdapter, newParentAdapter));

                /**
                 * 2. Process adapters
                 * */
                if (null != oldParentAdapter)
                {
                    oldParentAdapter.RemoveChild(adapter);
                    ParentChildLinker.Instance.Update(oldParentAdapter);
                }

                if (null == newParentAdapter)
                {
                    if (!thisIsStage)
                    {
                        const string txt = "eDriven.Gui components could be added to containers only";
                        //Debug.LogWarning(txt);
                        throw new Exception(txt);
                    }
                }
                else
                {
                    newParentAdapter.AddChild(adapter, true);
                    ParentChildLinker.Instance.Update(newParentAdapter);
                }

                /**
                 * 3. If adapter stuff went without a problem...
                 * */
                node.Transform.parent = parentTransform;

                //ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                //bool thisIsStage = adapter is StageAdapter;

                ////Debug.Log("done.");

                //Transform parentTransform = node.ParentTransform;

                //if (null == parentTransform && !thisIsStage)
                //    continue; // not a stage, return (if stage, should process) - only stage could be added to root

                //ContainerAdapter newParentAdapter = null;

                //if (null != parentTransform)
                //    newParentAdapter = GuiLookup.GetAdapter(parentTransform) as ContainerAdapter;

            //    /**
            //     * 2. Process adapters
            //     * */
            //    if (null != adapter) 
            //    {
            //        // this is eDriven.Gui component. process it properly
            //        if (null == newParentAdapter)
            //        {
            //            if (!thisIsStage) {
            //                const string txt = "eDriven.Gui components could be added to containers only";
            //                //Debug.LogWarning(txt);
            //                throw new Exception(txt);
            //                //return;
            //            }
            //        }
            //        else
            //        {
            //            ContainerAdapter oldParentAdapter = node.OldParentTransform.GetComponent<ContainerAdapter>();

            //            //Debug.Log("oldParentAdapter: " + oldParentAdapter);
            //            //Debug.Log("newParentAdapter: " + newParentAdapter);

            //            oldParentAdapter.RemoveChild(adapter);

            //            newParentAdapter.AddChild(adapter, true);

            //            ////if (!PrefabUtil.IsCreatedFromPrefab(oldParentAdapter)) // TODO: Need this?
            //            //    ParentChildLinker.Instance.Update(oldParentAdapter);

            //            ////if (!PrefabUtil.IsCreatedFromPrefab(newParentAdapter)) // TODO: Need this?
            //            //    ParentChildLinker.Instance.Update(newParentAdapter);

            //            if (EditorApplication.isPlaying)
            //            {
            //                if (!adapter.Instantiated)
            //                    adapter.DoInstantiate(true);
            //                //Debug.Log(string.Format("OnHierarchyChange: Component instantiated: {0} [{1}]", adapter.Component, adapter.transform.GetInstanceID()));
            //            }

            //            var cmp = adapter.Component;
            //            if (null == cmp)
            //            {
            //                throw new Exception("Component not found on ComponentAdapter: " + adapter);
            //            }
            //        }
            //    }

            //    /**
            //     * 3. If adapter stuff went without a problem...
            //     * */
            //    node.Transform.parent = parentTransform;
            }
        }
    }
}
