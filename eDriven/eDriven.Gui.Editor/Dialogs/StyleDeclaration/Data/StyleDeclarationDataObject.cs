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
using System.Text;
using eDriven.Gui.Editor.Dialogs.Commands;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEditor;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// This object represents the data collected by the StyleDeclarationDialog<br/>
    /// If creating a new declaration, we should pass the newly created data object<br/>
    /// If editing the old one, we should pass the data belonging to declaration
    /// </summary>
    internal class StyleDeclarationDataObject
    {
        /// <summary>
        /// A reference to the stylesheet that's being edited (in edit mode)
        /// </summary>
        public eDrivenStyleSheet StyleSheet;

        /// <summary>
        /// The provider ID that will is serialized with style declaration, used for lookup
        /// </summary>
        public string ModuleId;

        /// <summary>
        /// Component type (string)
        /// </summary>
        public string Type;

        /// <summary>
        /// Class
        /// </summary>
        public string Class = string.Empty;

        /// <summary>
        /// ID
        /// </summary>
        public string Id = string.Empty;

        /// <summary>
        /// Style attribute (collected from the actual class)
        /// </summary>
        public List<StyleProperty> StyleProperties = new List<StyleProperty>();

        /// <summary>
        /// Media queries
        /// </summary>
        public List<MediaQuery> MediaQueries = new List<MediaQuery>();

        public bool AllowSubjectOmmision;

        public override string ToString()
        {
            return string.Format(@"Type: ""{0}"",
StyleProperties: {1}", Type ?? "-", StyleProperties.Count);
        }

        public static StyleDeclarationDataObject FromSerializedProperty(SerializedProperty declaration)
        {
            return new StyleDeclarationDataObject
            {
                ModuleId = declaration.FindPropertyRelative("Module").stringValue,
                Type = declaration.FindPropertyRelative("Type").stringValue,
                Class = declaration.FindPropertyRelative("Class").stringValue,
                Id = declaration.FindPropertyRelative("Id").stringValue
            };
        }

        public void UpdateSerializedProperty(SerializedProperty declaration)
        {
            declaration.FindPropertyRelative("Module").stringValue = ModuleId;
            declaration.FindPropertyRelative("Type").stringValue = Type;
            declaration.FindPropertyRelative("Class").stringValue = Class;
            declaration.FindPropertyRelative("Id").stringValue = Id;

            UpdateProperties(declaration);
            UpdateMediaQueries(declaration);

            declaration.serializedObject.ApplyModifiedProperties();
        }

        private void UpdateProperties(SerializedProperty declaration)
        {
            var properties = declaration.FindPropertyRelative("Properties");
            var oldCount = properties.arraySize;
            List<SerializedProperty> oldProps = new List<SerializedProperty>();

            for (int i = 0; i < oldCount; i++)
            {
                SerializedProperty prop = properties.GetArrayElementAtIndex(i);
                oldProps.Add(prop);
            }

            List<string> newPropertyNames = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (StyleProperty styleProperty in StyleProperties)
            {
                var name = styleProperty.Name;
                newPropertyNames.Add(name);
                sb.AppendLine(name);
            }
            
            // Remove non-existing (reverse loop)
            for (int i = properties.arraySize - 1; i >= 0; i--)
            {
                var propName = properties.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                if (!newPropertyNames.Contains(propName))
                {
                    properties.DeleteArrayElementAtIndex(i);
                }
            }

            //Debug.Log("New size after deletion: " + properties.arraySize);

            // walk through new properties
            // if no corresponding property found in the old serialized array:
            // 1) make room for the property
            // 2) copy new name and type
            var newCount = StyleProperties.Count;
            for (int i = 0; i < newCount; i++)
            {
                var name = StyleProperties[i].Name;

                bool shouldInsertOrAddNewElement = i > properties.arraySize - 1;

                if (!shouldInsertOrAddNewElement)
                {
                    string oldPropName = properties.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                    if (oldPropName != name)
                        shouldInsertOrAddNewElement = true;
                }

                if (shouldInsertOrAddNewElement)
                {
                    if (i == properties.arraySize)
                        properties.arraySize++;
                    else
                        properties.InsertArrayElementAtIndex(i);

                    //Debug.Log("  --> " + properties.arraySize);

                    var prop = properties.GetArrayElementAtIndex(i);

                    prop.FindPropertyRelative("Name").stringValue = name;

                    SerializedPropertyHelper.Apply(prop, StyleProperties[i]);
                }
            }
        }

        private void UpdateMediaQueries(SerializedProperty declaration)
        {
            var mediaQueries = declaration.FindPropertyRelative("MediaQueries");

            var oldCount = mediaQueries.arraySize;
            List<SerializedProperty> oldProps = new List<SerializedProperty>();

            for (int i = 0; i < oldCount; i++)
            {
                var prop = mediaQueries.GetArrayElementAtIndex(i);
                oldProps.Add(prop);
            }

            List<string> newQueryIDs = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (MediaQuery query in MediaQueries)
            {
                var id = query.Name;
                newQueryIDs.Add(id);
                sb.AppendLine(id);
            }

            // Remove non-existing (reverse loop)
            for (int i = mediaQueries.arraySize - 1; i >= 0; i--)
            {
                var query = mediaQueries.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                if (!newQueryIDs.Contains(query))
                {
                    mediaQueries.DeleteArrayElementAtIndex(i);
                }
            }

            //Debug.Log("New size after deletion: " + properties.arraySize);

            // walk through new properties
            // if no corresponding property found in the old serialized array:
            // 1) make room for the property
            // 2) copy new name and type
            var newCount = MediaQueries.Count;
            for (int i = 0; i < newCount; i++)
            {
                var name = MediaQueries[i].Name;
                var type = MediaQueries[i].Type;

                bool shouldInsertOrAddNewElement = i > mediaQueries.arraySize - 1;

                if (!shouldInsertOrAddNewElement)
                {
                    string oldQueryId = mediaQueries.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                    if (oldQueryId != name)
                        shouldInsertOrAddNewElement = true;
                }

                if (shouldInsertOrAddNewElement)
                {
                    if (i == mediaQueries.arraySize)
                        mediaQueries.arraySize++;
                    else
                        mediaQueries.InsertArrayElementAtIndex(i);

                    //Debug.Log("  --> " + properties.arraySize);

                    var prop = mediaQueries.GetArrayElementAtIndex(i);

                    prop.FindPropertyRelative("Name").stringValue = name;

                    // apply type, value etc.
                    SerializedPropertyHelper.Apply(prop, MediaQueries[i]);
                }
            }
        }

        //public void Reset()
        //{
        //    Debug.Log("Reset");
        //    Type = null;
        //    StyleProperties.Clear();
        //}
    }
}