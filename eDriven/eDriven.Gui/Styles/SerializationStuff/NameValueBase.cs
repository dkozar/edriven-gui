using System;
using eDriven.Gui.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The base class for serialized property and media query
    /// </summary>
    [Serializable]
    public class NameValueBase : IEquatable<NameValueBase>
    {
        #region Metadata

        /// <summary>
        /// Media query identifier
        /// </summary>
        public string Name = String.Empty;

        /* Internal set!!! */

        /// <summary>
        /// Full type name
        /// </summary>
        public string Type = String.Empty;

        /* Internal set!!! */

        /// <summary>
        /// Serialized type switch<br/>
        /// Switching to a specified slot for getting/setting a value
        /// </summary>
        public SerializedType SerializedType = SerializedType.ObjectReference;

        #endregion

        #region Slots

        ///<summary>
        ///</summary>
        public int Int;

        ///<summary>
        ///</summary>
        public bool Bool;

        ///<summary>
        ///</summary>
        public float Float;

        ///<summary>
        ///</summary>
        public string String;

        ///<summary>
        ///</summary>
        public Color Color;

        ///<summary>
        ///</summary>
        public LayerMask LayerMask;

        ///<summary>
        ///</summary>
        public string Enum;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Vector2;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Vector3;

        /// <summary>
        /// 
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// 
        /// </summary>
        public Char Char;

        /// <summary>
        /// 
        /// </summary>
        public AnimationCurve AnimationCurve;

        /// <summary>
        /// 
        /// </summary>
        public Bounds Bounds;

        /// <summary>
        /// 
        /// </summary>
        public Gradient Gradient;

        /// <summary>
        /// 
        /// </summary>
        public Quaternion Quaternion;

        ///<summary>
        /// UnityEngine.Object
        ///</summary>
        public Object ObjectReference;

        #endregion

        /// <summary>
        /// Depending of a serialized type switch, the setter saves the value to the specified field (called slot)<br/>
        /// Also, getter gets the value from the specified slot
        /// </summary>
        public object Value
        {
            get
            {
                switch (SerializedType)
                {
                    case SerializedType.Integer:
                        return Int;
                    case SerializedType.Boolean:
                        return Bool;
                    case SerializedType.Float:
                        return Float;
                    case SerializedType.String:
                        return String;
                    case SerializedType.Color:
                        return Color;
                    case SerializedType.LayerMask:
                        return LayerMask;
                    case SerializedType.Enum:
                        // enum is saved as string (selected value name) 
                        var type = GlobalTypeDictionary.Instance.Get(Type);
                        if (string.IsNullOrEmpty(Enum)) // not yet defined
                            return System.Enum.GetValues(type).GetValue(0); // so return first option
                        return System.Enum.Parse(type, Enum); // parse string
                    case SerializedType.Vector2:
                        return Vector2;
                    case SerializedType.Vector3:
                        return Vector3;
                    case SerializedType.Rect:
                        return Rect;
                    case SerializedType.Character:
                        return Char;
                    case SerializedType.AnimationCurve:
                        return AnimationCurve;
                    case SerializedType.Bounds:
                        return Bounds;
                    case SerializedType.Gradient:
                        return Gradient;
                    case SerializedType.Quaternion:
                        return Quaternion;
                    default:
                        return ObjectReference;
                }
            }
            set
            {
                //Debug.Log("Set value: " + value + " [Type: " + Type + "]");
                //var type = value.GetType();

                if (null == value)
                    return;

                /*if (null == value)
                    value = StyleDeclaration.UNDEFINED;*/

                try
                {
                    switch (SerializedType)
                    {
                        case SerializedType.Integer:
                            Int = (int)value;
                            break;
                        case SerializedType.Boolean:
                            Bool = (bool)value;
                            break;
                        case SerializedType.Float:
                            Float = (float)value;
                            break;
                        case SerializedType.String:
                            String = (string)value;
                            break;
                        case SerializedType.Color:
                            Color = (Color)value;
                            break;
                        case SerializedType.LayerMask:
                            Color = (Color)value;
                            break;
                        case SerializedType.Enum:
                            //Enum = (string) value;
                            //Enum = System.Enum.GetName(value.GetType(), value);
                            Enum = System.Enum.GetName(GlobalTypeDictionary.Instance.Get(Type), value);
                            break;
                        case SerializedType.Vector2:
                            Vector2 = (Vector2)value;
                            break;
                        case SerializedType.Vector3:
                            Vector2 = (Vector3)value;
                            break;
                        case SerializedType.Rect:
                            Rect = (Rect)value;
                            break;
                        case SerializedType.Character:
                            Char = (Char)value;
                            break;
                        case SerializedType.AnimationCurve:
                            AnimationCurve = (AnimationCurve)value;
                            break;
                        case SerializedType.Bounds:
                            Bounds = (Bounds)value;
                            break;
                        case SerializedType.Gradient:
                            Gradient = (Gradient)value;
                            break;
                        case SerializedType.Quaternion:
                            Quaternion = (Quaternion)value;
                            break;
                        default:
                            ObjectReference = (Object)value;
                            break;
                    }
                }
                catch (InvalidCastException)
                {
                    //throw new Exception(string.Format("{0} -> Cannot cast from {1} to {2}", Id, value, Type));
                    Debug.LogError(String.Format("{0} -> Cannot cast from {1} to {2}", Name, value, Type));
                }
            }
        }

        /// <summary>
        /// Creates a new style property, taking care of setting up the appropriate type switch
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NameValueBase CreateProperty<T>(string id, Type type) where T : NameValueBase, new()
        {
            var prop = default(T);
            try
            {
                prop = new T { Type = type.FullName };

                if (type == typeof(int) || type == typeof(int?))
                {
                    prop.SerializedType = SerializedType.Integer;
                }
                else if (type == typeof(bool) || type == typeof(bool?))
                {
                    prop.SerializedType = SerializedType.Boolean;
                }
                else if (type == typeof(float) || type == typeof(float?))
                {
                    prop.SerializedType = SerializedType.Float;
                }
                else if (type == typeof(string))
                {
                    prop.SerializedType = SerializedType.String;
                }
                else if (type == typeof(Color) || type == typeof(Color?))
                {
                    prop.SerializedType = SerializedType.Color;
                }
                else if (type == typeof(LayerMask))
                {
                    prop.SerializedType = SerializedType.LayerMask;
                }
                else if (type.IsEnum) //else if (type == typeof(Enum))
                {
                    prop.SerializedType = SerializedType.Enum;
                }
                else if (type == typeof(Vector2))
                {
                    prop.SerializedType = SerializedType.Vector2;
                }
                else if (type == typeof(Vector3))
                {
                    prop.SerializedType = SerializedType.Vector3;
                }
                else if (type == typeof(Rect))
                {
                    prop.SerializedType = SerializedType.Rect;
                }
                else if (type == typeof(Char))
                {
                    prop.SerializedType = SerializedType.Character;
                }
                else if (type == typeof(AnimationCurve))
                {
                    prop.SerializedType = SerializedType.AnimationCurve;
                }
                else if (type == typeof(Bounds))
                {
                    prop.SerializedType = SerializedType.Bounds;
                }
                else if (type == typeof(Gradient))
                {
                    prop.SerializedType = SerializedType.Gradient;
                }
                else if (type == typeof(Quaternion))
                {
                    prop.SerializedType = SerializedType.Quaternion;
                }
                else
                {
                    prop.SerializedType = SerializedType.ObjectReference;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(String.Format(@"Cannot create property of type [{0}]. Exception: {1}", type, ex));
            }

            if (null != prop)
                prop.Name = id;

            return prop;
        }

        /// <summary>
        /// Values are pushed to slots (properties of this object) depending of the type<br/>
        /// This method is used for determining the slot to write to/read from
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetSlotName(SerializedType enumValue)
        {
            switch (enumValue)
            {
                case SerializedType.Integer:
                    return "Int";
                case SerializedType.Boolean:
                    return "Bool";
                case SerializedType.Float:
                    return "Float";
                case SerializedType.String:
                    return "String";
                case SerializedType.Color:
                    return "Color";
                case SerializedType.LayerMask:
                    return "LayerMask";
                case SerializedType.Enum:
                    return "Enum";
                case SerializedType.Vector2:
                    return "Vector2";
                case SerializedType.Vector3:
                    return "Vector3";
                case SerializedType.Rect:
                    return "Rect";
                case SerializedType.Character:
                    return "Char";
                case SerializedType.AnimationCurve:
                    return "AnimationCurve";
                case SerializedType.Bounds:
                    return "Bounds";
                case SerializedType.Gradient:
                    return "Gradient";
                case SerializedType.Quaternion:
                    return "Quaternion";
                default:
                    return "ObjectReference";
            }
        }

        #region Equals

        public bool Equals(NameValueBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(Type, other.Type) &&
                   SerializedType == other.SerializedType && Int == other.Int && Bool.Equals(other.Bool) &&
                   Float.Equals(other.Float) && string.Equals(String, other.String) && Color.Equals(other.Color) &&
                   LayerMask.Equals(other.LayerMask) && Equals(Enum, other.Enum) &&
                   Equals(ObjectReference, other.ObjectReference) && Vector2.Equals(other.Vector2) &&
                   Vector3.Equals(other.Vector3) && Rect.Equals(other.Rect) && Char == other.Char &&
                   Equals(AnimationCurve, other.AnimationCurve) && Bounds.Equals(other.Bounds) &&
                   Equals(Gradient, other.Gradient) && Quaternion.Equals(other.Quaternion);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NameValueBase)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)SerializedType;
                hashCode = (hashCode * 397) ^ Int;
                hashCode = (hashCode * 397) ^ Bool.GetHashCode();
                hashCode = (hashCode * 397) ^ Float.GetHashCode();
                hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ LayerMask.GetHashCode();
                hashCode = (hashCode * 397) ^ (Enum != null ? Enum.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ObjectReference != null ? ObjectReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Vector2.GetHashCode();
                hashCode = (hashCode * 397) ^ Vector3.GetHashCode();
                hashCode = (hashCode * 397) ^ Rect.GetHashCode();
                hashCode = (hashCode * 397) ^ Char.GetHashCode();
                hashCode = (hashCode * 397) ^ (AnimationCurve != null ? AnimationCurve.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Bounds.GetHashCode();
                hashCode = (hashCode * 397) ^ (Gradient != null ? Gradient.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Quaternion.GetHashCode();
                return hashCode;
            }
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(NameValueBase left, NameValueBase right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(NameValueBase left, NameValueBase right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, SerializedType: {2}", Name, Type, SerializedType);
        }

        #endregion
    }
}