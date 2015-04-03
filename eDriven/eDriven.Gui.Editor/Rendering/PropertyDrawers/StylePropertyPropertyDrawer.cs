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
using eDriven.Core.Reflection;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    [CustomPropertyDrawer(typeof(StyleProperty))]
// ReSharper disable UnusedMember.Global
    internal class StylePropertyPropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
    {
        private GUIContent _name;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Type type = null;
            var slot = GetSlot(property, ref type);
            if (null == slot)
                //Debug.Log("Null: " + label.text + "; property: ");
                return 20;
            return EditorGUI.GetPropertyHeight(slot, label);
        }

        private string _nameStr;
        private Type _propType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _nameStr = property.FindPropertyRelative("Name").stringValue;
            _propType = GetType(property);
            _name = new GUIContent(_nameStr);

            Type type = null;
            SerializedProperty prop = GetSlot(property, ref type);

            try
            {
                if (null != type)
                {
                    // 1. Object reference
                    prop.objectReferenceValue = EditorGUI.ObjectField(position, _name, prop.objectReferenceValue, type, false);    
                }
                else
                {
                    if (_propType.IsEnum)
                    {
                        // 2. Enum values
                        prop.stringValue = EnumHelper.Popup(position, _name, _propType, prop.stringValue);
                    }
                    else { 
                        // 3. Normal values
                        EditorGUI.PropertyField(position, prop, _name);
                    }
                }
            }
            catch (Exception ex)
            {
                // NOTE: Silent fail!
                GUI.Label(position, new GUIContent(_nameStr + " ???"));
            }
        }

        /// <summary>
        /// Extract a sub property (Int, String, Bool... etc.) from the serialized property container
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SerializedProperty GetSlot(SerializedProperty property, ref Type type)
        {
            SerializedProperty serializedTypeProp = property.FindPropertyRelative("SerializedType");
            var enumValue =
                (SerializedType) Enum.GetValues(typeof (SerializedType)).GetValue(serializedTypeProp.enumValueIndex);

            var propertyName = NameValueBase.GetSlotName(enumValue);
            if ("ObjectReference" == propertyName)
            {
                type = GetType(property);
            }

            return property.FindPropertyRelative(propertyName);
        }

        /// <summary>
        /// Extract a sub property (Int, String, Bool... etc.) from the serialized property container
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Type GetType(SerializedProperty property)
        {
            var strType = property.FindPropertyRelative("Type").stringValue;
            if (string.IsNullOrEmpty(strType))
                return null;
            return GlobalTypeDictionary.Instance.Get(strType);
        }
    }
}