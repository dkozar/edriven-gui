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
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal static class ToolboxInitializer
    {
        /// <summary>
        /// Looks for all descriptor classes in running assemblies via reflection
        /// </summary>
        public static void Initialize()
        {
            //Debug.Log("Initialize");

            var toolbox = Toolbox.Instance;

            toolbox.Clear();

            /**
             * 1. Get all types for all loaded assemblies
             * This is done only when componet tab expanded, so is no performance issue
             * */
            List<Type> types = GuiReflector.GetAllLoadedTypes();

            foreach (Type type in types)
            {
                if (type.IsClass)
                {
                    if (type.IsSubclassOf(typeof(ComponentAdapter)))
                    {
                        string icon = string.Empty;
                        string label = string.Empty;
                        string groupName = string.Empty;
                        string tooltip = string.Empty;
                        
                        //ToolboxAttribute[] toolboxAttributes = (ToolboxAttribute[])type.GetCustomAttributes(typeof(ToolboxAttribute), true);
                        var toolboxAttributes = Core.Reflection.CoreReflector.GetClassAttributes<ToolboxAttribute>(type);

                        if (toolboxAttributes.Count > 0)
                        {
                            var attr = toolboxAttributes[0];
                            label = attr.Label;
                            icon = attr.Icon;
                            tooltip = attr.Tooltip;
                            groupName = attr.Group;
                        }

                        Texture iconTexture = null;

                        if (!string.IsNullOrEmpty(icon))
                            iconTexture = (Texture)Resources.Load(icon);

                        if (string.IsNullOrEmpty(label))
                            label = type.Name.Replace("Adapter", string.Empty);

                        var desc = new ComponentTypeDesc(label, type, iconTexture, tooltip);

                        if (type.IsAbstract)
                            continue; // skip abstract classes (SkinnableComponentAdapter etc.)

                        /**
                         * 1. Stage
                         * */
                        if (type == typeof(StageAdapter))
                        {
                            Toolbox.Instance.StageContent = desc.GetContent();
                            continue;
                        }

                        /**
                         * 2. Named group
                         * */
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            desc.Group = groupName;
                            toolbox.Add(desc);
                            continue;
                        }

                        /**
                         * 3. Group
                         * */
                        //if (type == typeof (GroupAdapter) || type.IsSubclassOf(typeof (GroupAdapter)))
                        if (typeof(GroupAdapter).IsAssignableFrom(type))
                        {
                            desc.Group = "Containers";
                            toolbox.Add(desc);
                            continue;
                        }

                        /**
                         * 4. Component
                         * */
                        desc.Group = "Components";
                        toolbox.Add(desc);
                    }
                }
            }

            toolbox.Process();
        }
        
        /// <summary>
        /// A test for could the component of a particular type be instantiated 
        /// as a child of the component selected in the hierarchy
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ControlCouldBeInstantiated(Component transform, Type type)
        {
            bool componentSelected = (null != transform &&
                                      null != transform.GetComponent(typeof(ComponentAdapter)));

            /**
             * 1) StageAdapter could be intantiated only when nothing is selected
             * */
            if (type == typeof(StageAdapter))
                return !componentSelected; // alow stage creation on non-components

            /**
             * 2) If nothing selected, return
             * */
            if (null == transform)
                return false;

            /**
             * 3) Other descriptors could be instantiated if ContainerAdapter selected
             * */
            if (null != transform.GetComponent(typeof(GroupAdapter)))
                return true;

            return false;
        }
    }
}