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

namespace eDriven.Gui.Editor.Building
{
    /// <summary>
    /// Processes edit mode removals
    /// When the child is removed from parent, child lists of the parent 
    /// must be consolidated (null references removed) 
    /// </summary>
    internal static class EditModeRemovalProcessor
    {
        internal static void Process(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return;

            //Debug.Log(string.Format("Processing {0} top level removals in edit mode.", nodes.Count));

            foreach (Node node in nodes)
            {
                if (null == node.ParentTransform)
                    continue;

                GroupAdapter parentGroupAdapter = GuiLookup.GetAdapter(node.ParentTransform) as GroupAdapter;
                if (null != parentGroupAdapter)
                {
                    /**
                     * Important: the removal could happen when we *move* components outside of the Stage hierarchy
                     * From group's standpoint, these components have been *removed*
                     * However, the adapters are not null, so the consolidation without a prior
                     * removal would do nothing to the parent collection (e.g. the moved adapter
                     * would still be on the list)
                     * */
                    parentGroupAdapter.RemoveChild(node.Adapter);
                    
                    ChildGroupPack pack = ChildGroupPack.Read(parentGroupAdapter);
                    pack.Consolidate(); // there is a null slot here. We have to re-render children
                }
            }
        }
    }
}
