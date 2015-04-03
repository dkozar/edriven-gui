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

using eDriven.Gui.Editor.Dialogs.Commands;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    [CustomPropertyDrawer(typeof(StyleSheet))]
// ReSharper disable UnusedMember.Global
    internal class StyleSheetPropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
    {
        private const float ButtonWidth = 20f, ButtonHeight = 15f, ButtonMargin = 6f;

        //public static float ButtonGroupWidth
        //{
        //    get
        //    {
        //        return ButtonWidth * 3 + ButtonMargin * 2;
        //    }
        //}

        private const int VerticalGap = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f; // since no stylesheet title (yet)

            /**
             * NOTE: This is weird (the following 3 lines
             * I commented out these on 20131129 because eDrivenGuiStyleSheet didn't render properly
             * (measured height of each declaration was zero)
             * It's weird because it worked fine with eDrivenStyleSheet and they both have the same base class
             * */
            /*if (!property.isExpanded)
            {
                return height;
            }*/

            SerializedProperty declarations = property.FindPropertyRelative("Declarations");
            int size = declarations.arraySize;

            if (size == 0)
            {
                return height; // +16f;
            }

            for (int i = 0; i < size; i++)
            {
                height += EditorGUI.GetPropertyHeight(declarations.GetArrayElementAtIndex(i));
            }

            // gaps
            if (size > 0)
                height += VerticalGap*(size - 1);

            return height;
        }

        //private int count;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            //EditorGUIUtility.LookLikeControls(position.width/2);
            EditorGUIUtility.labelWidth = position.width / 2;

            //position.width -= ButtonWidth * 3 + ButtonMargin * 2;
            ShowElements(position, property);
            //Debug.Log(count++);

            /*if (GUI.changed)
            {
                Debug.Log(string.Format("StyleSheetPropertyDrawer -> Changed"));
                ShouldProcessStyles = true;
            }*/

            if (ShouldProcessStyles)
            {
                property.serializedObject.ApplyModifiedProperties();
                if (EditorSettings.LiveStyling)
                    Gui.ProcessStyles();

                ShouldProcessStyles = false;
            }
        }

        private static int _size;
        private void ShowElements(Rect position, SerializedProperty property)
        {
            Rect buttonPosition = new Rect(position.x + ButtonMargin, position.y, ButtonWidth, ButtonHeight);
            //buttonPosition.x -= ButtonGroupWidth + 4;
            
            SerializedProperty declarations = property.FindPropertyRelative("Declarations");
            _size = declarations.arraySize;
            //GUI.Label(position, "declarations.arraySize: " + _size);

            int index = -1;

            for (int i = 0; i < _size; i++)
            {
                SerializedProperty element = declarations.GetArrayElementAtIndex(i);
                position.height = EditorGUI.GetPropertyHeight(element, GUIContent.none);
                EditorGUI.PropertyField(position, element, GUIContent.none, true);
                
                if (!eDrivenStyleSheetEditor.EditorLocked)
                    buttonPosition.y += StyleDeclarationPropertyDrawer.ToolbarHeight;

                if (element.isExpanded && !eDrivenStyleSheetEditor.EditorLocked)
                {
                    if (ShowButtons(buttonPosition, declarations, i))
                    {
                        index = i;
                    }    
                }
                
                buttonPosition.y = position.y += position.height;

                if (index < i)
                    buttonPosition.y = position.y += VerticalGap;
            }

            if (index > -1) { 
                declarations.DeleteArrayElementAtIndex(index);
                /*if (EditorSettings.LiveStyling)
                    Gui.ProcessStyles();*/
                ShouldProcessStyles = true;
            }
        }

        //private static Color _oldColor;
        private static bool ShowButtons(Rect position, SerializedProperty property, int index)
        {
            var result = false;

            var pos = new Rect(position);
            pos.y += 4;

            if (GUI.Button(pos, GuiContentCache.Instance.EditSelector, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonMid
            {
                //EditorUtility.DisplayDialog("Editing selector", "Not implemented.", "Close");
                SerializedProperty declaration = property.GetArrayElementAtIndex(index);
                EditStyleDeclarationCommand.Execute(declaration);
            }

            pos.x += ButtonWidth;
            if (GUI.Button(pos, GuiContentCache.Instance.Duplicate, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonMid
            {
                bool expanded = property.GetArrayElementAtIndex(index).isExpanded;
                property.InsertArrayElementAtIndex(index);
                SerializedProperty newItem = property.GetArrayElementAtIndex(index);
                newItem.Reset();
                newItem.isExpanded = expanded;
            }
            
            pos.x += ButtonWidth;
            if (GUI.Button(pos, GuiContentCache.Instance.Delete, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonRight
            {
                if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to remove this style declaration?", "OK", "Cancel"))
                    result = true;
            }

            // move buttons

            if (index == 0)
                GUI.enabled = false;

            pos.x += ButtonWidth;
            if (GUI.Button(pos, GuiContentCache.Instance.MoveUp, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonLeft
            {
                if (index > 0)
                {
                    bool expanded1 = property.GetArrayElementAtIndex(index).isExpanded;
                    bool expanded2 = property.GetArrayElementAtIndex(index - 1).isExpanded;
                    property.MoveArrayElement(index, index - 1);
                    property.GetArrayElementAtIndex(index).isExpanded = expanded1;
                    property.GetArrayElementAtIndex(index - 1).isExpanded = expanded2;

                    ShouldProcessStyles = true;
                }
            }
            pos.x += ButtonWidth;

            if (index == 0)
                GUI.enabled = true;

            if (index == _size - 1)
                GUI.enabled = false;

            if (GUI.Button(pos, GuiContentCache.Instance.MoveDown, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonLeft
            {
                bool expanded1 = property.GetArrayElementAtIndex(index).isExpanded;
                bool expanded2 = property.GetArrayElementAtIndex(index + 1).isExpanded;
                property.MoveArrayElement(index, index + 1);
                property.GetArrayElementAtIndex(index).isExpanded = expanded1;
                property.GetArrayElementAtIndex(index + 1).isExpanded = expanded2;

                ShouldProcessStyles = true;
            }

            if (index == _size - 1)
                GUI.enabled = true;

            pos.x += ButtonWidth;

            return result;
        }

        internal static bool ShouldProcessStyles;

	    
    }
}