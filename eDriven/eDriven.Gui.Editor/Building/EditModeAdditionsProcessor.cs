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
    /// Processes edit mode additions of components
    /// We don't care about the component (instatiation) stuff since we're in edit mode
    /// We are processing only the top level aditions
    /// That's because we don't care about the instantiation
    /// Also this structure could be dragged in as a prefab, which has internally everything set
    /// </summary>
    internal static class EditModeAdditionsProcessor
    {
        public static void Process(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return;

            //Debug.Log(string.Format("Processing {0} top level additions in edit mode.", nodes.Count));

            foreach (Node node in nodes)
            {
                /**
                 * 1. Process transforms
                 * */
                Transform transform = node.Transform;

                if (null == transform)
                    continue; // ROOT

                ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                bool isStage = adapter is StageAdapter;

                Transform parentTransform = node.ParentTransform;

                /* This happens when a stage added to root */
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
                        parentAdapter.AddChild(adapter, true);
                        //if (!PrefabUtil.IsCreatedFromPrefab(parentAdapter))
                        ParentChildLinker.Instance.Update(parentAdapter);
                    }
                }
            }
        }
    }
}
