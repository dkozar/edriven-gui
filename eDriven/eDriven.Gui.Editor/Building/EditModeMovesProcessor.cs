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
    /// Processes edit mode nodes of components
    /// </summary>
    internal static class EditModeMovesProcessor
    {
        internal static void Process(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return;

            //Debug.Log(string.Format("Processing {0} moves in edit mode.", nodes.Count));

            foreach (Node node in nodes)
            {
                /**
                 * 1. Process transforms
                 * */
                Transform transform = node.Transform;
                if (null == node.Transform)
                    throw new Exception("Transform is null");

                /*if (null == transform)
                    continue; // ROOT*/

                //Debug.Log("node.Transform: " + transform);

                ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                bool isStage = adapter is StageAdapter;

                Transform oldParentTransform = node.OldParentTransform;
                Transform newParentTransform = node.ParentTransform;

                /* This happens when a stage moved to root */
                if (null == newParentTransform && !isStage)
                    continue; // not a stage, return (if stage, should process) - only stage could be added to root

                /**
                 * The adapter was moved
                 * It is still placed on the same (moved) transform
                 * We are accessing the old and the new parent adapter via transforms:
                 * 1. old parent transform could be referenced using the node.OldParentTransform
                 * 2. new parent transform could be referenced using the node.ParentTransform
                 * */
                GroupAdapter oldParentAdapter = null != oldParentTransform ?
                    GuiLookup.GetAdapter(oldParentTransform) as GroupAdapter :
                    null;

                GroupAdapter newParentAdapter = null != newParentTransform ? 
                    GuiLookup.GetAdapter(newParentTransform) as GroupAdapter :
                    null;

                Debug.Log(string.Format("[{0}] -> moving from [{1}] to [{2}]", 
                    adapter,
                    null == oldParentAdapter ? "-" : oldParentAdapter.ToString(),
                    null == newParentAdapter ? "-" : newParentAdapter.ToString()
                ));

                /**
                 * 2. Sanity check
                 * */
                if (null == newParentAdapter && !isStage)
                {
                    throw new Exception("eDriven.Gui components could be added to containers only");
                }

                /**
                 * 3. Process adapters
                 * */
                if (null != oldParentAdapter)
                {
                    oldParentAdapter.RemoveChild(adapter);
                    ParentChildLinker.Instance.Update(oldParentAdapter);
                }

                if (null != newParentAdapter)
                {
                    newParentAdapter.AddChild(adapter, true);
                    ParentChildLinker.Instance.Update(newParentAdapter); 
                }

                /**
                 * 4. Set transform
                 * */
                node.Transform.parent = newParentTransform;
            }
        }
    }
}
