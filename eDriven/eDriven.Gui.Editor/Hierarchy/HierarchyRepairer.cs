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

using System.Text;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Display;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal static class HierarchyRepairer
    {
        public static void Fix(Node node)
        {
            /**
             * 1. Just in case it got stucked to value other then 0 (not related)
             * */
            EditorSettings.ReadyToProcessHierarchyChanges = 0;

            /**
             * 2. Fix hierarchy
             * */
            StringBuilder sb = new StringBuilder();
            DoFix(ref sb, node);

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                Debug.Log(string.Format(@"### FIXED ### 
{0}", sb));
            }

            // TEMP:
            OrderDisplay.Instance.Refresh();
        }

        /// <summary>
        /// Recursive!
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="node"></param>
        private static void DoFix(ref StringBuilder sb, Node node)
        {
            bool isRoot = (null == node.Adapter);
            //if (!isRoot)
            //    Debug.Log(StringUtil.Indent(node.Depth, "Fixing " + node.Adapter));

            var childNodes = node.ChildNodes;

            var unprocessedNodes = ListUtil<Node>.Clone(node.ChildNodes);
            
            var adapter = node.Adapter;

            //Debug.Log(@"adapter: " + adapter);
            var containerAdapter = adapter as GroupAdapter;
            //Debug.Log(@"containerAdapter: " + containerAdapter);

            // Note: the ROOT node has no adapter no groups defined!

            if (null != containerAdapter) {
                ChildGroupPack pack = ChildGroupPack.Read(containerAdapter);
                // Debug.Log(@"pack: 
                //" + pack);
                foreach (ChildGroup childGroup in pack.Groups)
                {
                    //sb.AppendLine(StringUtil.Indent(node.Depth + 1, string.Format("== {0} ==", childGroup.GroupName)));
                    //foreach (ComponentAdapter componentAdapter in childGroup.Adapters)
                    for (int i = childGroup.Adapters.Count - 1; i >= 0; i--)
                    {
                        ComponentAdapter componentAdapter = childGroup.Adapters[i];

                        bool doRemove = false;

                        /**
                         * 1. Handle null
                         * */
                        if (null == componentAdapter)
                        {
                            //sb.AppendLine(StringUtil.Indent(node.Depth + 1, "*** Not found ***"));
                            // adapter is not child of this container, remove it from the list
                            //toRemove.Add(componentAdapter);
                            doRemove = true;
                        }
                        /**
                         * 2. Not null. Handle the adapter
                         * */
                        else
                        {
                            var childNode = GetNode(childNodes, componentAdapter);
                            if (null == childNode)
                            {
                                //sb.AppendLine(StringUtil.Indent(node.Depth + 1, "*** Not found ***"));
                                // adapter is not child of this container, remove it from the list
                                //toRemove.Add(componentAdapter);
                                doRemove = true;
                            }
                            else
                            {
                                unprocessedNodes.Remove(childNode);
                                DoFix(ref sb, childNode);
                            }
                        }

                        if (doRemove)
                        {
                            //Debug.Log("list 1: " + ComponentAdapterUtil.DescribeAdapterList(childGroup.Adapters));
                            childGroup.Adapters.RemoveAt(i);
                            //childGroup.Adapters.Remove(adapterToRemove);
                            if (null != componentAdapter)
                                sb.AppendLine(string.Format("{0}: {1} [A:{2}] removed", GuiLookup.PathToString(containerAdapter.transform, "->"), componentAdapter, componentAdapter.GetInstanceID()));
                            else
                                sb.AppendLine(string.Format("{0}: Adapter at position {1} removed", GuiLookup.PathToString(containerAdapter.transform, "->"), i));
                            //Debug.Log("list 2: " + ComponentAdapterUtil.DescribeAdapterList(childGroup.Adapters));
                        }
                    }
                }
            }

            // orphans
            foreach (Node childNode in unprocessedNodes)
            {
                if (!isRoot)
                {
                    if (null != containerAdapter)
                    {
                        ChildGroupPack pack = ChildGroupPack.Read(containerAdapter);
                        if (pack.Groups.Count > 0)
                        {
                            pack.Groups[0].Adapters.Add(childNode.Adapter);
                            sb.AppendLine(string.Format("{0}: [{1}] added to the first group", GuiLookup.PathToString(containerAdapter.transform, "->"), childNode.Adapter));    
                        }
                    }
                }
                DoFix(ref sb, childNode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="adapter"></param>
        /// <returns></returns>
        // ReSharper disable SuggestBaseTypeForParameter
        private static Node GetNode(System.Collections.Generic.List<Node> nodes, ComponentAdapter adapter)
        // ReSharper restore SuggestBaseTypeForParameter
        {
            return nodes.Find(delegate(Node node)
            {
                return node.Adapter == adapter;
            });
        }
    }
}
