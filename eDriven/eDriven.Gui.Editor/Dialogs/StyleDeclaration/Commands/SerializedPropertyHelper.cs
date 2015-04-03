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
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs.Commands
{
    internal static class SerializedPropertyHelper
    {
        /// <summary>
        /// Reads the style property value from a serialized value<br/>
        /// Used by the system when in need to read the serialized value
        /// </summary>
        /// <param name="serializedProperty"></param>
        public static object Read(SerializedProperty serializedProperty)
        {
            var typeStr = serializedProperty.FindPropertyRelative("Type").stringValue;
            var serializedType = (SerializedType)serializedProperty.FindPropertyRelative("SerializedType").enumValueIndex;
            
            //var type = value.GetType();
            if (!GlobalTypeDictionary.Instance.ContainsKey(typeStr))
                throw new Exception("Couldn't get type from GlobalTypeDictionary: " + typeStr);

            var type = GlobalTypeDictionary.Instance[typeStr];
            var slotName = NameValueBase.GetSlotName(serializedType);
            var valueProp = serializedProperty.FindPropertyRelative(slotName);

            if (type == typeof(int))
                return valueProp.intValue;
            if (type == typeof(bool))
                return valueProp.boolValue;
            if (type == typeof(float))
                return valueProp.floatValue;
            if (type == typeof(string))
                return valueProp.stringValue;
            if (type == typeof(Color))
                return valueProp.colorValue;
            /*else if (type == typeof(LayerMask))
                valueProp.objectReferenceValue = (LayerMask)styleProperty.Value;*/
            if (type.IsEnum) {// == typeof(Enum)) 
                return Enum.Parse(GlobalTypeDictionary.Instance.Get(typeStr), valueProp.stringValue);
            }
            if (type == typeof(Vector2))
                return valueProp.vector2Value;
            if (type == typeof(Vector3))
                return valueProp.vector3Value;
            if (type == typeof(Rect))
                return valueProp.rectValue;
            /*else if (type == typeof(Char))
                valueProp.stringValue = (Char)styleProperty.Value;*/
            if (type == typeof(AnimationCurve))
                return valueProp.animationCurveValue;
            if (type == typeof(Bounds))
                return valueProp.boundsValue;
            /*else if (type == typeof(Gradient))
                valueProp.Gradient = (Gradient)styleProperty.Value;*/
            if (type == typeof(Quaternion))
                return valueProp.quaternionValue;
            if (type == typeof(UnityEngine.Object))
                return valueProp.objectReferenceValue;

            return null;
        }

        /// <summary>
        /// Applies the style property value to a serialized value<br/>
        /// Used by the system when in need to update the serialized value (either just created or edited)
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="styleProperty"></param>
        public static void Apply(SerializedProperty serializedProperty, NameValueBase styleProperty)
        {
            serializedProperty.FindPropertyRelative("Type").stringValue =
                styleProperty.Type;
            serializedProperty.FindPropertyRelative("SerializedType").enumValueIndex =
                (int)styleProperty.SerializedType;

            var value = styleProperty.Value;
            if (null != value)
            {
                //var type = value.GetType();
                if (!GlobalTypeDictionary.Instance.ContainsKey(styleProperty.Type))
                    throw new Exception("Couldn't get type from GlobalTypeDictionary: " + styleProperty.Type);

                var type = GlobalTypeDictionary.Instance[styleProperty.Type];
                var slotName = NameValueBase.GetSlotName(styleProperty.SerializedType);
                var valueProp = serializedProperty.FindPropertyRelative(slotName);

                if (type == typeof(int))
                    valueProp.intValue = (int)styleProperty.Value;
                else if (type == typeof(bool))
                    valueProp.boolValue = (bool)styleProperty.Value;
                else if (type == typeof(float))
                    valueProp.floatValue = (float)styleProperty.Value;
                else if (type == typeof(string))
                    valueProp.stringValue = (string)styleProperty.Value;
                else if (type == typeof(Color))
                    valueProp.colorValue = (Color)styleProperty.Value;
                /*else if (type == typeof(LayerMask))
                    valueProp.objectReferenceValue = (LayerMask)styleProperty.Value;*/
                else if (type.IsEnum)
                {// == typeof(Enum)) 
                    //valueProp.intValue = (int)styleProperty.Value;
                    valueProp.stringValue = Enum.GetName(GlobalTypeDictionary.Instance.Get(styleProperty.Type), value);
                }
                else if (type == typeof(Vector2))
                    valueProp.vector2Value = (Vector2)styleProperty.Value;
                else if (type == typeof(Vector3))
                    valueProp.vector3Value = (Vector3)styleProperty.Value;
                else if (type == typeof(Rect))
                    valueProp.rectValue = (Rect)styleProperty.Value;
                /*else if (type == typeof(Char))
                    valueProp.stringValue = (Char)styleProperty.Value;*/
                else if (type == typeof(AnimationCurve))
                    valueProp.animationCurveValue = (AnimationCurve)styleProperty.Value;
                else if (type == typeof(Bounds))
                    valueProp.boundsValue = (Bounds)styleProperty.Value;
                /*else if (type == typeof(Gradient))
                    valueProp.Gradient = (Gradient)styleProperty.Value;*/
                else if (type == typeof(Quaternion))
                    valueProp.quaternionValue = (Quaternion)styleProperty.Value;
                else if (type == typeof(UnityEngine.Object))
                    valueProp.objectReferenceValue = (UnityEngine.Object)styleProperty.Value;
            }
        }
    }
}
