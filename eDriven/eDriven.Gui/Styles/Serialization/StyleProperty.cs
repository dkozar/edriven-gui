using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Animation;
using eDriven.Gui.Components;
using UnityEngine;

#pragma warning disable 1591
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace eDriven.Gui.Styles.Serialization
{
    /// <summary>
    /// Serialized style property
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Serializable]
    public class StyleProperty : NameValueBase, ICloneable
    {
        /// <summary>
        /// List of style types that cannot be styled using the inspector
        /// </summary>
        public static List<Type> NonSerializableStyleTypes = new List<Type>
        {
            typeof(GUIStyle), typeof(GUISkin), typeof(IAnimation), typeof(Type)/*, typeof(TweenBase)*/
        };

        /// <summary>
        /// Creates style property from StyleAttribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static StyleProperty FromAttribute(StyleAttribute attribute)
        {
            var value = attribute.GetDefault();

            /**
             * 1. If the value is a skin class, we should switch to string type
             * because we cannot serialize the type
             * Hopwever, both type and string are valid for skin class
             * */
            if (value is Type)
            {
                var type = (Type) value;
                if (typeof (Skin).IsAssignableFrom(type))
                {
                    /*if (null != value)
                    attribute.Type = value.GetType(); // default*/

                    attribute.Type = typeof(string); // we cannot serialize type, so switching to string
                    value = type.FullName; // using string instead (full name as default value)
                }
            }

            var prop = CreateProperty<StyleProperty>(attribute.Name, attribute.Type);
            //prop.Name = attribute.Name;

            /**
             * We cannot serialize GUIStyle generated from code as object reference
             * */
            if (null != value && !NonSerializableStyleTypes.Contains(attribute.Type))
            {
                prop.Value = value;
            }
            
            return (StyleProperty) prop;
        }

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            StyleProperty clone = new StyleProperty
            {
                Name = Name,
                Type = Type,
                SerializedType = SerializedType,
                // #####
                Int = Int,
                Bool = Bool,
                Float = Float,
                String = String,
                Color = Color,
                LayerMask = LayerMask,
                Enum = Enum,
                Vector2 = Vector2,
                Vector3 = Vector3,
                Rect = Rect,
                Char = Char,
                AnimationCurve = AnimationCurve,
                Bounds = Bounds,
                Gradient = Gradient,
                Quaternion = Quaternion,
                ObjectReference = ObjectReference
            };
            return clone;
        }

        #endregion
    }
}

// ReSharper restore FieldCanBeMadeReadOnly.Global
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnusedMember.Global
// ReSharper restore InconsistentNaming
#pragma warning restore 1591