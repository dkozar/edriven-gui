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
using System.Reflection;
using eDriven.Gui.Designer.Adapters;

namespace eDriven.Gui.Designer
{
    /// <summary>
    /// </summary>
    public static class DesignerReflection
    {
        ///<summary>
        ///</summary>
        ///<param name="adapter"></param>
        ///<returns></returns>
        public static List<ChildGroupDescriptor> GetChildGroupsReferences(GroupAdapter adapter)
        {
            Type adapterType = adapter.GetType();
            Type componentType = adapter.ComponentType;
            List<ChildGroupDescriptor> descriptors = new List<ChildGroupDescriptor>();

            foreach (MemberInfo memberInfo in adapterType.GetMembers())
            {
                //var attributes = memberInfo.GetCustomAttributes(typeof(ChildCollectionAttribute), true);

                var attributes = Core.Reflection.CoreReflector.GetMemberAttributes<ChildCollectionAttribute>(memberInfo);

                if (null != attributes && attributes.Count > 0)
                {
                    ChildCollectionAttribute attribute = attributes[0];
                    MemberInfo mi = null;
                    if (!string.IsNullOrEmpty(attribute.TargetContainer)) {
                        MemberInfo[] list = componentType.GetMember(attribute.TargetContainer);
                        if (list.Length > 0)
                            mi = list[0];
                    }
                    
                    ChildGroupDescriptor descriptor = new ChildGroupDescriptor
                    {
                        Attribute = attribute,
                        CollectionMemberInfo = memberInfo,
                        TargetContainerMemberInfo = mi
                    };
                    descriptors.Insert(0, descriptor);
                }
            }

            return descriptors;
        }

        ///// <summary>
        ///// Gets all skin classes
        ///// </summary>
        ///// <param name="dict"></param>
        //public static void GetAllStyleableClasses(ref Dictionary<string, Type> dict)
        //{
        //    List<Type> types = GetAllLoadedTypes();

        //    foreach (Type type in types)
        //    {
        //        if (type.IsClass)
        //        {
        //            if (typeof(IStyleClient).IsAssignableFrom(type)) {
        //                StyleAttribute[] styleAttributes = (StyleAttribute[])type.GetCustomAttributes(typeof(StyleAttribute), true);

        //                //Debug.Log("hostComponentAttributes.Length: " + hostComponentAttributes.Length);
        //                if (styleAttributes.Length > 0)
        //                {
        //                    //Debug.Log(3);
        //                    dict.Add(type.FullName, type);
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets all style properties
        ///// </summary>
        ///// <param name="type"></param>
        //public static List<StyleAttribute> GetStyleAttributes(Type type)
        //{
        //    //Debug.Log("GetStyleAttributes: " + type);

        //    StyleAttribute[] styleAttributes = (StyleAttribute[])type.GetCustomAttributes(typeof(StyleAttribute), true);
        //    //Debug.Log("styleAttributes.Length: " + styleAttributes.Length);
            
        //    List<StyleAttribute> attributes = new List<StyleAttribute>();
        //    foreach (StyleAttribute attribute in styleAttributes)
        //    {
        //        //Debug.Log("attribute: " + attribute.Name + "; " + attribute.Type);
        //        if (null != attribute.Type && StyleProperty.AlowedTypes.ContainsKey(attribute.Type))
        //            attributes.Add(attribute);
        //    }
        //    //Debug.Log("Allowed attributes: " + attributes.Count);

        //    return attributes;
        //}

        //private static List<Type> _allTypes;
        ///// <summary>
        ///// Gets all loaded types from all assemblies
        ///// </summary>
        ///// <returns></returns>
        //public static List<Type> GetAllLoadedTypes()
        //{
        //    /**
        //     * 1. Get all types for all loaded assemblies
        //     * This is done only when componet tab expanded, so is no performance issue
        //     * */

        //    if (null == _allTypes)
        //    {
        //        _allTypes = new List<Type>();
                
        //        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        //        foreach (Assembly assembly in loadedAssemblies)
        //        {
        //            _allTypes.AddRange(assembly.GetTypes());
        //        }
        //    }

        //    return _allTypes;
        //}
    }
}