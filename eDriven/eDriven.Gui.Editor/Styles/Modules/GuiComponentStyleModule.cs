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
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    /// <summary>
    /// Handles reflecting of styles and building collections needed for the StyleDeclaration dialog
    /// </summary>
    [StyleModule("gui", "Styleable components", "component", 
        AllowMultipleClients = true, AllowSubjectOmmision = true, ProcessEditModeChanges = false)]
    internal class GuiComponentStyleModule : StyleModuleBase
    {
        private List<TypeDescriptor> _typeNameList;

        /*public Type TraverserType
        {
            get
            {
                return typeof (GuiComponentTraverser);
            }
        }*/

        public override IComponentTraverser Traverser
        {
            get
            {
                return GuiComponentTraverser.Instance;
            }
        }

        public override List<TypeDescriptor> GetComponentDescriptors()
        {
            if (null != _typeNameList) return _typeNameList;

            _typeNameList = new List<TypeDescriptor>();

            var types = EditorReflector.GetAllStyleableClasses();
            foreach (var type in types)
            {
                var exists = _typeNameList.Exists(delegate(TypeDescriptor descriptor)
                {
                    return descriptor.Type == type;
                });

                if (!exists)
                    _typeNameList.Add(new TypeDescriptor(type.FullName, type, GetComponentIcon(type)));
            }

            return _typeNameList;
        }

        /// <summary>
        /// Returns the collection of available styles for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Dictionary<string, MemberDescriptor> GetStyleDescriptors(Type type)
        {
            //return EditorReflector.GetStyleProperties(type, true); // restrict
            var attributes = EditorReflector.GetStyleAttributes(type);
            Dictionary<string, MemberDescriptor> dict = new Dictionary<string, MemberDescriptor>();

            foreach (var attribute in attributes)
            {
                if (StyleProperty.NonSerializableStyleTypes.Contains(attribute.Type))
                    continue; // skip

                if (null == attribute.Type)
                    attribute.Type = typeof (Type);
                dict[attribute.Name] = new MemberDescriptor(attribute.Name, attribute.Type, GetStyleIcon(attribute.Type)); // no value needed
            }
            return dict;
        }

        public override Texture GetComponentIcon(Type type)
        {
            return GuiComponentEvaluator.EvaluateComponentRowIcon(type); //return TextureCache.Instance.Component;
        }

        public override Texture GetStyleIcon(Type type)
        {
            return TextureCache.Instance.Style;
        }
    }
}
