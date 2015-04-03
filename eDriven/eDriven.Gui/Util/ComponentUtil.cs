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
using System.Text;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Util
{
    public class ComponentUtil
    {
        /// <summary>
        /// Gets a parent chain
        /// Example: If out component is in a chain Stage-A-B-C-Component, this method returns the list of: Stage, A, B, C.
        /// </summary>
        /// <param name="component">Compo</param>
        /// <param name="reverse">Reverse the chain</param>
        /// <returns></returns>
        public static List<DisplayListMember> GetParentChain(DisplayListMember component, bool reverse)
        {
            if (null == component)
                throw new Exception("Component not defined");

            List<DisplayListMember> list = new List<DisplayListMember>();
            DisplayListMember current = component;

            //list.Add(current); // removed on 2011-09-18

            while (!(current is Stage) && null != current.Parent)
            {
                current = current.Parent;
                list.Add(current);
            }

            if (reverse)
                list.Reverse();

            return list;
        }

        /// <summary>
        /// Prints out the component list
        /// </summary>
        /// <param name="collection"></param>
        public static string DescribeComponentList(List<Component> collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Component child in collection)
            {
                sb.AppendLine(string.Format("    -> {0}", child));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Prints out the component list
        /// </summary>
        /// <param name="collection"></param>
        public static string DescribeComponentList(List<Component> collection, bool describePaths)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Component child in collection)
            {
                sb.AppendLine(describePaths ? 
                    PathToString(child, "->") :
                    string.Format("    -> {0}", child)
                );
            }
            return sb.ToString();
        }

        ///<summary>
        ///</summary>
        ///<param name="component"></param>
        ///<param name="delimiter"></param>
        ///<returns></returns>
        public static string PathToString(Component component, string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
                delimiter = ".";

            /**
             * 1. Build path
             * */
            List<Component> list = new List<Component>();

            while (null != component)
            {
                list.Add(component);
                component = component.Parent as Component;
            }

            list.Reverse();

            string name = "";
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                name += list[i].Name;
                if (i < count - 1)
                    name += delimiter;
            }

            return name;
        }

        ///// <summary>
        ///// Utility method for getting the string presentation of component path
        ///// </summary>
        ///// <param name="component"></param>
        ///// <param name="delimiter">Delimiter used when building the string</param>
        ///// <returns></returns>
        //public static string PathToString(Component component, string delimiter)
        //{
        //    var list = GetParentChain(component, true);
        //    if (string.IsNullOrEmpty(delimiter))
        //        delimiter = ".";

        //    string name = "";
        //    int count = list.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        name += list[i].Name;
        //        if (i < count - 1)
        //            name += delimiter;
        //    }

        //    return name;
        //}

        ///// <summary>
        ///// Prints out the DisplayListMember list
        ///// </summary>
        ///// <param name="collection"></param>
        //public static string DescribeMemberList(List<DisplayListMember> collection)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (DisplayListMember child in collection)
        //    {
        //        sb.AppendLine(string.Format("    -> {0}", child));
        //    }
        //    return sb.ToString();
        //}
    }
}
