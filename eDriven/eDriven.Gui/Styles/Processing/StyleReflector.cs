using System;
using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Handles the styling-related reflection
    /// </summary>
    public static class StyleReflector
    {
        /*///<summary>
        ///</summary>
        public static bool GuiComponentsOnly = false;*/

        private static readonly Type StyleClientType = typeof (IStyleClient);

        /// <summary>
        /// Gets all skin classes
        /// </summary>
        public static List<Type> GetAllStyleableClasses()
        {
            List<Type> allTypes = TypeReflector.GetAllLoadedTypes();
            List<Type> output = new List<Type>();

            foreach (Type type in allTypes)
            {
                if (type.IsClass)
                {
                    if (/*!GuiComponentsOnly || */StyleClientType.IsAssignableFrom(type))
                    {
                        if (CoreReflector.HasClassAttributes<StyleAttribute>(type))
                            output.Add(type);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Reads the default style value from style declaration
        /// TODO: Heavily optimize!
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        /*public static StyleableProxy GetProxy(object component)
        {
            //Debug.Log("Getting default style value for: " + styleName);
            //var fields = component.GetType().GetFields();

            // TODO: cache type->proxy

            //Debug.Log("component.GetType(): " + component.GetType());
            //var attributes = component.GetType().GetCustomAttributes(typeof (StyleableAttribute), true);
            var attributes = CoreReflector.GetClassAttributes<StyleableAttribute>(component.GetType());
            //Debug.Log(component + "; attributes.Length: " + attributes.Length);

            foreach (StyleableAttribute attribute in attributes)
            {
                /*if (null != attribute)
                {#1#
                //Debug.Log("Proxy found for component: " + component);
                return StyleableProxy.CreateProxy(component, attribute);
                /*}#1#
            }
            //Debug.Log("Coudn't find proxy on component: " + component);
            return null;
        }*/

        private static readonly Dictionary<Type, List<StyleAttribute>> StyleAttributeCache = new Dictionary<Type, List<StyleAttribute>>();

        /// <summary>
        /// Gets all style properties
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restrictToInspectableTypes"></param>
        public static List<StyleAttribute> GetStyleAttributes(Type type)
        {
            if (StyleAttributeCache.ContainsKey(type))
                return StyleAttributeCache[type];

            var styleAttributes = CoreReflector.GetClassAttributes<StyleAttribute>(type);

            List<StyleAttribute> attributes = new List<StyleAttribute>();

            foreach (StyleAttribute attribute in styleAttributes)
            {
                /* with "skinClass" style, the Type isn't required. Set it here */
                /*if (attribute.Name == "skinClass" && null == attribute.Type)
                    attribute.Type = typeof(object);*/

                if (/*!restrictToInspectableTypes || */(attribute.Name == "skinClass" || null != attribute.Type/* && StyleProperty.AlowedTypes.ContainsKey(attribute.Type)*/))
                {
                    /**
                     * Important: Avoid duplication
                     * Subclass attributes are being added before the superclass attributes, so we're fine
                     * */
                    var name = attribute.Name;

                    if (!attributes.Exists(delegate(StyleAttribute a)
                    {
                        return a.Name == name;
                    }))
                    {
                        attributes.Add(attribute);
                    }
                    else
                    {
                        //Debug.Log(type + " has duplicated attribute: " + name + ": " + attribute.GetDefault());
                    }
                }
            }

            StyleAttributeCache[type] = attributes;

            return attributes;
        }
    }
}