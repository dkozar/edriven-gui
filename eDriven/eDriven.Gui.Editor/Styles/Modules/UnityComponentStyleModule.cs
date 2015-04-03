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
using eDriven.Core.Reflection;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using eDriven.Gui.Styles;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    /// <summary>
    /// Handles reflecting of Unity components and building collections needed for the StyleDeclaration dialog
    /// </summary>
    [StyleModule("unity", "Unity components", "unity_component")]
    internal class UnityComponentStyleModule : StyleModuleBase
    {
        private List<TypeDescriptor> _descriptors;

        /*public Type TraverserType
        {
            get
            {
                return typeof (UnityComponentTraverser);
            }
        }*/

        public override IComponentTraverser Traverser
        {
            get
            {
                return UnityComponentTraverser.Instance;
            }
        }

        public override List<TypeDescriptor> GetComponentDescriptors()
        {
            if (null != _descriptors) return _descriptors;

            _descriptors = new List<TypeDescriptor>();

            var types = StyleSheetsReflector.GetAllUnityComponents();

            foreach (Type type in types)
            {
                var exists = _descriptors.Exists(delegate(TypeDescriptor descriptor)
                {
                    return descriptor.Type == type;
                });

                if (!exists)
                    _descriptors.Add(new TypeDescriptor(type.FullName, type, GetComponentIcon(type)));
            }

            return _descriptors;
        }
        
        /// <summary>
        /// Returns the collection of available styles for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Dictionary<string, MemberDescriptor> GetStyleDescriptors(Type type)
        {
            // we should reflect a given component type and get properties and fields
            //return EditorReflector.GetStyleProperties(type, true); // restrict

            Dictionary<string, MemberDescriptor> output = new Dictionary<string, MemberDescriptor>();

            // 1. getting all public fields
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                var name = fieldInfo.Name;
                object @default = null;
                var styleAttributes = CoreReflector.GetMemberAttributes<StyleAttribute>(fieldInfo);
                if (styleAttributes.Count > 0)
                {
                    @default = styleAttributes[0].GetDefault();
                    /*if (!string.IsNullOrEmpty(styleAttributes[0].Name))
                        name = styleAttributes[0].Name; // attribute value overrides the name*/
                }
                
                var prop = new MemberDescriptor(name, fieldInfo.FieldType, GetStyleIcon(fieldInfo.FieldType), @default);// StyleProperty.CreateProperty(fieldInfo.FieldType);
                output[fieldInfo.Name] = prop;
            }

            // 2. getting all public properties having a *setter*
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                //if (propertyInfo.GetSetMethod() == null)
                if (!propertyInfo.CanWrite)
                    continue; // get only writeable propertis

                var name = propertyInfo.Name;
                object @default = null;
                var styleAttributes = CoreReflector.GetMemberAttributes<StyleAttribute>(propertyInfo);
                if (styleAttributes.Count > 0)
                {
                    @default = styleAttributes[0].GetDefault();
                    /*if (!string.IsNullOrEmpty(styleAttributes[0].Name))
                        name = styleAttributes[0].Name; // attribute value overrides the name*/
                }

                var prop = new MemberDescriptor(name, propertyInfo.PropertyType, GetStyleIcon(propertyInfo.PropertyType), @default);// StyleProperty.CreateProperty(fieldInfo.FieldType);
                output[propertyInfo.Name] = prop;
            }

            return output;
        }

        public override Texture GetComponentIcon(Type type)
        {
            //return TextureCache.Instance.UnityComponent;
            return UnityComponentEvaluator.EvaluateComponentRowIcon(type);
        }

        public override Texture GetStyleIcon(Type type)
        {
            //Debug.Log("GetStyleIcon for " + type);
            return AssetPreview.GetMiniTypeThumbnail(type) ?? 
                TextureCache.Instance.Property;
        }
    }
}