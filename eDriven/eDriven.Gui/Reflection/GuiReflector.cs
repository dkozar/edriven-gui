using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Gui.Components;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class GuiReflector
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif
        /// <summary>
        /// Gets all loaded types from all assemblies
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllLoadedTypes()
        {
            return TypeReflector.GetAllLoadedTypes();
        }
        /// <summary>
        /// Gets all the types assignable from the supplied type (being subclasses of or implementing the given interface)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetAllLoadedTypesAsignableFrom(Type type)
        {
            List<Type> allTypes = GetAllLoadedTypes();
            List<Type> outputList = new List<Type>();

            foreach (Type t in allTypes)
            {
                if (t.IsClass)
                {
                    if (type.IsAssignableFrom(t))
                    {
                        outputList.Add(t);
                    }
                }
            }

            return outputList;
        }

        /// <summary>
        /// Gets all the types decorated with specified attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static List<Type> GetAllLoadedTypesDecoratedWith(Type attributeType, bool inherit = true)
        {
            List<Type> allTypes = GetAllLoadedTypes();
            List<Type> outputList = new List<Type>();
            
            foreach (var t in allTypes)
            {
                if (!t.IsClass) continue;
                var styleAttributes = t.GetCustomAttributes(attributeType, inherit);
                if (styleAttributes.Length > 0)
                {
                    outputList.Add(t);
                }
            }

            return outputList;
        }

        /// <summary>
        /// Gets all the methods decorated with specified attribute
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        internal static List<MethodInfo> GetMethodsDecoratedWith(Type type, Type attributeType)
        {
            if (!type.IsClass)
                return null;

            List<MethodInfo> outputList = new List<MethodInfo>();
            IEnumerable<MethodInfo> methods = type.GetMethods();
            foreach (var methodInfo in methods)
            {
                if (Attribute.IsDefined(methodInfo, attributeType))
                    outputList.Add(methodInfo);
            }

            return outputList;
        }

        /// <summary>
        /// Gets all the methods decorated with specified attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        internal static List<MethodInfo> GetMethodsInAllLoadedTypesDecoratedWith(Type attributeType)
        {
            List<Type> allTypes = GetAllLoadedTypes();
            List<MethodInfo> outputList = new List<MethodInfo>();

            foreach (var t in allTypes)
            {
                if (!t.IsClass) continue;
                var methods = GetMethodsDecoratedWith(t, attributeType);
                outputList.AddRange(methods);
            }

            return outputList;
        }

        #region Class attributes

        /// <summary>
        /// Gets all the attributes of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        internal static List<T> GetClassAttributes<T>(Type type, bool inherit = true)
        {
            List<T> outputList = new List<T>();
            var attributes = type.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    outputList.Add((T)attribute);
                }
            }
            return outputList;
        }

        /*/// <summary>
        /// Gets all the attributes of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasClassAttributes<T>(Type type, bool inherit = true)
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);
            return attributes.Length > 0;
        }*/

        #endregion

        #region Member attributes

        /*/// <summary>
        /// Gets all the attributes of the specified type
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasMemberAttributes<T>(MemberInfo memberInfo, bool inherit = true)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return attributes.Length > 0;
        }*/

        /*/// <summary>
        /// Gets all the member attributes of the specified type
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="inherit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetMemberAttributes<T>(MemberInfo memberInfo, bool inherit = true)
        {
            List<T> outputList = new List<T>();
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    outputList.Add((T)attribute);
                }
            }
            return outputList;
        }*/

        #endregion

        #region Method attributes

        /*/// <summary>
        /// Gets all the attributes of the specified type
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasMethodAttributes<T>(MethodInfo methodInfo, bool inherit = true)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(T), inherit);
            return attributes.Length > 0;
        }*/

        /*/// <summary>
        /// Gets all the method attributes of the specified type
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="inherit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetMethodAttributes<T>(MethodInfo methodInfo, bool inherit = true)
        {
            List<T> outputList = new List<T>();
            var attributes = methodInfo.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    outputList.Add((T)attribute);
                }
            }
            return outputList;
        }*/

        #endregion

        /// <summary>
        /// Exposes skin parts to other assemblies
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> GetSkinParts(Type componentType)
        {
            return SkinPartCache.Instance.Get(componentType);
        }

        /// <summary>
        /// Exposes skin states to other assemblies
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static List<string> GetSkinStates(Type componentType)
        {
            List<string> states = new List<string>();
            var skinStatesAttributes = GetClassAttributes<SkinStatesAttribute>(componentType);
            if (skinStatesAttributes.Count == 0)
                return states;

            SkinStatesAttribute attr = skinStatesAttributes[0];

            foreach (var state in attr.States)
            {
                states.Add(string.Format(@"""{0}""", state));
            }

            return states;
        }
    }
}
