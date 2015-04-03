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
using System.Text;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEngine;

namespace eDriven.Gui.Editor.Reflection
{
    internal static class EditorReflector
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        public const BindingFlags AllInstanceMethodsBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static List<MethodInfo> GetMethodsBySignature(Type type, Type returnType, params Type[] parameterTypes)
        {
            var allMethods = new List<MethodInfo>(type.GetMethods(AllInstanceMethodsBindingFlags));

            return allMethods.FindAll(delegate(MethodInfo methodInfo)
                                          {
                                              if (methodInfo.ReturnType != returnType) return false;
                                              var parameters = methodInfo.GetParameters();
                                              if ((parameterTypes == null || parameterTypes.Length == 0))
                                                  return parameters.Length == 0;
                                              if (parameters.Length != parameterTypes.Length)
                                                  return false;
                                              for (int i = 0; i < parameterTypes.Length; i++)
                                              {
                                                  if (parameters[i].ParameterType != parameterTypes[i])
                                                      return false;
                                              }
                                              return true;
                                          });

            
        }

        public static List<PropertyInfo> GetPropertiesByType(Type type, Type propertyType)
        {
            var allProperties = new List<PropertyInfo>(type.GetProperties());

            //Debug.Log(string.Format("GetPropertiesByType [{0}, {1}]", type, propertyType));
            return allProperties.FindAll(delegate(PropertyInfo propertyInfo)
                                             {
                                                 //Debug.Log(string.Format("   [{0}, {1}]", propertyInfo.PropertyType, propertyType));
                                                 return propertyInfo.PropertyType == propertyType;
                                             });
        }

        public static PropertyInfo GetPropertyByTypeAndName(Type type, string name)
        {
            //Debug.Log(string.Format("GetPropertiesByType [{0}, {1}]", type, propertyType));
            return type.GetProperty(name);
        }

        private static List<MethodInfo> _methodList;

        public static bool ContainsEventHandlers(GameObject go)
        {
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is ComponentAdapter)
                    continue; // ignore component adapters

                _methodList = GetMethodsBySignature(component.GetType(), typeof(void), typeof(Core.Events.Event));
                if (_methodList.Count > 0)
                    return true;
            }
            return false;
        }

        public static bool ContainsEventHandlerScripts(GameObject go)
        {
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is ComponentAdapter)
                    continue; // ignore component adapters

                return true;
            }
            return false;
        }

        public static List<ScriptWithEventHandlers> GetEventHandlerScriptsPacked(GameObject go)
        {
            List<ScriptWithEventHandlers> list = new List<ScriptWithEventHandlers>();
            
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is ComponentAdapter)
                    continue; // ignore component descriptors

                ScriptWithEventHandlers mapping = new ScriptWithEventHandlers();
                mapping.AddRange(GetMethodsBySignature(component.GetType(), typeof(void), typeof(Event)));

                list.Add(mapping);
            }

            return list;
        }

        /*public static List<EventAttribute> GetEvents(Type componentType)
        {
            object[] list = componentType.GetCustomAttributes(typeof(EventAttribute), true);
            var events = new List<EventAttribute>();
            foreach (var e in list)
            {
                events.Add((EventAttribute) e);
            }
            return events;
        }*/

        private static void GetEventsRecursive(ComponentAdapter clickedAdapter, ComponentAdapter currentAdapter, ref Dictionary<string, EventAttribute> dict, bool bubbling, ICollection<ComponentAdapter> adaptersToExclude)
        {
            Type componentType = currentAdapter.ComponentType;

            if (null == adaptersToExclude || !adaptersToExclude.Contains(currentAdapter))
            {
                //object[] list = componentType.GetCustomAttributes(typeof (EventAttribute), true);
                var eventAttributes = CoreReflector.GetClassAttributes<EventAttribute>(componentType);

                foreach (EventAttribute attribute in eventAttributes)
                {
                    if (clickedAdapter == currentAdapter)
                    {
                        /**
                         * 1. If this is a clicked adapter, get all events
                         * */
                        dict[attribute.Name] = attribute;
                    }
                    else if (bubbling && attribute.Bubbles) // if (bubbling)
                    {
                        /**
                         * 2. Else get only events that may bubble from children
                         * */
                        dict[attribute.Name] = attribute;
                    }

                    /*if (!bubbling || attribute.Bubbles) // if (bubbling)
                    {
                        // bubbling events only
                        if (attribute.Bubbles)
                            dict[attribute.Name] = attribute;
                    }
                    else
                    {
                        // target events only
                        dict[attribute.Name] = attribute;
                    }*/
                    //Debug.Log(" --> " + attribute.Name);
                }
            }

            if (bubbling)
            {
                Transform transform = currentAdapter.transform;
                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var childTransform = transform.GetChild(i);
                    ComponentAdapter childAdapter = GuiLookup.GetAdapter(childTransform);
                    GetEventsRecursive(clickedAdapter, childAdapter, ref dict, true, adaptersToExclude);
                }
            }
        }

        public static Dictionary<string, EventAttribute> GetEvents(ComponentAdapter adapter)
        {
            Dictionary<string, EventAttribute> dict = new Dictionary<string, EventAttribute>();
            GetEventsRecursive(adapter, adapter, ref dict, false, null);
            return dict;
        }

        public static Dictionary<string, EventAttribute> GetBubblingEvents(ComponentAdapter adapter)
        {
            Dictionary<string, EventAttribute> dict = new Dictionary<string, EventAttribute>();
            GetEventsRecursive(adapter, adapter, ref dict, true, null);
            return dict;
        }

        public static Dictionary<string, EventAttribute> GetEventsBubblingFromChildren(ComponentAdapter adapter)
        {
            var excludeList = new List<ComponentAdapter> { adapter };
            Dictionary<string, EventAttribute> dict = new Dictionary<string, EventAttribute>();
            GetEventsRecursive(adapter, adapter, ref dict, true, excludeList);
            return dict;
        }

        public static string GetDefaultEventName(ComponentAdapter adapter)
        {
            var list = Core.Reflection.CoreReflector.GetClassAttributes<DefaultEvent>(adapter.ComponentType);
            if (list.Count == 0)
                return null;

            return list[0].Name;
        }

        //private static List<Type> _allTypes;
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

        public static bool IsUniqueScriptName(string className)
        {
            var types = GuiReflector.GetAllLoadedTypes();
            //Debug.Log(string.Format(@"nameToTest: {0}", className));
            //Debug.Log(string.Format(@"types: {0}", types.Count));
            bool found = types.Exists(delegate(Type t)
            {
                return /*t.Namespace == nameSpace && */t.Name == className;
            });
            return !found;
        }

        public static Type GetTypeByClassName(string className)
        {
            var types = GuiReflector.GetAllLoadedTypes();
            return types.Find(delegate(Type t)
                                  {
                                      return /*t.Namespace == nameSpace && */ t.Name == className;
                                  });
        }

        public static string CreateUniqueScriptName(string className)
        {
            //Debug.Log(string.Format(@"CreateUniqueScriptName className: {0}", className));
            const int max = 1000;
            int count = 0;
            string name = null;
            while (count < max && name == null)
            {
                var nameToTest = className + (count == 0 ? string.Empty : count.ToString());
                //Debug.Log(string.Format(@"nameToTest: {0}", nameToTest));
                //Debug.Log(string.Format(@"types: {0}", types.Count));
                bool isUnique = IsUniqueScriptName(nameToTest);

                if (isUnique)
                {
                    //Debug.Log(string.Format(@"Not found: {0}", nameToTest));
                    name = nameToTest;
                }

                count++;
            }

            if (count == max)
                Debug.LogWarning("Count == 1000. Consider choosing another script name");

            return name;
        }

        private static Dictionary<string, Type> _allTypesDict;

        public static Type GetTypeByFullName(string name)
        {
            if (null == _allTypesDict)
            {
                _allTypesDict = new Dictionary<string, Type>();
                //Type myType = Type.GetType(name);
                var allTypes = GuiReflector.GetAllLoadedTypes();
                foreach (Type type in allTypes)
                {
                    _allTypesDict[type.FullName] = type;
                }
            }

            if (_allTypesDict.ContainsKey(name))
                return _allTypesDict[name];

            return null;
        }
        
        private static List<Type> _mappableTypes;
        ///<summary>
        /// Returns all the styles that could be mapped via the style mapper
        ///</summary>
        ///<returns></returns>
        public static List<Type> MappableStyleTypes
        {
            get
            {
                if (null == _mappableTypes)
                {
                    _mappableTypes = new List<Type>
                                     {
                                         typeof(int), typeof(float), typeof(string), typeof(Texture), typeof(GUIStyle)
                                     };
                }

                return _mappableTypes;
            }
        }

        private static List<Type> _availableStyleableClasses;

        public static List<Type> GetAllStyleableClasses()
        {
            if (null == _availableStyleableClasses)
            {
                //Dictionary<string, Type> types = new Dictionary<string, Type>();

                var types = StyleReflector.GetAllStyleableClasses();
#if DEBUG
                if (DebugMode)
                {
                    StringBuilder sb = new StringBuilder();
                    if (types.Count == 0)
                    {
                        sb.AppendLine("No styleable classes available.");
                    }
                    else
                    {
                        foreach (Type type in types)
                        {
                            sb.AppendLine(string.Format("    -> {0}", type));
                        }
                    }

                    Debug.Log(string.Format(@"====== Styleable classes ======
{0}", sb));
                }
#endif
                _availableStyleableClasses = types;
                //Debug.Log("_availableStyleableClasses: " + _availableStyleableClasses.Count);
            }

            return _availableStyleableClasses;
        }

        private static readonly Dictionary<Type, List<StyleAttribute>> StyleAttributeCache = new Dictionary<Type, List<StyleAttribute>>();

        public static List<StyleAttribute> GetStyleAttributes(Type type)
        {
            //Debug.Log("GetStyleAttributes: " + className);
            if (StyleAttributeCache.ContainsKey(type))
                return StyleAttributeCache[type];

            //var type = GetTypeByFullName(theType.FullName);
            //Debug.Log("type: " + type);

            var attributes = StyleReflector.GetStyleAttributes(type); 
#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder();
                if (attributes.Count == 0)
                {
                    sb.AppendLine("No available style properties.");
                }
                else
                {
                    foreach (StyleAttribute styleAttribute in attributes)
                    {
                        sb.AppendLine(string.Format("    {0} -> {1}", styleAttribute.Name, styleAttribute.Type));
                    }
                }

                Debug.Log(string.Format(@"====== Styleable properties ======
{0}", sb));
            }
#endif
            StyleAttributeCache[type] = attributes;

            return attributes;
        }

        /// <summary>
        /// Gets style properties (attributes converted to style properties)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restrictToSerializableTypes"></param>
        /// <returns></returns>
        public static List<StyleProperty> GetStyleProperties(Type type, bool restrictToSerializableTypes = false)
        {
            var attributes = GetStyleAttributes(type);

            List<StyleProperty> list = new List<StyleProperty>();
            foreach (var attribute in attributes)
            {
                /* with "skinClass" style, the Type isn't required. Set it here */
                if (attribute.Name == "skinClass"/* && null == attribute.Type*/) { 
                    attribute.Type = typeof(string);
                    attribute.Default = null;
                }
                
                if (restrictToSerializableTypes && StyleProperty.NonSerializableStyleTypes.Contains(attribute.Type))
                    continue;

                //if (!restrictToSerializableTypes || StyleProperty.AlowedTypes.ContainsKey(attribute.GetType()))
                try
                {
                    list.Add(StyleProperty.FromAttribute(attribute));
                }
                catch (StylePropertyCreationException ex)
                {
                    // cannot be created
                }
            }

            return list;
        }

        /// <summary>
        /// Used by designer
        /// </summary>
        /// <param name="componentType"></param>
        public static List<Type> GetSkinClasses(Type componentType)
        {
            List<Type> list = new List<Type>();

            //Debug.Log("componentType: " + componentType);
            if (!componentType.IsSubclassOf(typeof(SkinnableComponent)))
            {
                //Debug.LogError("Component is not a subclass of SkinnableComponent: " + componentType);
                return list;
            }

            List<Type> types = GuiReflector.GetAllLoadedTypes();

            foreach (Type type in types)
            {
                if (!type.IsClass)
                    continue;

                if (!type.IsSubclassOf(typeof (Skin)))
                    continue;
                
                var componentTypeSpecifiedInAttribute = SkinUtil.GetHostComponent(type);
                if (componentTypeSpecifiedInAttribute == componentType)
                {
                    list.Add(type);
                }
            }

            return list;
        }

        /*public static Type GetHostComponent(Type skinType)
        {
            var hostComponentAttributes = CoreReflector.GetClassAttributes<HostComponentAttribute>(skinType);

                //Debug.Log("hostComponentAttributes.Length: " + hostComponentAttributes.Length);

            if (hostComponentAttributes.Count > 0)
            {
                var attr = hostComponentAttributes[0];
                return attr.Type;
            }
            return null;
        }*/

        /// <summary>
        /// Used by designer
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="dict"></param>
        public static void GetSkinClasses(Type componentType, ref Dictionary<string, Type> dict)
        {
            List<Type> types = GetSkinClasses(componentType);
            foreach (Type type in types)
            {
                dict.Add(type.FullName, type);
            }
        }

        /// <summary>
        /// Describes component skins
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static SkinReflectedData GetSkins(Type componentType)
        {
            var data = new SkinReflectedData();
            if (!typeof(SkinnableComponent).IsAssignableFrom(componentType))
            {
                return data;
            }

            /**
             * 1. Det the default skin
             * */
            var styles = GetStyleAttributes(componentType);
            var defaultStyleAttribute = styles.Find(delegate(StyleAttribute attr)
            {
                return attr.Name == "skinClass";
            });

            /**
             * 2. Get all the skins pointing to this component type via the HostComponentAttribute
             * */
            var skins = GetSkinClasses(componentType);
            skins.Sort(ClassNameSort);

            if (null != defaultStyleAttribute)
            {
                var skinType = defaultStyleAttribute.GetDefault();
                if (null != skinType)
                {
                    var stringType = skinType as string;
                    if (stringType != null)
                    {
                        if (string.IsNullOrEmpty(stringType))
                        {
                            throw new Exception(@"""skinClass"" attribute is an empty string");
                        }

                        if (!GlobalTypeDictionary.Instance.ContainsKey(stringType))
                        {
                            throw new Exception(@"""skinClass"" attribute is of type string, but it cannot be found in the type dictionary: " + stringType);
                        }

                        skinType = GlobalTypeDictionary.Instance[stringType];
                    }
                    /*else if (skinType.GetType() != typeof (Type))
                    {
                        Debug.Log(skinType.GetType());
                        throw new Exception(@"""skinClass"" attribute must be of type Type or string");
                    }*/

                    data.Default = (Type) skinType;
                }
            }

            foreach (var skin in skins)
            {
                data.DirectSkins.Add(skin);
            }
            
            return data;
        }

        private static int ClassNameSort(Type x, Type y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        /*public static Dictionary<string, Type> GetSkinParts(Type componentType)
        {
            Dictionary<string, Type> dict = new Dictionary<string, Type>();
            if (!typeof(Skin).IsAssignableFrom(componentType))
            {
                return dict;
            }

            GuiReflector.GetMethodsDecoratedWith(componentType, typeof(SkinPartAttribute));

            return dict;
        }*/
    }
}