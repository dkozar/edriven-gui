using System;
using System.Reflection;
using eDriven.Gui.Components;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Serialized media query
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Serializable]
    public class MediaQuery : NameValueBase, ICloneable
    {
        /// <summary>
        /// Parameter descriptions
        /// </summary>
        public Type[] Parameters { get; set; }

        /// <summary>
        /// Creates style property from StyleAttribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MediaQuery FromAttribute(StyleAttribute attribute)
        {
            var value = attribute.GetDefault();

            /**
             * 1. If the value is a skin class, we should switch to string type
             * because we cannot serialize the type
             * Hopwever, both type and string are valid for skin class
             * */
            if (value is Type)
            {
                var type = (Type)value;
                if (typeof(Skin).IsAssignableFrom(type))
                {
                    /*if (null != value)
                    attribute.Type = value.GetType(); // default*/

                    attribute.Type = typeof(string); // we cannot serialize type, so switching to string
                    value = type.FullName; // using string instead (full id as default value)
                }
            }

            var prop = CreateProperty<MediaQuery>(attribute.Name, attribute.Type);
            //prop.Name = attribute.Name;

            /**
             * We cannot serialize GUIStyle generated from code as object reference
             * */
            if (null != value)
            {
                prop.Value = value;
            }

            return (MediaQuery) prop;
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
            MediaQuery clone = new MediaQuery
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