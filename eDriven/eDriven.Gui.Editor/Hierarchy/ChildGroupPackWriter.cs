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

using eDriven.Gui.Designer.Adapters;
using UnityEngine;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal static class ChildGroupPackWriter
    {
        public static void Write(GroupAdapter parentAdapter, ChildGroupPack pack)
        {
            ChildGroupPack currentGroupPack = ChildGroupPack.Read(parentAdapter);
            Debug.Log(string.Format("ParentAdapter: {0}, {1}", parentAdapter,
                                    parentAdapter.GetInstanceID()));

            for (int i = 0; i < currentGroupPack.Groups.Count; i++)
            {
                var currentGroup = currentGroupPack.Groups[i];
                Debug.Log("newGroup.Adapters: " + currentGroup.Adapters.Count);
                currentGroup.Adapters.Clear();
                //Debug.Log("newGroup.Adapters 2: " + newGroup.Adapters.Count);
                currentGroup.Adapters.AddRange(pack.Groups[i].Adapters);
                Debug.Log("newGroup.Adapters 2: " + currentGroup.Adapters.Count);
            }
        }
    }
}