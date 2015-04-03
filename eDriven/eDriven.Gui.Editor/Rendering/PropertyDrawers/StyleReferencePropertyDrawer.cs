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

using eDriven.Gui.Styles;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    [CustomPropertyDrawer(typeof(SkinStyleReference))]
// ReSharper disable UnusedMember.Global
    internal class StyleReferencePropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {

            //return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Skin")) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("StyleName"));
            return 16*3; // label + 2 property fields
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            position.height = 16;
            var labelPos = new Rect(position) {x = 23};
            GUI.Label(labelPos, label);
            position.y += 16;
            EditorGUI.indentLevel = 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(SkinStyleReference.SKIN));
            position.y += 16;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(SkinStyleReference.STYLE_NAME));
            EditorGUI.indentLevel = 1;
        }
    }
}