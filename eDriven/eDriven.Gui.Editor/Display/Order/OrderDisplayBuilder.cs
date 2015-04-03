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
using eDriven.Gui.Editor.List;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Editor.Display.Order
{
    internal static class OrderDisplayBuilder
    {
        static readonly Color Dark = ColorMixer.FromHex(0x777777).ToColor();
        static readonly Color Light = ColorMixer.FromHex(0xEEEEEE).ToColor();

        public static ChildGroupRenderingList BuildContentGroups(GroupAdapter groupAdapter)
        {
            //Debug.Log("BuildContentGroups");
            ChildGroupRenderingList childGroupList = new ChildGroupRenderingList();

            var groupDescriptors = DesignerReflection.GetChildGroupsReferences(groupAdapter);
            bool skipFirst = false;
            if (groupDescriptors.Count > 0)
            {
                skipFirst = groupDescriptors[0].Attribute.ShowHeader;
            }

            foreach (ChildGroupDescriptor descriptor in groupDescriptors)
            {
                var attr = descriptor.Attribute;

                ChildGroup group;

                if (attr.ShowHeader)
                {
                    Texture iconTexture = null;

                    if (!string.IsNullOrEmpty(attr.Icon))
                        iconTexture = (Texture)Resources.Load(attr.Icon);

                    group = new ChildGroup(new GUIContent(attr.Label, iconTexture, attr.Tooltip));
                }
                else
                {
                    group = new ChildGroup();
                }

                List<ComponentAdapter> adapters = Core.Reflection.CoreReflector.GetMemberValue(descriptor.CollectionMemberInfo, groupAdapter) as List<ComponentAdapter>;

                // temp hack
                // ReSharper disable ConvertIfStatementToNullCoalescingExpression
                if (null == adapters)
                // ReSharper restore ConvertIfStatementToNullCoalescingExpression
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    adapters = new List<ComponentAdapter>(Core.Reflection.CoreReflector.GetMemberValue(descriptor.CollectionMemberInfo, groupAdapter) as ComponentAdapter[]);
                    // ReSharper restore AssignNullToNotNullAttribute
                }

                //Debug.Log("adapters.Count: " + adapters.Count);
                foreach (ComponentAdapter adapter in adapters)
                {
                    //Debug.Log("  -> adapter: " + adapter);
                    if (null != adapter)
                    {
                        var dataObject = new OrderDisplayRow(adapter.ToString(), adapter, new Rect(0, 0, 0, 0))
                        {
                            Color = (adapter is GroupAdapter) ? Dark : Light,
                            IsContainer = adapter is GroupAdapter
                        };

                        if (skipFirst)
                            dataObject.YMin = OrderDisplay.ElementHeight;

                        //Debug.Log("Adding " + dataObject);
                        group.Add(dataObject);
                    }
                }

//                Debug.Log(string.Format(@"***** Group created: 
//{0}
//
//", group));
                childGroupList.AddGroup(group);
            }

            //Debug.Log(childGroupList);

            return childGroupList;
        }
    }
}
