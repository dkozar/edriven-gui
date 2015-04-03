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
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Util;
using eDriven.Gui.Util;
using UnityEditor;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class HierarchyDescriber
    {
        /// <summary>
        /// Recursive!
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="node"></param>
        public static void DescribeChildren(ref StringBuilder sb, Node node, bool richText)
        {
            foreach (Node childNode in node.ChildNodes)
            {
                string name = null == childNode.Transform ? "New" : childNode.Transform.name;
                sb.AppendLine(StringUtil.Indent(childNode.Depth/* + 1*/, string.Format("{0} [A:{1}]", name, childNode.AdapterId)));
                DescribeChildren(ref sb, childNode, richText);
            }
        }

        public static void DescribeChildrenWithGroups(ref StringBuilder sb, Node node, bool richText)
        {
            var childNodes = node.ChildNodes;

            var unprocessedNodes = ListUtil<Node>.Clone(node.ChildNodes);

            var adapter = node.Adapter;

            //Debug.Log(@"adapter: " + adapter);
            var containerAdapter = adapter as GroupAdapter;

            // Note: the ROOT node has no adapter nor groups defined!
            if (null != containerAdapter)
            {
                ChildGroupPack pack = ChildGroupPack.Read(containerAdapter);
                //                Debug.Log(@"pack: 
                //" + pack);
                foreach (ChildGroup childGroup in pack.Groups)
                {
                    sb.AppendLine(StringUtil.Indent(node.Depth + 1, string.Format("== {0} ==", childGroup.GroupName)));
                    foreach (ComponentAdapter componentAdapter in childGroup.Adapters)
                    {
                        var childNode = GetNode(childNodes, componentAdapter);
                        if (null == childNode)
                        {
                            var line = StringUtil.Indent(node.Depth + 1, string.Format("*** Not found ***"));
                            if (richText)
                                line = (StringUtil.WrapColor(StringUtil.WrapTag(line, "b"), EditorSettings.UseDarkSkin ? "yellow" : "blue"));
                            sb.AppendLine(line);
                        }
                        else
                        {
                            unprocessedNodes.Remove(childNode);
                            string name = null == childNode.Transform ? "New" : childNode.Transform.name;

                            var isSelected = Selection.activeGameObject == componentAdapter.gameObject;
                            string selected = isSelected ? "*" : string.Empty;
                            string changed = ParentChildLinker.Instance.IsChanged(childNode.AdapterId) ? "[changed]" : string.Empty;

                            var line = StringUtil.Indent(childNode.Depth /* + 1*/,
                                string.Format("{2}{0} [A:{1}]{3}", name, childNode.AdapterId, selected, changed));

                            if (richText && isSelected)
                                line = StringUtil.WrapTag(line, "b");

                            sb.AppendLine(line);
                            DescribeChildrenWithGroups(ref sb, childNode, richText);
                        }
                    }
                }
            }

            if (unprocessedNodes.Count > 0 && null != node.ParentTransform)
            {
                sb.AppendLine(StringUtil.Indent(node.Depth + 1, string.Format("== Orphans ==")));
            }

            // orphans
            foreach (Node childNode in unprocessedNodes)
            {
                string name = null == childNode.Transform ? "New" : childNode.Transform.name;
                var isSelected = (Selection.activeGameObject == childNode.Adapter.gameObject);
                string selected = isSelected ? "*" : string.Empty;
                string changed = ParentChildLinker.Instance.IsChanged(childNode.AdapterId) ? "[changed]" : string.Empty;

                bool isOrphan = !(childNode.Adapter is StageAdapter);
                string orphan = isOrphan ? "[orphan] " : string.Empty;
                //string orphan = StringUtil.WrapColor((childNode.Adapter is StageAdapter) ? string.Empty : "[orphan] ",
                    //EditorSettings.UseDarkSkin ? "yellow" : "blue");

                var line = StringUtil.Indent(childNode.Depth/* + 1*/, string.Format("{4}{2}{0} [A:{1}]{3}", name, childNode.AdapterId, selected, changed, orphan));
                if (isOrphan)
                    line = StringUtil.WrapColor(StringUtil.WrapTag(line, "b"), EditorSettings.UseDarkSkin ? "yellow" : "blue");

                if (richText && isSelected)
                    line = StringUtil.WrapTag(line, "b");

                sb.AppendLine(line);
                DescribeChildrenWithGroups(ref sb, childNode, richText);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="adapter"></param>
        /// <returns></returns>
// ReSharper disable SuggestBaseTypeForParameter
        private static Node GetNode(List<Node> nodes, ComponentAdapter adapter)
// ReSharper restore SuggestBaseTypeForParameter
        {
            return nodes.Find(delegate(Node node)
            {
                return node.Adapter == adapter;
            });
        }
    }
}
