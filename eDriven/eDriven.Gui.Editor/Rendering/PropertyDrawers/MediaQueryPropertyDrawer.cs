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
using eDriven.Gui.Editor.Dialogs.Commands;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.MediaQueries;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    [CustomPropertyDrawer(typeof(MediaQuery))]
// ReSharper disable UnusedMember.Global
    internal class MediaQueryPropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
    {
        private GUIContent _name;
        private string _nameStr;
        private Type _propType;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // property height of the media query is always 20
            // that's because of the monitor icon height of 16 px
            // and the fact that EditorGUI.GetPropertyHeight() distorts the height
            // the height of 20 gives crisp icons
            return 20; //EditorGUI.GetPropertyHeight(slot, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _nameStr = property.FindPropertyRelative("Name").stringValue;
            _propType = GetType(property);
            
            Type type = null;
            SerializedProperty prop = GetSlot(property, ref type);

            var isScanning = EditorSettings.LiveStyling && Application.isPlaying;

            bool mediaQueryPasses = false;
            //var oldColor = GUI.backgroundColor;
            if (isScanning)
            {
                var value = SerializedPropertyHelper.Read(property);
                //Debug.Log("value: " + value);
                mediaQueryPasses = MediaQueryManager.Instance.EvaluateQuery(_nameStr, value);
                //GUI.backgroundColor = mediaQueryPasses ? Color.green : Color.red;
            }

            /*_name = new GUIContent(" " + _nameStr, mediaQueryPasses ?
                TextureCache.Instance.MediaQueryPass : TextureCache.Instance.MediaQueryFail);*/

            _name = new GUIContent(" " + _nameStr);

            /* draw icon */
            GUI.Label(position, mediaQueryPasses ? TextureCache.Instance.MediaQueryPass : TextureCache.Instance.MediaQueryFail);

            /* move a bit to the right */
            position.x += 16;
            position.width -= 16;
            position.height = 16; // height for the rest

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
                        prop.stringValue = EnumHelper.Popup(position, _name, _propType, prop.stringValue);
                    }
                    else
                    {
                        // 2. Normal values
                        EditorGUI.PropertyField(position, prop, _name); //, new GUIContent(label));    
                    }
                }
            }
            catch (Exception)
            {
                // NOTE: Silent fail!
                GUI.Label(position, new GUIContent(" " + _nameStr + " ???", TextureCache.Instance.MediaQuery));
            }

            /*if (isScanning)
            {
                GUI.backgroundColor = oldColor;
            }*/
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
            //Debug.Log("propertyName: " + propertyName);

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
            return GlobalTypeDictionary.Instance[strType];
        }
    }
}