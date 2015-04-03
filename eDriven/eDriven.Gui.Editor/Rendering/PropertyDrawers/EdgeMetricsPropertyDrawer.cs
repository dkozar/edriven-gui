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

using eDriven.Gui.Geom;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    [CustomPropertyDrawer(typeof(EdgeMetrics))]
// ReSharper disable UnusedMember.Global
    internal class EdgeMetricsPropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {

            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Left")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Right")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Top")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Bottom"));
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            position.height = 16;
            position.width /= 4;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Left"));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Right"));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Top"));
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Bottom"));
        }
    }
}