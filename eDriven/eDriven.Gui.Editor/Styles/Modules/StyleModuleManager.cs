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
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    /// <summary>
    /// Handles the cached collection of loaded styling modules<br/>
    /// Loads available modules once, during the initialization
    /// </summary>
    internal sealed class StyleModuleManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleModuleManager _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleModuleManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleModuleManager Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleModuleManager instance"));
#endif
                    _instance = new StyleModuleManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private readonly List<StyleModuleBase> _allModules = new List<StyleModuleBase>();

        internal List<StyleModuleBase> AllModules
        {
            get { return _allModules; }
        }

        internal List<StyleModuleBase> ModulesVisibleInStyleDialog
        {
            get
            {
                return _allModules.FindAll(delegate(StyleModuleBase module)
                {
                    return module.Enabled;
                });
            }
        }

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            _allModules.Clear();

            //var providerTypes = ReflectionUtil.GetAllLoadedTypesDecoratedWith(typeof (StyleModuleAttribute));
            var moduleTypes = GuiReflector.GetAllLoadedTypesAsignableFrom(typeof(StyleModuleBase));
            foreach (var moduleType in moduleTypes)
            {
                if (moduleType == typeof (StyleModuleBase))
                    continue; // skip the abstract class

                var module = (StyleModuleBase)Activator.CreateInstance(moduleType);
                module.ReflectAttribute();

                _allModules.Add(module);
            }
        }

        /// <summary>
        /// Returns the parent pack for a descriptor
        /// This is a rare operation and happens when the user chooses
        /// the type in the first step of the wizard and goes to the second step
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public StyleModuleBase GetOwnerModule(Type type)
        {
            foreach (var module in _allModules)
            {
                var exists = module.GetComponentDescriptors().Exists(delegate(TypeDescriptor descriptor)
                {
                    return descriptor.Type == type;
                });

                if (exists)
                    return module;
            }
            throw new Exception("Couldn't find the type pack for descriptor: " + type); // shouldn't happen
        }

        public StyleModuleBase GetOwnerModule(string type)
        {
            return GetOwnerModule(GlobalTypeDictionary.Instance.Get(type));
        }

        public StyleModuleBase GetModule(string id)
        {
            return _allModules.Find(delegate(StyleModuleBase module)
            {
                return module.Id == id;
            });
            /*var descriptor = GetModuleDescriptor(id);
            return null == descriptor ? null : descriptor.Module;*/
        }

        #region Search

        private readonly List<TypeDescriptor> _typeDescriptorList = new List<TypeDescriptor>();

        /// <summary>
        /// Component search
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<TypeDescriptor> Search(string searchText, List<string> ids = null)
        {
            _typeDescriptorList.Clear();

            //var index = 0;
            foreach (StyleModuleBase module in _allModules)
            {
                var typeDescriptors = module.GetComponentDescriptors();

                foreach (var s in typeDescriptors)
                {
                    if (null != ids && !ids.Contains(module.Id))
                        continue;

                    if (!string.IsNullOrEmpty(searchText) && !s.Type.FullName.ToUpper().Contains(searchText.ToUpper()))
                        continue;

                    _typeDescriptorList.Add(s);
                    /*if (s.Type.FullName == _typeToFind)
                    {
                        //_selectedIndex = index;
                        _typeToFind = null;
                    }*/
                    //index++;
                }
            }

            _typeDescriptorList.Sort(StyleableTypeDescriptorSort);

            return _typeDescriptorList;
        }

        #endregion

        #region Helper

        private int StyleableTypeDescriptorSort(TypeDescriptor x, TypeDescriptor y)
        {
            return String.CompareOrdinal(x.Type.FullName, y.Type.FullName);
        }

        #endregion
    }
}
