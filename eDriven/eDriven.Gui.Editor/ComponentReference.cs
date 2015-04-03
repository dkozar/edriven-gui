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
using eDriven.Core.Signals;
using eDriven.Gui.Components;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Editor
{
    public static class ComponentReference
    {
        public static string NewLine = @"
";
        public static string Line = @"----------------------------------------------------------------------------------------";

        /*/// <summary>
        /// Gets al relevant component data as a string
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string Describe(Type componentType)
        {
            var retVal = "Component: " + componentType.FullName + NewLine + Line + NewLine + NewLine;

            retVal += GetEvents(componentType);
            retVal += GetStyles(componentType);
            if (typeof (SkinnableComponent).IsAssignableFrom(componentType))
            {
                retVal += GetSkins(componentType);
            }

            return retVal;
        }*/

        public static string Describe(Type componentType, bool showEvents, bool showMulticastDelegates, bool showStyles, bool showSkins, bool showSkinParts, bool showSkinStates, bool showSignals)
        {
            var retVal = "Component: " + componentType.Name + NewLine + Line + NewLine;
            retVal += "Namespace: " + componentType.Namespace + NewLine;
            retVal += "Inheritance: " + GetInheritance(componentType) + NewLine;
            //retVal += "Interfaces: " + GetInterfaces(componentType) + NewLine;
            retVal += "Subclasses: " + GetSubclasses(componentType) + NewLine;

            retVal += NewLine;

            if (showEvents)
            {
                retVal += GetEvents(componentType);
            }

            if (showMulticastDelegates)
            {
                retVal += GetMulticastDelegates(componentType);
            }

            if (showStyles)
            {
                retVal += GetStyles(componentType);
            }

            if (showSkins)
            {
                retVal += GetSkins(componentType);
            }

            if (showSkinParts)
            {
                retVal += GetSkinParts(componentType);
            }

            if (showSkinStates)
            {
                retVal += GetSkinStates(componentType);
            }

            if (showSignals)
            {
                retVal += GetSignals(componentType);
            }

            return retVal;
        }

        private static string GetInheritance(Type componentType)
        {
            List<string> list = new List<string>();
            while (null != componentType && (componentType != typeof(Component).BaseType)) // all up to Component
            {
                list.Add(componentType.Name);
                componentType = componentType.BaseType;
            }

            return string.Join(" → ", list.ToArray());
        }

        /*private static string GetInterfaces(Type componentType)
        {
            var interfaces = componentType.GetInterfaces();

            var list = new List<string>();
            foreach (Type type in interfaces)
            {
                list.Add(type.Name);
            }

            return string.Join(", ", list.ToArray());
        }*/

        private static string GetSubclasses(Type componentType)
        {
            var list = new List<string>();
            foreach (Type type in GlobalTypeDictionary.Instance.Values)
            {
                if (type.BaseType == componentType)
                    list.Add(type.Name);
            }
            return list.Count == 0 ? "-" : string.Join(", ", list.ToArray());
        }

        #region Styles

        /// <summary>
        /// Describes component styles
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetStyles(Type componentType)
        {
            var styles = EditorReflector.GetStyleAttributes(componentType);
            styles.Sort(StyleSort);

            StringBuilder sb = new StringBuilder();

            foreach (var styleAttribute in styles)
            {
                sb.AppendLine(styleAttribute.ToString());
            }

            return string.Format(@"Styles ({0}):
{1}
{2}", styles.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        private static int StyleSort(StyleAttribute x, StyleAttribute y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Skins

        /// <summary>
        /// Describes component skins
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetSkins(Type componentType)
        {
            if (!typeof(SkinnableComponent).IsAssignableFrom(componentType))
            {
                return string.Format(@"Skins: Not skinnable." + NewLine + NewLine);
            }
            
            return EditorReflector.GetSkins(componentType) + NewLine/* + NewLine*/;
        }

        /*private static int ClassNameSort(Type x, Type y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }*/

        #endregion

        #region Skin parts
        
        /// <summary>
        /// Describes skin parts
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetSkinParts(Type componentType)
        {
            if (!typeof(SkinnableComponent).IsAssignableFrom(componentType))
            {
                return string.Format(@"Skin parts: Not a skinnable component." + NewLine + NewLine);
            }

            var dict = GuiReflector.GetSkinParts(componentType); // string->bool

            var list = new List<string>();
            foreach (string key in dict.Keys)
            {
                list.Add(key);
            }
            list.Sort();

            StringBuilder sb = new StringBuilder();
            foreach (var name in list)
            {
                MemberWrapper mw = new MemberWrapper(componentType, name);
                sb.AppendLine(string.Format("{0} [Type: {1}, Required: {2}]", name, mw.MemberType, dict[name]));
            }

            return string.Format(@"Skin parts ({0}):
{1}
{2}", list.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        #endregion

        #region Skin states

        /// <summary>
        /// Describes skin states
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetSkinStates(Type componentType)
        {
            if (!typeof(SkinnableComponent).IsAssignableFrom(componentType))
            {
                return string.Format(@"Skin states: Not a skinnable component." + NewLine + NewLine);
            }

            var list = GuiReflector.GetSkinStates(componentType); // string->bool

            StringBuilder sb = new StringBuilder();
            foreach (var name in list)
            {
                sb.AppendLine(name);
            }

            return string.Format(@"Skin states ({0}):
{1}
{2}", list.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        #endregion

        #region Events

        /// <summary>
        /// Describes component events
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetEvents(Type componentType)
        {
            //var events = ReflectionUtil.GetEvents(componentType);
            var events = CoreReflector.GetClassAttributes<EventAttribute>(componentType);
            events.Sort(EventSort);

            StringBuilder sb = new StringBuilder();

            foreach (var eventAttribute in events)
            {
                sb.AppendLine(eventAttribute.ToString());
            }

            return string.Format(@"Events ({0}):
{1}
{2}", events.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        /// <summary>
        /// Describes mulricast delegates
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static string GetMulticastDelegates(Type componentType)
        {
            var memberNames = CoreReflector.GetFieldAndPropertyNames(componentType);
            //memberNames.Sort(MemberInfoSort);
            List<string> data = new List<string>();
            foreach (var name in memberNames)
            {
                MemberWrapper mw = new MemberWrapper(componentType, name);
                if (mw.MemberType == typeof (Core.Events.MulticastDelegate))
                {
                    var output = mw.MemberInfo.Name;

                    /*var clazz = mw.MemberInfo.DeclaringType;
                    if (null != clazz)
                    {
                        var types = 


                        var instance = Activator.CreateInstance(clazz);
                        var value = mw.GetValue(instance);
                        if (null != value)
                            output = string.Format(@"{0} [{1}]", output, value.GetType().FullName);
                    }*/
                    
                    var attributes = CoreReflector.GetMemberAttributes<EventAttribute>(mw.MemberInfo);
                    if (attributes.Count > 0)
                    {
                        var eventAttribute = attributes[0];
                        output = string.Format(@"{0} → {1}", output, eventAttribute);
                    }

                    data.Add(output);
                }
            }

            data.Sort();

            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.AppendLine(item);
            }

            return string.Format(@"Multicast delegates ({0}):
{1}
{2}", data.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        private static int EventSort(EventAttribute x, EventAttribute y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }
        
        #endregion

        #region Signals

        private static string GetSignals(Type componentType)
        {
            var memberNames = CoreReflector.GetFieldAndPropertyNames(componentType);
            //memberNames.Sort(MemberInfoSort);
            List<MemberInfo> infos = new List<MemberInfo>();
            foreach (var name in memberNames)
            {
                MemberWrapper mw = new MemberWrapper(componentType, name);
                if (mw.MemberType == typeof(Signal))
                    infos.Add(mw.MemberInfo);
            }

            infos.Sort(MemberInfoSort);

            StringBuilder sb = new StringBuilder();
            foreach (var memberInfo in infos)
            {
                sb.AppendLine(memberInfo.Name);
            }

            return string.Format(@"Signals ({0}):
{1}
{2}", infos.Count, Line, sb) + NewLine/* + NewLine*/;
        }

        private static int MemberInfoSort(MemberInfo x, MemberInfo y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

    }
}
